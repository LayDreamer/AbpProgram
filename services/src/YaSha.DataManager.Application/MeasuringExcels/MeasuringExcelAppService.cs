using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using OfficeOpenXml;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using YaSha.DataManager.StandardAndPolicy.Dto;
using Volo.Abp.Users;
using System.Linq.Dynamic.Core;
using System.Drawing;
using OfficeOpenXml.Style;


namespace YaSha.DataManager.MeasuringExcels
{
    public class MeasuringExcelAppService :
      CrudAppService<
         MeasuringExcel,
        MeasuringExcelDto,
         Guid,
         PagedAndSortedResultRequestDto>,
     IMeasuringExcelAppService
    {
        protected readonly IRepository<MeasuringExcel, Guid> _repository;

        private readonly ICurrentUser _currentUser;

        public string severPath = "https://bds.chinayasha.com/bdsfileservice/MeasuringExcels/";
        public MeasuringExcelAppService(IRepository<MeasuringExcel, Guid> repository, ICurrentUser currentUser)
            : base(repository)
        {
            _repository = repository;
            _currentUser = currentUser;
        }

        public override async Task<PagedResultDto<MeasuringExcelDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            var source = await _repository.GetListAsync();
            if (!string.IsNullOrEmpty(input.Sorting))
            {
                source = source.AsQueryable().OrderBy(input.Sorting).ToList();
            }
            else
            {
                source = source.OrderByDescending(e => e.CreationTime).ToList();
            }
            return new PagedResultDto<MeasuringExcelDto>(source.Count,
              base.ObjectMapper.Map<List<MeasuringExcel>, List<MeasuringExcelDto>>(source));
        }

        public override async Task DeleteAsync(Guid id)
        {
            try
            {
                var source = await _repository.GetAsync(id);
                if (source.Creator.Equals(_currentUser.Name))
                {
                    await _repository.DeleteAsync(id);
                }
            }
            catch { }
        }

        public async Task<ApiResultDto> UploadFile(IFormFile file)
        {
            var serverPath = "/ServerData/FileManagement/MeasuringExcels/";
            //var serverPath = "D:\\0\\01收到资料\\MeasuringExcels\\";
            string excelPath = serverPath + Path.GetFileName(file.FileName);
            int num = 1;
            bool success = false;
            while (File.Exists(excelPath))
            {
                excelPath = $"{serverPath}{Path.GetFileNameWithoutExtension(file.FileName)}({num}).xlsx";
                num++;
            }
            try
            {
                using (FileStream fileStream = new FileStream(excelPath, FileMode.Create, FileAccess.Write))
                {
                    await file.CopyToAsync(fileStream);
                }
                success = true;
            }
            catch (Exception ex) { }
            return new ApiResultDto()
            {
                Data = excelPath,
                Success = success,
            };
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<byte[]> DownloadExcelAsync(Guid id)
        {
            try
            {
                var source = await _repository.GetAsync(id);
                string path = source.FileAddress;
                if (string.IsNullOrEmpty(path)) return null;
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                var package = new ExcelPackage(new FileInfo(path));
                byte[] bytes = await package.GetAsByteArrayAsync();
                return bytes;
            }
            catch (Exception ex) { }
            return null;
        }



        public async Task<ApiResultDto> AddAsync(Object input)
        {
            string fileAddress = "";
            double designDiffer = 0;
            double minCavityValue = 0;
            try
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(input.ToString());
                fileAddress = GetStringValue(jo["fileAddress"]);
                designDiffer = GetDoubleValue(jo["designDiffer"]);
                minCavityValue = GetDoubleValue(jo["minCavityValue"]);

                if (!File.Exists(fileAddress)) return null;
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                var package = new ExcelPackage(new FileInfo(fileAddress));
                foreach (ExcelWorksheet sheet in package.Workbook.Worksheets)
                {
                    if (sheet.Name.Contains("平面"))
                    {
                        SetPlane(sheet, designDiffer, minCavityValue);
                    }
                    else if (sheet.Name.Contains("楼板"))
                    {
                        SetFloor(sheet);
                    }
                    else if (sheet.Name.Contains("梁"))
                    {
                        SetBeam(sheet);
                    }
                }
                package.Save();
                MeasuringExcelCreateDto excelDto = new MeasuringExcelCreateDto()
                {
                    Id = Guid.NewGuid(),
                    FileName = Path.GetFileName(fileAddress),
                    FileAddress = Path.Combine(severPath, Path.GetFileName(fileAddress)),
                    Creator = _currentUser.Name,
                    designDiffer = designDiffer.ToString(),
                    minCavityValue = minCavityValue.ToString(),
                };
                var data = ObjectMapper.Map<MeasuringExcelCreateDto, MeasuringExcel>(excelDto);
                await _repository.InsertAsync(data, autoSave: true);
                return new ApiResultDto()
                {
                    Success = true,
                };
            }
            catch (Exception ex)
            {
                return new ApiResultDto()
                {
                    Data = ex.Message,
                    Success = false,
                };
            }
        }

