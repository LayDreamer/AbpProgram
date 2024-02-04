using System.Data;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using YaSha.DataManager.ListProcessing.Base;

namespace YaSha.DataManager.ListProcessing.AssembleSheet
{
    public class BuildAssembleSheet : ListProcessingBuildBase
    {
        string dt;

        string ExcelName;

        internal override string TableName { get => "装配单"; set => TableName = value; }

        public BuildAssembleSheet(BuildFactory factory) : base(factory)
        {
            order = 1;
            dt = factory.dt;
        }

        internal override ExcelWorksheet ProcessExcel(ExcelWorksheets sheets)
        {
            var FilterExcel = WorksheetToTable(rulePath, "装配单-过滤设置", 0, 0);//装配单-过滤设置
            var SortExcel = WorksheetToTable(rulePath, "装配单-排序设置", 0, 0);  //装配单-排序设置
            var EditExcel = WorksheetToTable(rulePath, "装配单-修改设置", 0, 0); //装配单-修改设置

            int BeginColumn_Assembly = config.SheetStartRow;   //装配单——列名所在行数

            System.Data.DataTable SourceDataExcel = null;   //源表单（即清单）
            System.Data.DataTable SourceDataExcel_copy = null;   //源表单（即清单）
            try
            {
                int BeginColumn_Source = config.Origin.SheetStartRow; //内加工清单——列名所在行数

                var originSheetName = config?.Origin?.SheetName;

                var worksheet = sheets[originSheetName];

                if (null == worksheet)
                {
                    if (originSheetName == "内加工清单")
                    {
                        worksheet = sheets[0];

                        if (worksheet == null)
                        {
                            throw new Exception($"生成{config.SheetName}失败 源文件中没有{originSheetName}");
                        }
                    }
                    else
                    {
                        throw new Exception($"生成{config.SheetName}失败 源文件中没有{originSheetName}");
                    }
                }
                #region[获取清单标题]
                ExcelName = sheetName.Replace("内加工清单", "") + $"——装配及品检合计[{dt}]套";
                #endregion

                SourceDataExcel = WorksheetToTable(worksheet, BeginColumn_Source - 1, 0, false);
                SourceDataExcel_copy = WorksheetToTable(worksheet, BeginColumn_Source - 1, 0, false); ;
                SourceDataExcel = DelEmptyData(SourceDataExcel);

            }
            catch (Exception ex)
            {
                throw new Exception("打开源表单失败，原因：" + ex.Message);
            }

            #region[dt添加“批量”列（空），添加“非标码”列（赋值）]

            SourceDataExcel.Columns.Add("非标码", typeof(string));
            foreach (DataRow dr in SourceDataExcel.Rows)
            {
                string str = dr["模块编码"].ToString();
                if (str.Contains("-"))
                {
                    string[] sArray = str.Split('-');
                    dr["非标码"] = sArray[1];
                }
                else
                {
                    dr["非标码"] = "/";
                }
            }
            #endregion

            #region[通过《装配单-过滤设置》过滤源数据]
            int count = SourceDataExcel.Rows.Count;
            for (int i = 0; i < count; i++)
            {
                if (SourceDataExcel.Rows[0].ToString() == null || SourceDataExcel.Rows[0].ToString() == "")
                {
                    break;
                }
                DataRow dr = SourceDataExcel.Rows[i];
                int result_Filter = 0;
                try
                {
                    result_Filter = Assembly.FilterWork(dr, FilterExcel);  //0 => 报错；1 => 不符合条件； 2 => 符合条件
                }
                catch (Exception ex)
                {
                    throw new Exception("过滤源数据失败，原因：" + ex.Message);
                }
                if (result_Filter == 0)
                {
                    throw new Exception("过滤源数据失败");
                }
                else if (result_Filter == 1)
                {
                    SourceDataExcel.Rows.Remove(dr);
                    i = i - 1;
                    count = count - 1;
                }
                else if (result_Filter == 2)
                {
                    continue;
                }

            }
            #endregion

            if (SourceDataExcel.Rows.Count > 0)
            {
                #region[排序]
                SourceDataExcel = SortData.SortData_Assembly(SourceDataExcel, SortExcel);
                #endregion

                try
                {
                    //（根据修改规则）修改数据
                    SourceDataExcel = Assembly.EditWork(SourceDataExcel, EditExcel, int.Parse(dt), SourceDataExcel_copy);

                    #region[处理修改表中合并数据条件]
                    SourceDataExcel = MergeData.MergeExcelData(SourceDataExcel, EditExcel);
                    #endregion
                }
                catch (Exception ex)
                {
                    throw new Exception("修改源数据失败，原因：" + ex.Message);
                }

                bool isSucceed_Assembly = WriteExcel_Assembly(SourceDataExcel, config.Sheet, BeginColumn_Assembly, EditExcel, ExcelName, int.Parse(dt));  //数据写入工作表——装配单
            }
            else
            {
                throw new Exception("通过《装配单-过滤设置》过滤源数据后无符合条件数据。");
            }


            return config.Sheet;


        }

