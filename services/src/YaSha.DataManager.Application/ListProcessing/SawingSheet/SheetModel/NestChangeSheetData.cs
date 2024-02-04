using OfficeOpenXml;

namespace YaSha.DataManager.ListProcessing.SawingSheet.SheetModel
{
    internal class NestChangeSheetData
    {
        internal string 序号 { get; set; }

        internal string 类型1 { get; set; }

        internal string 字段 { get; set; }

        internal string 算法 { get; set; }

        internal string 类型2 { get; set; }

        internal string 值 { get; set; }

        internal string 多条件 { get; set; }

        internal string 赋值字段 { get; set; }

        internal string 赋值类型 { get; set; }

        internal string 赋值 { get; set; }

        internal static List<NestChangeSheetData> Get(string path)
        {
            List<NestChangeSheetData> datas = new List<NestChangeSheetData>();

            using (ExcelPackage package = new ExcelPackage(path))
            {
                var worksheet = package.Workbook.Worksheets["开料单-修改设置"];
                int rows = worksheet.Dimension.End.Row;
                int cols = worksheet.Dimension.End.Column;

                int idcell = 0;
                int typecell = 0;
                int feildcell = 0;
                int algrocell = 0;
                int type2cell = 0;
                int valuecell = 0;
                int mulcell = 0;
                int setvaluefeildcell = 0;
                int setvaluetypecell = 0;
                int setvaluecell = 0;
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
                            typecell = cell; break;
                        case "字段":
                            feildcell = cell; break;
                        case "算法":
                            algrocell = cell; break;
                        case "类型2":
                            type2cell = cell; break;
                        case "值":
                            valuecell = cell; break;
                        case "多条件":
                            mulcell = cell; break;
                        case "赋值字段":
                            setvaluefeildcell = cell; break;
                        case "赋值类型":
                            setvaluetypecell = cell; break;
                        case "赋值":
                            setvaluecell = cell; break;
                        default: break;
                    }
                }

                for (int row = 2; row <= rows; row++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[row, 1].Text)) break;
                    var tmp = new NestChangeSheetData()
                    {
                        序号 = worksheet.Cells[row, idcell].Text,
                        类型1 = worksheet.Cells[row, typecell].Text,
                        字段 = worksheet.Cells[row, feildcell].Text,
                        算法 = worksheet.Cells[row, algrocell].Text,
                        类型2 = worksheet.Cells[row, type2cell].Text,
                        值 = worksheet.Cells[row, valuecell].Text,
                        多条件 = worksheet.Cells[row, mulcell].Text,
                        赋值字段 = worksheet.Cells[row, setvaluefeildcell].Text,
                        赋值类型 = worksheet.Cells[row, setvaluetypecell].Text,
                        赋值 = worksheet.Cells[row, setvaluecell].Text,
                    };
                    datas.Add(tmp);
                }
            }
            return datas;
        }

        public override string ToString()
        {
            return $"{this.序号}";
        }
    }
}