        /// <summary>
        /// 设置梁
        /// </summary>
        /// <param name="sheet"></param>
        public void SetBeam(ExcelWorksheet sheet)
        {
            int rowStart = 2;
            int colStart = 2;
            int beamRow = 3;//梁至轴线,梁高
            int rowEnd = GetRowEnd(sheet);
            int colEnd = GetColEnd(sheet, "梁");

            int design = -1;
            int minCavity = -1;
            int max = -1;
            int min = -1;
            int differ = -1;
            int designDiffer = -1;
            int final = -1;
            GetFieldRowIndex(sheet, rowStart, sheet.Dimension.End.Row, ref rowEnd, ref design, ref minCavity,
           ref max, ref min, ref differ, ref designDiffer, ref final);
            #region Old
            //int max = rowEnd + 2;
            //int min = max + 1;
            //int differ = min + 1;
            //int final = differ + 1;
            //InsertRowAndValue(sheet, max, 1, "最大值");
            //InsertRowAndValue(sheet, min, 1, "最小值");
            //InsertRowAndValue(sheet, differ, 1, "差值");
            //InsertRowAndValue(sheet, final, 1, "最终取值");
            #endregion

            //梁高=最小值+1000
            for (int i = colStart; i <= colEnd; i++)
            {
                double maxValue = double.MinValue;
                double minValue = double.MaxValue;

                for (int j = rowStart; j <= rowEnd; j++)
                {
                    double value = GetDoubleValue(sheet.Cells[j, i].Value);
                    if (value == 0) continue;
                    maxValue = Math.Max(maxValue, value);
                    minValue = Math.Min(minValue, value);
                }
                minValue = minValue == double.MaxValue ? 0 : minValue;
                sheet.Cells[max, i].Value = maxValue;
                sheet.Cells[min, i].Value = minValue;
                sheet.Cells[differ, i].Value = maxValue - minValue;

                string s = GetMergeValue(sheet, beamRow, i);
                if (s.Contains("轴线"))
                {
                    sheet.Cells[final, i].Value = minValue;
                }
                else if (s.Contains("高"))
                {
                    sheet.Cells[final, i].Value = minValue + 1000;
                }
            }

            SetExcelRangeFont(sheet.Cells[design, colStart, final + 1, colEnd]);
            var FirstTableRange = sheet.Cells[design, 1, final + 1, colEnd];
            SetExcelRangeStyle(FirstTableRange);
        }


