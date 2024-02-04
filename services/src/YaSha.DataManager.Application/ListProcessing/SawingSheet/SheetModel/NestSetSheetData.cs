using OfficeOpenXml;

namespace YaSha.DataManager.ListProcessing.SawingSheet.SheetModel
{
    internal class NestSetSheetData
    {
        internal string 序号 { get; set; }

        internal string 类型1 { get; set; }

        internal string 类型2 { get; set; }

        internal string 算法 { get; set; }

        internal string 内容1 { get; set; }

        internal string 多条件 { get; set; }

        internal string 标题 { get; set; }

        internal string 规则 { get; set; }

        internal string 内容2 { get; set; }

        internal static List<NestSetSheetData> Get(string path)
        {
            List<NestSetSheetData> datas = new List<NestSetSheetData>();
            using (ExcelPackage package = new ExcelPackage(path))
            {
                var worksheet = package.Workbook.Worksheets["套料设置"];
                int rows = worksheet.Dimension.End.Row;
                int cols = worksheet.Dimension.End.Column;

                int idcell = 0;
                int type1cell = 0;
                int type2cell = 0;
                int algocell = 0;
                int content1 = 0;
                int mutilcell = 0;
                int headcell = 0;
                int rulecell = 0;
                int content2 = 0;

                for (int cell = 1; cell <= cols; cell++)
                {
                    var text = worksheet.Cells[1, cell].Text;
                    if (string.IsNullOrEmpty(text))
                    {
                        break;
                    }
                    switch (text)
                    {
                        case "序号":
                            idcell = cell; break;
                        case "类型1":
                            type1cell = cell; break;
                        case "类型2":
                            type2cell = cell; break;
                        case "算法":
                            algocell = cell; break;
                        case "内容1":
                            content1 = cell; break;
                        case "多条件":
                            mutilcell = cell; break;
                        case "标题":
                            headcell = cell; break;
                        case "规则":
                            rulecell = cell; break;
                        case "内容2":
                            content2 = cell; break;
                        default: break;
                    }
                }

                for (int row = 2; row <= rows; row++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[row, 1].Text)) break;
                    var tmp = new NestSetSheetData()
                    {
                        序号 = worksheet.Cells[row, idcell].Text,
                        类型1 = worksheet.Cells[row, type1cell].Text,
                        类型2 = worksheet.Cells[row, type2cell].Text,
                        算法 = worksheet.Cells[row, algocell].Text,
                        内容1 = worksheet.Cells[row, content1].Text,
                        多条件 = worksheet.Cells[row, mutilcell].Text,
                        标题 = worksheet.Cells[row, headcell].Text,
                        规则 = worksheet.Cells[row, rulecell].Text,
                        内容2 = worksheet.Cells[row, content2].Text,
                    };
                    datas.Add(tmp);
                }
            }
            return datas;
        }

        internal bool Compare(string target, string compare)
        {
            bool next = false;
            double d1 = 0;
            double d2 = 0;
            double.TryParse(target, out d1);
            double.TryParse(compare, out d2);
            switch (this.算法)
            {
                case "等于": next = target.Equals(compare); break;
                case "不等于": next = !target.Equals(compare); break;
                case "包含": next = target.Contains(compare); break;
                case "不包含": next = !target.Contains(compare); break;
                case "大于": next = d1 > d2; break;
                case "小于": next = d1 < d2; break;
                case "大于等于": next = d1 >= d2; break;
                case "小于等于": next = d1 <= d2; break;
            }
            return next;
        }

        public override string ToString()
        {
            return $"{this.序号}";
        }
    }
}
