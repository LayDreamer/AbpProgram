using OfficeOpenXml;

namespace YaSha.DataManager.ListProcessing.SawingSheet.SheetModel
{
    [Serializable]
    public class MaterialSheetData
    {
        public string 序号 { get; set; }

        public string 材料名称 { get; set; }

        public string 长度 { get; set; }

        public string 宽度 { get; set; }

        public string 高度 { get; set; }

        public string 数量 { get; set; }

        public string 型号 { get; set; }

        internal MaterialSheetData(MaterialSheetData data, int size)
        {
            this.序号 = data.序号;
            this.材料名称 = data.材料名称;
            this.长度 = data.长度;
            this.宽度 = data.宽度;
            this.高度 = data.高度;
            this.数量 = size.ToString();
            this.型号 = data.型号;
        }

        public MaterialSheetData()
        {

        }

        internal static List<MaterialSheetData> Get(string path)
        {
            List<MaterialSheetData> datas = new List<MaterialSheetData>();

            using (ExcelPackage package = new ExcelPackage(path))
            {
                var worksheet = package.Workbook.Worksheets["原材料设置"];
                int rows = worksheet.Dimension.End.Row;
                int cols = worksheet.Dimension.End.Column;

                int idcell = 0;
                int materialnamecell = 0;
                int lengthcell = 0;
                int widthcell = 0;
                int heightcell = 0;
                int sizecell = 0;
                int typecell = 0;

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
                        case "材料名称":
                            materialnamecell = cell; break;
                        case "长度":
                            lengthcell = cell; break;
                        case "宽度":
                            widthcell = cell; break;
                        case "高度":
                            heightcell = cell; break;
                        case "数量":
                            sizecell = cell; break;
                        case "型号":
                            typecell = cell; break;
                        default: break;
                    }
                }

                for (int row = 2; row <= rows; row++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[row, 1].Text)) break;
                    var tmp = new MaterialSheetData()
                    {
                        序号 = worksheet.Cells[row, idcell].Text,
                        材料名称 = worksheet.Cells[row, materialnamecell].Text,
                        长度 = worksheet.Cells[row, lengthcell].Text,
                        宽度 = worksheet.Cells[row, widthcell].Text,
                        高度 = worksheet.Cells[row, heightcell].Text,
                        数量 = worksheet.Cells[row, sizecell].Text,
                        型号 = worksheet.Cells[row, typecell].Text,
                    };
                    datas.Add(tmp);
                }
            }
            return datas;
        }

        internal string GetSize()
        {
            return $"{this.长度}*{this.宽度}*{this.高度}";
        }

        public override string ToString()
        {
            return $"{this.长度}*{this.宽度}*{this.高度}={this.数量}";
        }
    }
}