        /// <summary>
        /// 设置楼板
        /// </summary>
        /// <param name="sheet"></param>
        public void SetFloor(ExcelWorksheet sheet)
        {
            int rowStart = 2;
            int colStart = 2;
            int rowEnd = GetRowEnd(sheet);
            int colEnd = GetColEnd(sheet, "楼板");

            int design = -1;
            int minCavity = -1;
            int max = -1;
            int min = -1;
            int differ = -1;
            int designDiffer = -1;
            int final = -1;
            GetFieldRowIndex(sheet, rowStart, sheet.Dimension.End.Row, ref rowEnd, ref design, ref minCavity,
           ref max, ref min, ref differ, ref designDiffer, ref final);


            #region Old
            //int max = rowEnd + 2;
            //int min = max + 1;
            //int differ = min + 1;
            //int final = differ + 1;
            //InsertRowAndValue(sheet, max, 1, "最大值");
            //InsertRowAndValue(sheet, min, 1, "最小值");
            //InsertRowAndValue(sheet, differ, 1, "差值");
            //InsertRowAndValue(sheet, final, 1, "最终取值");
            #endregion

            //楼板逻辑：上楼板=最小值+1000   下楼版不填   带卫字的下楼版=1000-最小值
            for (int i = colStart; i <= colEnd; i++)
            {
                double maxValue = double.MinValue;
                double minValue = double.MaxValue;
                string floorName = GetMergeValue(sheet, rowStart, i);
                string floorPosition = GetMergeValue(sheet, rowStart + 1, i);
                for (int j = rowStart; j <= rowEnd; j++)
                {
                    double value = GetDoubleValue(sheet.Cells[j, i].Value);
                    if (value == 0) continue;
                    maxValue = Math.Max(maxValue, value);
                    minValue = Math.Min(minValue, value);
                }
                minValue = minValue == double.MaxValue ? 0 : minValue;
                if (floorPosition.Contains("上") || floorName.Contains("卫"))
                {
                    sheet.Cells[max, i].Value = maxValue;
                    sheet.Cells[min, i].Value = minValue;
                    sheet.Cells[differ, i].Value = maxValue - minValue;
                }
                if (floorPosition.Contains("上"))
                {
                    sheet.Cells[final, i].Value = minValue + 1000;
                }
                else
                {
                    if (floorName.Contains("卫"))
                    {
                        sheet.Cells[final, i].Value = 1000 - minValue;
                    }
                }
            }

            SetExcelRangeFont(sheet.Cells[design, colStart, final + 1, colEnd]);
            var FirstTableRange = sheet.Cells[design, 1, final + 1, colEnd];
            SetExcelRangeStyle(FirstTableRange);
        }

        public int GetRowEnd(ExcelWorksheet sheet)
        {
            int rowEnd = 1;
            int rows = sheet.Dimension.End.Row;
            for (int i = 1; i <= rows; i++)
            {
                string value = GetMergeValue(sheet, i, 1);
                if (string.IsNullOrEmpty(value) || value.Equals("设计值")) break;
                rowEnd = i;
            }
            return rowEnd;
        }

