using OfficeOpenXml;

namespace YaSha.DataManager.ListProcessing.SawingSheet.SheetModel
{
    internal class NestSortSheetData
    {
        internal string 优先级 { get; set; }

        internal string 类型 { get; set; }

        internal string 字段 { get; set; }

        internal string 规则 { get; set; }

        internal string 内容 { get; set; }

        internal string 多条件 { get; set; }

        internal string 排序规则 { get; set; }

        internal static List<NestSortSheetData> Get(string path)
        {
            List<NestSortSheetData> datas = new List<NestSortSheetData>();
            using (ExcelPackage package = new ExcelPackage(path))
            {
                var worksheet = package.Workbook.Worksheets["开料单-排序设置"];
                int rows = worksheet.Dimension.End.Row;
                int cols = worksheet.Dimension.End.Column;

                int idcell = 0;
                int typecell = 0;
                int feildcell = 0;
                int rulecell = 0;
                int conterntcell = 0;
                int mutilcell = 0;
                int sortcell = 0;

                for (int cell = 1; cell <= cols; cell++)
                {
                    var text = worksheet.Cells[1, cell].Text;
                    if (string.IsNullOrEmpty(text))
                    {
                        break;
                    }
                    switch (text)
                    {
                        case "优先级":
                            idcell = cell; break;
                        case "类型":
                            typecell = cell; break;
                        case "字段":
                            feildcell = cell; break;
                        case "规则":
                            rulecell = cell; break;
                        case "内容":
                            conterntcell = cell; break;
                        case "多条件":
                            mutilcell = cell; break;
                        case "排序规则":
                            sortcell = cell; break;
                        default: break;
                    }
                }

                for (int row = 2; row <= rows; row++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[row, 1].Text)) break;
                    var tmp = new NestSortSheetData()
                    {
                        优先级 = worksheet.Cells[row, idcell].Text,
                        类型 = worksheet.Cells[row, typecell].Text,
                        字段 = worksheet.Cells[row, feildcell].Text,
                        规则 = worksheet.Cells[row, rulecell].Text,
                        内容 = worksheet.Cells[row, conterntcell].Text,
                        多条件 = worksheet.Cells[row, mutilcell].Text,
                        排序规则 = worksheet.Cells[row, sortcell].Text,
                    };
                    datas.Add(tmp);
                }
            }
            return datas;
        }

        public override string ToString()
        {
            return $"{this.优先级} {this.类型}";
        }
    }
}
