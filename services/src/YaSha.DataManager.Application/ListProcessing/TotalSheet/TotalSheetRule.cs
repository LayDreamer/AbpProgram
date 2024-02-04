using OfficeOpenXml;

namespace YaSha.DataManager.ListProcessing.TotalSheet
{
    /// <summary>
    /// 修改设置
    /// </summary>
    internal class TotalSheetRule
    {
        internal bool first;

        internal string id;

        internal string type1;

        internal string field;

        internal string ai;

        internal string type2;

        internal string value;

        internal string moreValue;

        internal string defaultValue;

        internal string defaultType;

        internal string evaluationValue;

        internal static List<TotalSheetRule> Get(string path)
        {
            List<TotalSheetRule> result = new List<TotalSheetRule>();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage package = new ExcelPackage(path))
            {
                var configSheet = package.Workbook.Worksheets["总单-修改设置"];

                if (null == configSheet)
                {
                    package.Dispose();

                    throw new ArgumentNullException("configSheet is Null");
                }

                int rows = configSheet.Dimension.End.Row;

                int cols = configSheet.Dimension.End.Column;

                int r = 0;

                for (int i = 2; i <= rows; i++)
                {
                    var id = configSheet.Cells[i, 1].Text;

                    if (id == "序号")
                    {
                        r = i + 1;
                        break;
                    }
                    result.Add(new TotalSheetRule
                    {
                        first = true,

                        id = configSheet.Cells[i, 1].Text,

                        type1 = configSheet.Cells[i, 2].Text,

                        field = configSheet.Cells[i, 3].Text,

                        ai = configSheet.Cells[i, 4].Text,

                        type2 = configSheet.Cells[i, 5].Text,

                        value = configSheet.Cells[i, 6].Text,

                        moreValue = configSheet.Cells[i, 7].Text,

                        defaultValue = configSheet.Cells[i, 8].Text,

                        defaultType = configSheet.Cells[i, 9].Text,

                        evaluationValue = configSheet.Cells[i, 10].Text,
                    });
                }

                for (int i = r; i <= rows; i++)
                {
                    var id = configSheet.Cells[i, 2].Text;

                    if (id == "")
                    {
                        break;
                    }
                    result.Add(new TotalSheetRule
                    {
                        id = configSheet.Cells[i, 1].Text,

                        type1 = configSheet.Cells[i, 2].Text,

                        field = configSheet.Cells[i, 3].Text,

                        ai = configSheet.Cells[i, 4].Text,

                        type2 = configSheet.Cells[i, 5].Text,

                        value = configSheet.Cells[i, 6].Text,

                        moreValue = configSheet.Cells[i, 7].Text,

                        defaultValue = configSheet.Cells[i, 8].Text,

                        defaultType = configSheet.Cells[i, 9].Text,

                        evaluationValue = configSheet.Cells[i, 10].Text,
                    });
                }
            }
            return result;
        }

        internal static Dictionary<int, double> GetWidthRule(string path)
        {
            var res = new Dictionary<int, double>();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage package = new ExcelPackage(path))
            {
                var configSheet = package.Workbook.Worksheets["总单-宽度系数设置"];

                if (null == configSheet)
                {
                    package.Dispose();

                    throw new ArgumentNullException("configSheet is Null");
                }

                int rows = configSheet.Dimension.End.Row;

                for (int row = 2; row < rows; row++)
                {
                    var id = configSheet.Cells[row, 1].Text;

                    if (string.IsNullOrEmpty(id)) continue;

                    var t1 = configSheet.Cells[row, 4].Text;

                    var t2 = configSheet.Cells[row, 5].Text;

                    res.Add(int.Parse(t1), double.Parse(t2));
                }
            }

            return res;
        }

        /// <summary>
        /// 排序设置
        /// </summary>
        internal class TotalSheetSortRule
        {

        }
    }
}