        public int GetColEnd(ExcelWorksheet sheet, string name)
        {
            int cols = sheet.Dimension.End.Column;
            int colEnd = 1;
            for (int i = 1; i <= cols; i++)
            {
                if (!(GetMergeValue(sheet, 1, i).Contains(name) && sheet.Cells[1, i].Merge)) break;
                colEnd = i;
            }
            return colEnd;
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="inputDiffer">界面输入：设计允许偏差值</param>
        /// <param name="inputMinCavity">界面输入：产品安装最小空腔值</param>
        public void SetPlane(ExcelWorksheet sheet, double inputDiffer, double inputMinCavity)
        {
            try
            {
                if (sheet.Dimension == null) return;
                int rows = sheet.Dimension.End.Row;
                int cols = sheet.Dimension.End.Column;
                int index = 1;
                int rowStart = 2;//从第二行楼层开始读
                int rowEnd = -1;//读到该行结束
                int colEnd = 0;

                int design = -1;
                int minCavity = -1;
                int max = -1;
                int min = -1;
                int differ = -1;
                int designDiffer = -1;
                int finalValue = -1;
                GetFieldRowIndex(sheet, rowStart, rows, ref rowEnd, ref design, ref minCavity,
                ref max, ref min, ref differ, ref designDiffer, ref finalValue);

                List<Bay> bays = new List<Bay>();

                for (int i = 2; i <= cols; i++)
                {
                    if (GetMergeValue(sheet, index, i).Contains("墙") && sheet.Cells[index, i].Merge)
                    {
                        colEnd = i;
                        Bay currBay = null;
                        #region  获取开间及开间子数据
                        bool bayRoom = false;
                        string name = GetMergeValue(sheet, rowStart, i);
                        if (name.Contains('-'))
                        {
                            string s = name.Split('-')[1];
                            var aa = bays.Where(e => e.BayName.Equals(s)).ToList();
                            if (aa.Count == 0)
                            {
                                Bay bay = new Bay();
                                bay.BayName = name.Split('-')[1];
                                bay.children.Add(i);
                                bays.Add(bay);
                                currBay = bay;
                            }
                            else
                            {
                                currBay = aa[0];
                                aa[0].children.Add(i);
                            }
                        }
                        else if (name.Length == 1)
                        {
                            bayRoom = true;
                            var aa = bays.Where(e => e.BayName.Equals(name)).ToList();
                            if (aa.Count == 0)
                            {
                                Bay bay = new Bay();
                                bay.BayName = name;
                                bay.Index = i;
                                bays.Add(bay);
                                currBay = bay;
                            }
                            else
                            {
                                currBay = aa[0];
                                aa[0].Index = i;
                            }
                        }
                        #endregion


                        double maxValue = double.MinValue;
                        double minValue = double.MaxValue;
                        double compareFinal = GetDoubleValue(sheet.Cells[design, i].Value) - inputDiffer + 2 * inputMinCavity;//设计值-允许偏差值+产品最小空腔*2

                        for (int j = rowStart; j <= rowEnd; j++)
                        {
                            double value = GetDoubleValue(sheet.Cells[j, i].Value);
                            if (bayRoom) currBay.dic.Add(GetMergeValue(sheet, j, 1), value.ToString());
                            if (value == 0) continue;
                            maxValue = Math.Max(maxValue, value);
                            minValue = Math.Min(minValue, value);
                        }
                        //表头填充黄色的，不减；表头填充绿色的，减2倍；无填充的，减一个；
                        string rgb = sheet.Cells[rowStart, i].Style.Fill.BackgroundColor.Rgb;
                        double subtractCavity = inputMinCavity;
                        if (!string.IsNullOrEmpty(rgb))
                        {
                            subtractCavity = rgb.Equals("FFFFC000") ? 0 : subtractCavity;
                            subtractCavity = rgb.Equals("FF00B050") ? 2 * inputMinCavity : subtractCavity;
                        }

                        minValue = minValue == double.MaxValue ? 0 : minValue;
                        sheet.Cells[minCavity, i].Value = inputMinCavity;//界面输入的值赋予到表格中
                        sheet.Cells[max, i].Value = maxValue - subtractCavity;  //最大值 - 空腔 * 2
                        sheet.Cells[min, i].Value = minValue - subtractCavity; //最小值-空腔*2
                        sheet.Cells[differ, i].Value = GetDoubleValue(sheet.Cells[max, i].Value) - GetDoubleValue(sheet.Cells[min, i].Value); //最大值-最小值
                        sheet.Cells[designDiffer, i].Value = GetDoubleValue(sheet.Cells[design, i].Value) - GetDoubleValue(sheet.Cells[min, i].Value);     //设计值-最小值


                        if (bayRoom)
                        {
                            //取不到设计值，那就是取最小值,取不到最小值，那就是取设计值
                            sheet.Cells[finalValue, i].Value = maxValue >= compareFinal ? GetDoubleValue(sheet.Cells[design, i].Value) : GetDoubleValue(sheet.Cells[min, i].Value);
                            sheet.Cells[finalValue + 1, i].Value = minValue < compareFinal ? GetDoubleValue(sheet.Cells[min, i].Value) : GetDoubleValue(sheet.Cells[design, i].Value);
                        }
                    }
                    else if ((GetMergeValue(sheet, index, i).Contains("窗") || GetMergeValue(sheet, index, i).Contains("门")) && sheet.Cells[index, i].Merge)
                    {
                        double maxValue = double.MinValue;
                        double minValue = double.MaxValue;

                        for (int j = rowStart; j <= rowEnd; j++)
                        {
                            double value = GetDoubleValue(sheet.Cells[j, i].Value);
                            if (value == 0) continue;
                            maxValue = Math.Max(maxValue, value);
                            minValue = Math.Min(minValue, value);
                        }
                        minValue = minValue == double.MaxValue ? 0 : minValue;
                        sheet.Cells[max, i].Value = maxValue;
                        sheet.Cells[min, i].Value = minValue;
                        sheet.Cells[differ, i].Value = maxValue - minValue;
                        sheet.Cells[finalValue, i].Value = minValue;

                        var tableRange = sheet.Cells[max, i, finalValue, i];
                        SetExcelRangeFont(tableRange);
                        SetExcelRangeStyle(tableRange);
                    }
                }
                foreach (var bay in bays)
                {
                    if (bay.children.Count == 0 || bay.Index == 0) continue;

                    string s1 = GetMergeValue(sheet, finalValue, bay.Index);
                    string s2 = GetMergeValue(sheet, finalValue + 1, bay.Index);


                    if (s1 == GetMergeValue(sheet, design, bay.Index).ToString().Trim())
                    {
                        BayIsDesignValue(sheet, bay, finalValue, design);
                    }
                    else
                    {
                        BayIsMinValue(sheet, bay, finalValue, min, max, design, cols);
                    }

                    if (s2 == GetMergeValue(sheet, design, bay.Index).ToString().Trim())
                    {
                        BayIsDesignValue(sheet, bay, finalValue + 1, design);
                    }
                    else
                    {
                        BayIsMinValue(sheet, bay, finalValue + 1, min, max, design, cols);
                    }

                }


                int setIndex = finalValue + 3;
                InsertRowAndValue(sheet, setIndex, 2, "套数");
                SetSets(bays, sheet, finalValue, setIndex, colEnd);
                // SetSets(bays, sheet, finalValue + 1, setIndex + 1, colEnd);

                SetExcelRangeFont(sheet.Cells[max, 1, setIndex + 1, colEnd]);
                var FirstTableRange = sheet.Cells[max, 1, setIndex + 1, colEnd];
                SetExcelRangeStyle(FirstTableRange);
            }
            catch (Exception ex) { }
        }

        public void SetExcelRangeFont(ExcelRange range)
        {
            range.Style.Font.Name = "微软雅黑";
            range.Style.Font.Size = 11;
        }

        public void SetExcelRangeStyle(ExcelRange FirstTableRange)
        {
            FirstTableRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            FirstTableRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            FirstTableRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            FirstTableRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            FirstTableRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            FirstTableRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        }

        public void SetSets(List<Bay> bays, ExcelWorksheet sheet, int row, int setRow, int colEnd)
        {
            int count = 0;
            string s = "";
            string less = "";
            int col = 2;
            List<string> lists = new List<string>();
            List<string> lessThanLists = new List<string>();
            for (int i = 0; i < bays.Count; i++)
            {
                string value = GetMergeValue(sheet, row, bays[i].Index);
                sheet.Cells[setRow, col + i].Value = value;

                string lessValue = GetMergeValue(sheet, row + 1, bays[i].Index);
                sheet.Cells[setRow + 1, col + i].Value = lessValue;

                var aa = bays[i].dic.Where(e => Convert.ToDouble(e.Value) >= Convert.ToDouble(value) && Convert.ToDouble(e.Value) != 0).ToList();
                var bb = bays[i].dic.Where(e => Convert.ToDouble(e.Value) <= Convert.ToDouble(value) && Convert.ToDouble(e.Value) != 0).ToList();
                if (lists.Count == 0)
                {
                    aa.ForEach(e => lists.Add(e.Key));
                }
                else
                {
                    List<string> allKeys = (from kvp in aa select kvp.Key).Distinct().ToList();
                    lists.Where(e => !allKeys.Contains(e)).ToList().ForEach(e => lists.Remove(e));
                }
                if (lessThanLists.Count == 0)
                {
                    bb.ForEach(e => lessThanLists.Add(e.Key));
                }
                else
                {
                    List<string> allKeys = (from kvp in bb select kvp.Key).Distinct().ToList();
                    lessThanLists.Where(e => !allKeys.Contains(e)).ToList().ForEach(e => lessThanLists.Remove(e));
                }
            }
            lists.ForEach(e => s += e + "/");
            sheet.Cells[setRow, col + bays.Count].Value = lists.Count + "套";
            sheet.Cells[setRow, col + bays.Count + 1].Value = s;

            lessThanLists.Where(e => lists.Contains(e)).ToList().ForEach(e => lessThanLists.Remove(e));
            lessThanLists.ForEach(e => less += e + "/");
            sheet.Cells[setRow + 1, col + bays.Count].Value = lessThanLists.Count + "套";
            sheet.Cells[setRow + 1, col + bays.Count + 1].Value = less;
            try
            {
                if (col + bays.Count + 1 < colEnd)
                {
                    sheet.Cells[setRow, col + bays.Count + 1, setRow, colEnd].Merge = true;
                    sheet.Cells[setRow + 1, col + bays.Count + 1, setRow + 1, colEnd].Merge = true;
                }
            }
            catch { }
        }



        /// <summary>
        /// 开间取到设计值时
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="bay"></param>
        /// <param name="currRow"></param>
        /// <param name="design"></param>
        public void BayIsDesignValue(ExcelWorksheet sheet, Bay bay, int currRow, int design)
        {
            if (!string.IsNullOrEmpty(GetMergeValue(sheet, currRow, bay.Index)))
            {
                foreach (var item in bay.children)
                {
                    sheet.Cells[currRow, item].Value = sheet.Cells[design, item].Value;
                }
            }
        }


        /// <summary>
        /// 开间取到最小值时
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="bay"></param>
        /// <param name="currRow"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="design"></param>
        /// <param name="cols"></param>
        public void BayIsMinValue(ExcelWorksheet sheet, Bay bay, int currRow, int min, int max, int design, int cols)
        {
            if (!string.IsNullOrEmpty(GetMergeValue(sheet, currRow, bay.Index)))
            {
                int leftIndex = 0;
                double value = 0;
                if (sheet.Cells[max, bay.children[0]].Value.ToString().Trim() == sheet.Cells[min, bay.children[0]].Value.ToString().Trim())
                {
                    leftIndex = 1;
                }
                for (int i = 0; i < bay.children.Count; i++)
                {
                    int num = bay.children[i];
                    if (i != leftIndex)
                    {
                        sheet.Cells[currRow, num].Value = sheet.Cells[design, num].Value;
                        if (sheet.Cells[max, num].Value.ToString().Trim() == sheet.Cells[min, num].Value.ToString().Trim())
                        {
                            sheet.Cells[currRow, num].Value = sheet.Cells[max, num].Value;
                        }
                        value += GetDoubleValue(sheet.Cells[currRow, num].Value);
                    }
                }
                string text = (GetDoubleValue(sheet.Cells[min, bay.Index].Value) - value).ToString();
                sheet.Cells[currRow, bay.children[leftIndex]].Value = text;
                bool same = false;
                for (int i = 2; i < cols; i++)
                {
                    if (GetMergeValue(sheet, i, bay.children[leftIndex]).ToString().Trim() == text.Trim())
                    {
                        same = true;
                        break;
                    }
                }
                if (!same)
                    sheet.Cells[currRow, bay.children[leftIndex]].Style.Font.Color.SetColor(Color.Red);

            }

        }




        public static void InsertRowAndValue(ExcelWorksheet sheet, int start, int count, string value)
        {
            sheet.InsertRow(start, count);
            sheet.Cells[start, 1].Value = value;
        }

        public static string GetStringValue(object oj)
        {
            string value = null;
            try
            {
                value = oj.ToString();
            }
            catch { }
            return value;

        }

        public double GetDoubleValue(object oj)
        {
            double value = 0;
            try
            {
                value = Convert.ToDouble(oj.ToString());
            }
            catch { }
            return value;
        }


        public static string GetMergeValue(ExcelWorksheet worksheet, int row, int col)
        {
            string value = "";
            try
            {
                if (worksheet.Cells[row, col].Merge)
                {
                    var aa = worksheet.Cells[worksheet.MergedCells[row, col]].First().Value;
                    if (aa == null) return "";
                    value = aa.ToString().Trim();
                }
                else
                {
                    value = worksheet.GetValue<string>(row, col);
                }
            }
            catch
            {
            }

            return value == null ? "" : value;
        }

        public static void GetFieldRowIndex(ExcelWorksheet sheet, int rowStart, int rows, ref int rowEnd, ref int design, ref int minCavity,
            ref int max, ref int min, ref int differ, ref int designDiffer, ref int finalValue)
        {
            #region 确定读到哪一行,并判断是否存在设计值等，如果没有，则插入
            for (int i = rowStart; i <= rows; i++)
            {
                if (i == rows - 1)
                {
                    if (design == -1)
                    {
                        InsertRowAndValue(sheet, rowEnd + 2, 1, "设计值");
                        design = rowEnd + 2;
                    }
                    if (minCavity == -1)
                    {
                        InsertRowAndValue(sheet, design + 1, 1, "产品安装最小空腔值");
                        minCavity = design + 1;
                    }
                    if (max == -1)
                    {
                        InsertRowAndValue(sheet, minCavity + 1, 1, "最大值");
                        max = minCavity + 1;
                    }
                    if (min == -1)
                    {
                        InsertRowAndValue(sheet, max + 1, 1, "最小值");
                        min = max + 1;
                    }
                    if (differ == -1)
                    {
                        InsertRowAndValue(sheet, min + 1, 1, "差值");
                        differ = min + 1;
                    }
                    if (designDiffer == -1)
                    {
                        InsertRowAndValue(sheet, differ + 1, 1, "设计差值");
                        designDiffer = differ + 1;
                    }
                    if (finalValue == -1)
                    {
                        //最终取值与设计差值间空一行
                        InsertRowAndValue(sheet, designDiffer + 1, 2, "最终取值");
                        sheet.Cells[designDiffer + 1, 1, designDiffer + 2, 1].Merge = true;
                        finalValue = designDiffer + 1;
                    }
                    break;
                }

                string value = GetMergeValue(sheet, i, 1);

                if (string.IsNullOrEmpty(value))
                {
                    if (rowEnd == -1) rowEnd = i - 1;
                    continue;
                }
                if (value.Equals("设计值") && rowEnd == -1)
                {
                    rowEnd = i - 1;
                }

                if (value.Equals("设计值"))
                    design = i;
                if (value.Contains("最小空腔值"))
                    minCavity = i;
                if (value.Equals("最大值"))
                    max = i;
                if (value.Equals("最小值"))
                    min = i;
                if (value.Equals("差值"))
                    differ = i;
                if (value.Equals("设计差值"))
                    designDiffer = i;
                if (value.Equals("最终取值"))
                    finalValue = i;
            }

            //可能会存在插入行的情况，所有重新读取
            for (int i = rowEnd; i <= sheet.Dimension.End.Row; i++)
            {
                string value = GetStringValue(sheet.Cells[i, 1].Value);
                if (string.IsNullOrEmpty(value)) continue;
                if (value.Equals("设计值"))
                    design = i;
                if (value.Contains("最小空腔值"))
                    minCavity = i;
                if (value.Equals("最大值"))
                    max = i;
                if (value.Equals("最小值"))
                    min = i;
                if (value.Equals("差值"))
                    differ = i;
                if (value.Equals("设计差值"))
                    designDiffer = i;
                if (value.Equals("最终取值"))
                    finalValue = i;
            }
            #endregion

        }

    }
}
