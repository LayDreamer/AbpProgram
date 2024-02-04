using OfficeOpenXml;
using System.Data;
using System.ComponentModel;
using System.Reflection;

namespace YaSha.DataManager.SaleOderList.Classes
{
    public class ExcelHelper
    {
        public static string ProjectName;
        public static string SeriesName;

        public ExcelHelper()
        {
            //指定EPPlus使用非商业化许可证
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            SeriesName = null;
        }








        public static DataTable filePathToDb(string filePath, string sheetName, int rowStart, ref string errorMessage)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            //指定EPPlus使用非商业化许可证
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet worksheet = null;
                foreach (var item in package.Workbook.Worksheets)
                {
                    if (sheetName == "包装")
                    {
                        if (item.Name.Contains(sheetName))
                        {
                            worksheet = item;
                            break;
                        }
                    }
                    else
                    {
                        if (item.Name.Equals(sheetName))
                        {
                            worksheet = item;
                            break;
                        }
                    }
                }
                if (worksheet == null)
                {
                    errorMessage += Path.GetFileName(filePath) + "中缺少表名为【" + sheetName + "】的表\r\n";
                    return new DataTable();
                }
                //从指定行开始读取数据，行数据为空结束
                DataTable dt = Common.WorksheetToTable(worksheet, worksheet.Dimension.End.Column, rowStart);
                return dt;
            }
        }



        public static bool AddFinishedCodeToExcel(DataTable sourceDt, List<string> list, string filePath, string sheetName, int rowCount, ref string errorMessage)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            //指定EPPlus使用非商业化许可证
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet worksheet = null;
                foreach (var item in package.Workbook.Worksheets)
                {
                    if (sheetName == "包装")
                    {
                        if (item.Name.Contains(sheetName))
                        {
                            worksheet = item;
                            break;
                        }
                    }
                    else
                    {
                        if (item.Name.Equals(sheetName))
                        {
                            worksheet = item;
                            break;
                        }
                    }
                }
                if (worksheet == null) return false;
                //从指定行开始读取数据，行数据为空结束
                DataTable dt = Common.WorksheetToTable(worksheet, worksheet.Dimension.End.Column, rowCount);
                string name = "产成品编码";
                int dtIndex = dt.Columns.IndexOf(name);
                int addColumnInSheet = 1;

                if (dtIndex == -1)
                {
                    try
                    {
                        worksheet.InsertColumn(addColumnInSheet, 1);
                        worksheet.Cells[rowCount - 1, addColumnInSheet].Value = name;
                    }
                    catch
                    {
                        errorMessage += "无法向拆单表中插入一列 \r\n";
                        return false;
                    }
                }
                else
                {
                    addColumnInSheet = dtIndex + 1;
                }

                #region  判断拆单表中数据
                foreach (var item in list)
                {
                    int count = 0;
                    bool hasValue = false;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string s = dt.Rows[i][item].ToString().Trim();
                        if (string.IsNullOrEmpty(s))
                        {
                            if (count > 10) break;
                            count++;
                        }
                        else
                        {
                            hasValue = true;
                            break;
                        }
                    }
                    if (!hasValue)
                    {
                        errorMessage += $"拆单表中{item}字段值为空，无法匹配数据 \r\n";
                    }
                }
                #endregion

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow row = dt.Rows[i];
                    for (int j = 0; j < sourceDt.Rows.Count; j++)
                    {
                        bool equal = true;
                        foreach (var item in list)
                        {
                            try
                            {
                                if ((!dt.Columns.Contains(item)) || (!sourceDt.Columns.Contains(item))) continue;
                                string s1 = row[item].ToString().Trim();
                                string s2 = sourceDt.Rows[j][item].ToString().Trim();
                                if (!s1.Equals(s2))
                                {
                                    equal = false;
                                    break;
                                }
                            }
                            catch
                            {
                                equal = false;
                            }
                        }
                        if (equal)
                        {
                            //得到单元格
                            ExcelRangeBase rangeBase = worksheet.Cells[rowCount + i, addColumnInSheet];
                            rangeBase.Value = sourceDt.Rows[j][name].ToString();
                            //rangeBase.Style.Font.Color.SetColor(Color.Red);
                            break;
                        }
                    }
                }
                package.Save();
            }
            return true;
        }


        /// <summary>
        /// 读取excel文件中的信息
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public List<T> ReadInfoFromExcel<T>(string fileName, string sheetName, int rowStart, int colEnd, int colStart, ref string errorMessage) where T : new()
        {
            errorMessage = string.Empty;
            List<T> datas = new();
            try
            {
                FileInfo fileInfo = new FileInfo(fileName);
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                using (ExcelPackage Excel = new ExcelPackage(fileInfo))//打开表
                {
                    var worksheets = Excel.Workbook.Worksheets;
                    ExcelWorksheet sheet = Excel.Workbook.Worksheets[0];
                    if (!string.IsNullOrEmpty(sheetName))
                    {
                        sheet = worksheets.FirstOrDefault(e => e.Name.Contains(sheetName));
                        if (sheet == null)
                        {
                            errorMessage = $"未找到指定sheet表：{sheetName}！";
                        }
                    }

                    ///-1时默认读取表单的
                    if (colEnd < 0)
                    {
                        colEnd = sheet.Dimension.End.Column;
                    }

                    if (sheet.Cells[3, 49].Value != null)
                    {
                        ProjectName = sheet.Cells[3, 49].Value.ToString();
                    }
                    if (sheet.Cells[4, 39].Value != null)
                    {
                        SeriesName = sheet.Cells[4, 39].Value.ToString();
                    }

                    ///从指定行开始读取数据，行数据为空结束
                    DataTable dataTable = Common.WorksheetToTable(sheet, colEnd, rowStart, colStart);
                    datas = dataTable.ToModelList<T>();
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            return datas;
        }


        /// <summary>
        /// 导出Excel文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath">文件存储地址</param>
        /// <param name="tList">数据源</param>
        /// <param name="titleRow">列名起始行</param>
        /// <param name="dataRow">数据起始行</param>
        /// <param name="titleCol">列名起始列</param>
        /// <returns></returns>
        public async Task OutPutExcel<T>(string filePath, string targetFilePath, List<T> tList, bool createCol, int titleRow = 1, int dataRow = 2, int titleCol = 1) where T : new()
        {
            string resDir = Path.GetDirectoryName(targetFilePath);
            if (!Directory.Exists(resDir))
            {
                Directory.CreateDirectory(resDir);
            }
            FileInfo fileInfo = new FileInfo(filePath);
            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                //工作簿
                //ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                //实体属性
                PropertyInfo[] properties = GetProperties(new T());
                //填充表头
                //for (int i = 1; i < properties.Length + 1; i++)

                if (createCol)
                {
                    for (int i = titleCol; i < properties.Length + titleCol; i++)
                    {
                        //worksheet.Cells[1, i].Value = properties[i - 1].Name;
                        string des = properties[i - titleCol] == null ? null : ((DescriptionAttribute)Attribute.GetCustomAttribute(properties[i - titleCol], typeof(DescriptionAttribute)))?.Description;
                        worksheet.Cells[titleRow, i].Value = des;
                        //worksheet.Cells[titleRow, i].Style.Font.Size = 18;
                    }
                }

                //填充行(从第二行开始)
                //for (int i = 2; i < tList.Count + 2; i++)
                for (int i = dataRow; i < tList.Count + dataRow; i++)
                {
                    //填充行内列
                    //for (int j = 1; j < properties.Length + 1; j++)
                    for (int j = titleCol; j < properties.Length + titleCol; j++)
                    {
                        var property = properties[j - titleCol].Name;
                        var des = properties[j - titleCol] == null ? null : ((DescriptionAttribute)Attribute.GetCustomAttribute(properties[j - titleCol], typeof(DescriptionAttribute)))?.Description;
                        if (des == null)
                            continue;
                        worksheet.Cells[i, j].Value = GetPropertyValue(tList[i - dataRow], property);

                    }
                    worksheet.Cells.Style.ShrinkToFit = true;//单元格自动适应大小
                    worksheet.Row(i).Height = 20;//设置行高
                    worksheet.Row(i).CustomHeight = true;//自动调整行高
                }

                foreach (var item in worksheet.Cells)
                {
                    item.Style.Font.Size = 18;
                }

                //列宽自适应
                worksheet.Cells.AutoFitColumns();
                using FileStream fileStream = new(targetFilePath, FileMode.Create, FileAccess.Write);

                //保存
                await package.SaveAsAsync(fileStream);

            }
        }

        /// <summary>
        /// 获取类的全部属性
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private static PropertyInfo[] GetProperties<T>(T t) where T : new()
        {
            PropertyInfo[] properties = t.GetType().GetProperties();
            return properties;
        }
        /// <summary>
        /// 获取类的属性值
        /// </summary>
        /// <param name="obj">类</param>
        /// <param name="property">属性</param>
        /// <returns></returns>
        private static object GetPropertyValue(object obj, string property)
        {
            return obj.GetType().GetProperty(property).GetValue(obj);
        }



    }



}