        /// <summary>
        /// “装配单”工作表数据填充
        /// </summary>
        /// <param name="sources" 源数据></param>  
        /// <param name="worksheet" “装配单”工作表></param>
        /// <param name="columnBegin" 列标题所在行数></param>
        /// <param name="dt_edit" “修改规则”（处理全局条件）></param>
        /// <param name="excelName"  清单文件名></param>
        /// <param name="num"  批量数></param>
        /// <returns></returns>
        public static bool WriteExcel_Assembly(DataTable sources, ExcelWorksheet worksheet, int columnBegin, DataTable dt_edit, string excelName, int num)
        {
            bool isSucceed = false;
            DataTable dt_Assembly = WorksheetToTable(worksheet, columnBegin - 1, 0, false);
            for (int c = 0; c < sources.Columns.Count; c++)
            {
                string columnName = sources.Columns[c].ColumnName;  //列标题
                int i = GetColumnNo(dt_Assembly, columnName);    //该标题(列)在工作表中的序号
                if (i != 0)
                {
                    for (int r = 0; r < sources.Rows.Count; r++)
                    {
                        ExcelRangeBase rangeBase = worksheet.Cells[r + columnBegin + 1, i + 1];
                        string str = sources.Rows[r][c].ToString();
                        double dou = 0;
                        bool isDouble = false;
                        try
                        {
                            dou = Convert.ToDouble(str);
                            rangeBase.Value = dou;
                            isDouble = true;
                        }
                        catch
                        { }
                        if (isDouble == false)
                        {
                            rangeBase.Value = sources.Rows[r][c].ToString();
                        }

                    }
                }
            }

            #region[序号赋值]
            for (int r = 0; r < sources.Rows.Count; r++)
            {
                ExcelRangeBase rangeBase = worksheet.Cells[r + columnBegin + 1, 1];
                rangeBase.Value = r + 1;
            }
            #endregion

            #region[修改规则中，“全局”条件处理]
            foreach (DataRow dr in dt_edit.Rows)
            {
                if (dr["类型1"].ToString() == "全局")
                {
                    string judgeField = dr["字段"].ToString();
                    string field = dr["赋值字段"].ToString();
                    string category = dr["赋值类型"].ToString();
                    string value = dr["赋值"].ToString();
                    int k = GetColumnNo(dt_Assembly, judgeField);
                    int i = GetColumnNo(dt_Assembly, field);    //赋值字段(列)在工作表中的序号
                    for (int r = 0; r < sources.Rows.Count; r++)
                    {
                        string tset = worksheet.Cells[r + columnBegin + 1, k + 1].Value.ToString();
                        bool isEqual = Assembly.JudgeIsEligible(worksheet.Cells[r + columnBegin + 1, k + 1].Value.ToString(), dr);
                        if (isEqual == false)
                        {
                            continue;
                        }
                        ExcelRangeBase rangeBase = worksheet.Cells[r + columnBegin + 1, i + 1];
                        if (category == "公式")
                        {
                            //rangeBase.Formula = value;
                            double workResult = 0;
                            string[] sArray = value.Split(' ');
                            int worknum = 0;
                            for (int j = 0; j < sArray.Count(); j++)
                            {
                                if (sArray[j] == null || sArray[j] == "")
                                {
                                    break;
                                }
                                if (sArray[j] == "+" || sArray[j] == "-" || sArray[j] == "*" || sArray[j] == "/")
                                {
                                    #region[计算符号后一位的数值]
                                    double after = 0;
                                    if (sArray[j + 1] == "批量数")
                                    {
                                        after = num;
                                    }
                                    else
                                    {
                                        int clo = GetColumnNo(dt_Assembly, DelBrackets(sArray[j + 1]));
                                        after = Convert.ToDouble(worksheet.Cells[r + columnBegin + 1, clo + 1].Value.ToString());
                                    }
                                    #endregion
                                    if (worknum == 0)
                                    {
                                        #region[计算符号前一位的数值]
                                        double before = 0;
                                        if (sArray[j - 1] == "批量数")
                                        {
                                            before = num;
                                        }
                                        else
                                        {
                                            int clo = GetColumnNo(dt_Assembly, DelBrackets(sArray[j - 1]));
                                            before = Convert.ToDouble(worksheet.Cells[r + columnBegin + 1, clo + 1].Value.ToString());
                                        }
                                        #endregion

                                        if (sArray[j] == "+")
                                        {
                                            workResult = before + after;
                                        }
                                        else if (sArray[j] == "-")
                                        {
                                            workResult = before - after;
                                        }
                                        else if (sArray[j] == "*")
                                        {
                                            workResult = before * after;
                                        }
                                        else if (sArray[j] == "/")
                                        {
                                            workResult = before / after;
                                        }
                                    }
                                    else
                                    {
                                        if (sArray[j] == "+")
                                        {
                                            workResult = workResult + after;
                                        }
                                        else if (sArray[j] == "-")
                                        {
                                            workResult = workResult - after;
                                        }
                                        else if (sArray[j] == "*")
                                        {
                                            workResult = workResult * after;
                                        }
                                        else if (sArray[j] == "/")
                                        {
                                            workResult = workResult / after;
                                        }
                                    }
                                    worknum++;
                                }
                            }
                            rangeBase.Value = workResult;
                        }
                        else if (category == "值")
                        {
                            rangeBase.Value = value;
                        }
                        else if (category == "值附加")
                        {
                            string str = null;
                            try
                            {
                                str = rangeBase.Value.ToString();
                            }
                            catch
                            { }
                            rangeBase.Value = str + value;
                        }
                        else if (category == "值前缀附加")
                        {
                            string str = null;
                            try
                            {
                                str = rangeBase.Value?.ToString();
                            }
                            catch
                            { }
                            rangeBase.Value = value + str;
                        }
                        else if (category == "向上取整")
                        {
                            double count = Convert.ToDouble(rangeBase.Value);
                            rangeBase.Value = Math.Ceiling(count);
                            sources.Rows[r][field] = Math.Ceiling(count).ToString();
                        }
                        else if (category == "向下取整")
                        {
                            double count = Convert.ToDouble(rangeBase.Value);
                            rangeBase.Value = Math.Floor(count);
                            sources.Rows[r][field] = Math.Ceiling(count).ToString();
                        }

                    }
                }
            }
            #endregion

            #region[图号相同合并单元格]
            DataTable dt_finally = WorksheetToTable(worksheet, columnBegin - 1, 0, false);  //已填充的工作表数据
            int drawCode_num = GetColumnNo(dt_Assembly, "图号") + 1;  //从1开始
            int total_num = GetColumnNo(dt_Assembly, "合计") + 1;
            List<TotalData> totalDatas = new List<TotalData>();   //需要合并单元格的数据集合
            IEnumerable<IGrouping<string, DataRow>> result = dt_finally.Rows.Cast<DataRow>().GroupBy<DataRow, string>(dr => dr["图号"].ToString());
            foreach (IGrouping<string, DataRow> res in result)
            {
                List<int> rowsNo = new List<int>();
                //List<string> cellsValue = new List<string>();
                int count = 0;   //“单套”数据合计
                foreach (DataRow item in res)
                {
                    if (item["序号"].ToString() == null || item["序号"].ToString() == "")
                    {
                        break;
                    }
                    //if (item["图号"].ToString() != "/" && item["图号"].ToString() != null && item["图号"].ToString() != "")
                    //{
                    rowsNo.Add(Convert.ToInt32(item["序号"].ToString()) + columnBegin + 1);
                    //cellsValue.Add(item["合计"].ToString());
                    string tset = item["单套"].ToString();
                    count = count + Convert.ToInt32(item["单套"].ToString());
                    //}                  
                }
                if (rowsNo.Count > 0)
                {
                    TotalData total = new TotalData();
                    total.ColumnNo = total_num;
                    total.RowsNo = rowsNo;
                    //total.CellsValue = cellsValue;
                    total.Count = count;
                    totalDatas.Add(total);
                }
            }
            if (totalDatas.Count > 0)
            {
                foreach (var data in totalDatas)
                {
                    if (data.RowsNo.Count > 1)
                    {
                        int min = data.RowsNo.Min() - 1;
                        int max = data.RowsNo.Max() - 1;
                        MergeSheetCells(worksheet, min, data.ColumnNo, max, data.ColumnNo);
                        ExcelRangeBase rangeBase = worksheet.Cells[min, data.ColumnNo];
                        rangeBase.Value = "合计：" + data.Count.ToString();
                        //worksheet.Cells[min, data.ColumnNo].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        //worksheet.Cells[min, data.ColumnNo].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    }
                    if (data.RowsNo.Count == 1)
                    {
                        ExcelRangeBase rangeBase = worksheet.Cells[data.RowsNo[0] - 1, data.ColumnNo];
                        rangeBase.Value = "合计：" + data.Count.ToString();
                        //worksheet.Cells[data.RowsNo[0] - 1, data.ColumnNo].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        //worksheet.Cells[data.RowsNo[0] - 1, data.ColumnNo].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    }
                }
            }
            #endregion


            #region[修改标题C1]
            try
            {
                ExcelRangeBase rangeBase = worksheet.Cells[1, 3];
                string str = rangeBase.Value.ToString();
                str = str.Replace("清单文件名去后缀", "");
                str = str.Replace("\"", "");
                string[] sArray = str.Split('+');
                string newName = excelName.Replace(sArray[0], "");
                newName = excelName + sArray[1].Replace("[批量数]", num.ToString());
                rangeBase.Value = newName;
            }
            catch
            { }
            #endregion

            //worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //worksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            #region[单元格居中]
            //for (int c = 0; c < sources.Columns.Count; c++)
            //{
            //    string columnName = sources.Columns[c].ColumnName;  //列标题
            //    int i = GetColumnNo(dt_Assembly, columnName);    //该标题(列)在工作表中的序号
            //    if (i != 0)
            //    {
            //        for (int r = 0; r < sources.Rows.Count; r++)
            //        {
            //            worksheet.Cells[r + columnBegin + 1, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //            worksheet.Cells[r + columnBegin + 1, i + 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            //        }
            //    }
            //}
            worksheet.Columns.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Rows.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            #endregion

            isSucceed = true;
            return isSucceed;
        }

        /// <summary>
        /// 通过名称和文件路径读取数据到DataTable
        /// </summary>
        /// <param name="fullFilePath">fullFilePath</param>
        /// <param name="name">sheetName</param>
        /// <param name="offsetRow">行偏移</param>
        /// <param name="offsetColumn">列偏移</param>
        /// <param name="isEmptyTable">是否为空表</param>
        /// <returns></returns>
        static DataTable WorksheetToTable(string fullFilePath, string name, int offsetRow, int offsetColumn, bool isEmptyTable = false)
        {
            try
            {
                using (var package = new ExcelPackage(new FileInfo(fullFilePath)))
                {
                    int index = GetWorksheetsIndex(package, name);

                    ExcelWorksheet worksheet = package.Workbook.Worksheets[index];//选定指定页

                    return WorksheetToTable(worksheet, offsetRow, offsetColumn, isEmptyTable);
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// 根据名称找索引
        /// </summary>
        /// <param name="package">ExcelPackage</param>
        /// <param name="name">Sheet页名称</param>
        /// <returns>第几页</returns>
        static int GetWorksheetsIndex(ExcelPackage package, string name)
        {
            int index = -1;
            for (int i = 0; i < package.Workbook.Worksheets.Count; i++)
            {

                if (name == package.Workbook.Worksheets[i].Name)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="offsetRow">默认偏移2是为兼容原函数</param>
        /// <param name="offsetColumn">默认偏移1是为兼容原函数</param>
        /// <param name="isEmptyTable">是否返回空表</param>
        /// <returns></returns>
        static DataTable WorksheetToTable(ExcelWorksheet worksheet, int offsetRow, int offsetColumn, bool isEmptyTable)
        {
            DataTable dt = new DataTable(worksheet.Name);
            if (worksheet.Dimension == null) { return dt; }
            //获取worksheet的行数
            int rows = worksheet.Dimension.End.Row;
            //获取worksheet的列数
            int cols = worksheet.Dimension.End.Column;

            DataRow dr = null;

            try
            {
                for (int i = 1 + offsetRow; i <= rows; i++)
                {
                    if (i > 1 + offsetRow)
                    {
                        dr = dt.Rows.Add();
                    }
                    for (int j = offsetColumn + 1; j <= cols; j++)
                    {
                        //默认将第一行设置为datatable的标题
                        if (i == 1 + offsetRow)
                        {
                            if (worksheet.Cells[i, 1 + offsetColumn].Value == null)
                            {
                                continue;
                            }
                            dt.Columns.Add(GetString(worksheet.Cells[i, j].Value));
                        }
                        //剩下的写入datatable 
                        else
                        {
                            try
                            {
                                int ind = j - offsetColumn - 1;
                                if (dr.Table.Columns.Count < ind)
                                {
                                    continue;
                                }
                                dr[ind] = GetString(worksheet.Cells[i, j].Value);
                            }
                            catch (System.Exception)
                            {
                                continue;
                            }
                        }
                    }
                    if (isEmptyTable) { break; }
                }
            }
            catch (Exception ex)
            {

            }

            return dt;
        }

        /// <summary>
        /// 转字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        static string GetString(object obj)
        {
            try
            {
                if (obj == null) { return ""; }
                return obj.ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }

        static DataTable DelEmptyData(DataTable dt)
        {
            int i = 0;
            for (int r = 0; r < dt.Rows.Count; r++)
            {
                DataRow dr = dt.Rows[r];
                if (i == 0)
                {
                    if (dr[0].ToString() == null || dr[0].ToString() == "")
                    {
                        i = r;
                        dt.Rows.Remove(dr);
                    }
                }
                else
                {
                    dt.Rows.Remove(dr);
                }
            }

            return dt;
        }


        private static int GetColumnNo(DataTable dt, string columnName)
        {
            int i = 0;
            for (int c = 0; c < dt.Columns.Count; c++)
            {
                if (dt.Columns[c].ColumnName == columnName)
                {
                    i = c;
                    break;
                }
            }
            return i;
        }

        static string DelBrackets(string value)
        {
            string str = null;
            str = value.Replace("[", "");
            str = str.Replace("]", "");
            return str;
        }


        class TotalData
        {
            public int ColumnNo { get; set; }   //列序号

            public List<int> RowsNo { get; set; }   //需要合并的行序号

            public List<string> CellsValue { get; set; }  //单元格中的内容

            public int Count { get; set; }  //“单套”数量合并
        }


        static void MergeSheetCells(ExcelWorksheet worksheet, int FromRow, int FromColumn, int ToRow, int ToColumn)
        {
            //先清空原有单元格数据
            for (var r = FromRow; r <= ToRow; r++)
            {
                for (var c = FromColumn; c < ToColumn; c++)
                {
                    worksheet.Cells[r, c].Clear();
                }
            }
            worksheet.Cells[FromRow, FromColumn, ToRow, ToColumn].Merge = true;
        }

        public static DataTable CreateNewDataTable(DataTable source)
        {
            DataTable dt = new DataTable();
            for (int c = 0; c < source.Columns.Count; c++)
            {
                dt.Columns.Add(source.Columns[c].ColumnName, typeof(string));
            }
            return dt;
        }
    }
}
