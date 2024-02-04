using OfficeOpenXml;

namespace YaSha.DataManager.ListProcessing.CommonModel
{
    /// <summary>
    /// 输出样式模板
    /// </summary>
    internal class OutSheetTemplate
    {

        internal SheetConfig this[string name]
        {
            get
            {
                return Configs.FirstOrDefault(x => x.SheetName == name);
            }
        }

        private List<SheetConfig> Configs { get; set; }

        internal OutSheetTemplate(string path)
        {
            Configs = new List<SheetConfig>();

            ExcelPackage package = new ExcelPackage(path);

            var configSheet = package.Workbook.Worksheets["表单配置"];

            if (configSheet != null)
            {
                for (int cell = 3; cell <= 5; cell++)
                {
                    Configs.Add(new SheetConfig()
                    {
                        SheetName = configSheet.Cells[2, cell].Value.ToString(),

                        SheetStartRow = int.Parse(configSheet.Cells[3, cell].Value.ToString()),

                        Origin = new SheetConfig()
                        {
                            SheetName = configSheet.Cells[2, 2].Value.ToString(),

                            SheetStartRow = int.Parse(configSheet.Cells[3, 2].Value.ToString()),
                        },
                    });
                }

                for (int cell = 3; cell <= 4; cell++)
                {
                    Configs.Add(new SheetConfig()
                    {
                        SheetName = configSheet.Cells[4, cell].Value.ToString(),

                        SheetStartRow = int.Parse(configSheet.Cells[5, cell].Value.ToString()),

                        Origin = new SheetConfig()
                        {
                            SheetName = configSheet.Cells[4, 2].Value.ToString(),

                            SheetStartRow = int.Parse(configSheet.Cells[5, 2].Value.ToString()),
                        },
                    });
                }

                for (int cell = 3; cell <= 3; cell++)
                {
                    Configs.Add(new SheetConfig()
                    {
                        SheetName = configSheet.Cells[6, cell].Value.ToString(),

                        SheetStartRow = int.Parse(configSheet.Cells[7, cell].Value.ToString()),

                        Origin = new SheetConfig()
                        {
                            SheetName = configSheet.Cells[6, 2].Value.ToString(),

                            SheetStartRow = int.Parse(configSheet.Cells[7, 2].Value.ToString()),
                        },
                    });
                }
            }

            foreach (var item in Configs)
            {
                item.Sheet = package.Workbook.Worksheets[item.SheetName];
            }
        }
    }



    public class SheetConfig
    {
        internal string SheetName { get; set; }

        internal int SheetStartRow { get; set; }

        internal ExcelWorksheet Sheet { get; set; }

        internal SheetConfig Origin { get; set; }

    }
}
