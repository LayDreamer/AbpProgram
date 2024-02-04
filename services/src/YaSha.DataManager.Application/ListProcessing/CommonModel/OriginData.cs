using OfficeOpenXml;

namespace YaSha.DataManager.ListProcessing.CommonModel
{
    [Serializable]
    public class OriginData
    {
        /// <summary>
        /// Excel列名 
        /// </summary>
        public Dictionary<string, string> Datas { get; set; }

        public OriginData()
        {
            Datas = new Dictionary<string, string>();
        }

        public override string ToString()
        {
            string str = string.Empty;

            if (Datas.ContainsKey("序号"))
            {
                str += $"序号 {Datas["序号"]} ";
            }
            if (Datas.ContainsKey("展开面"))
            {
                str += $"展开面 {Datas["展开面"]} ";
            }
            if (Datas.ContainsKey("物料-长"))
            {
                str += $"物料-长 {Datas["物料-长"]} ";
            }
            if (Datas.ContainsKey("批量"))
            {
                str += $"批量 {Datas["批量"]} ";
            }

            return str;
        }

        internal static List<OriginData> Get(ExcelWorksheets sheets, SheetConfig config)
        {
            List<OriginData> res = new List<OriginData>();

            var startRow = config?.Origin?.SheetStartRow;

            var originSheetName = config?.Origin?.SheetName;

            var worksheet = sheets[originSheetName];

            if (null == worksheet)
            {
                if (originSheetName == "内加工清单")
                {
                    worksheet = sheets[0];

                    if(worksheet == null)
                    {
                        throw new Exception($"生成{config.SheetName}失败 源文件中没有{originSheetName}");
                    }
                }
                else
                {
                    throw new Exception($"生成{config.SheetName}失败 源文件中没有{originSheetName}");
                }
            }

            int rows = worksheet.Dimension.End.Row;

            int cols = worksheet.Dimension.End.Column;

            Dictionary<string, int> allColumns = new Dictionary<string, int>();

            for (int i = 1; i < cols; i++)
            {
                var value = worksheet.Cells[startRow.Value, i].Text;

                if (!string.IsNullOrEmpty(value))
                {
                    allColumns.Add(value, i);
                }
            }

            for (int row = startRow.Value + 1; row <= rows; row++)
            {
                OriginData originData = new OriginData();

                var v = worksheet.Cells[row, 1].Text;

                if (string.IsNullOrEmpty(v))
                {
                    break;
                }

                foreach (var item in allColumns)
                {
                    originData.Datas.Add(item.Key, worksheet.Cells[row, item.Value].Text);
                }

                res.Add(originData);
            }
            return res;
        }

    }
}
