using System.Text.RegularExpressions;
using OfficeOpenXml;
using YaSha.DataManager.ListProcessing.CommonMethods;
using YaSha.DataManager.ListProcessing.CommonModel;

namespace YaSha.DataManager.ListProcessing.TotalSheet
{
    internal class TotalSheetOriginData
    {

        internal int startRow;

        internal string groupName;

        /// <summary>
        /// 物料名称
        /// </summary>
        internal string MaterialName { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        internal string Brand { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        internal string Type { get; set; }

        /// <summary>
        /// 套数
        /// </summary>
        internal int TotalNumber { get; set; }


        internal List<BaseBoard> Boards { get; set; }

        internal TotalSheetOriginData(int startRow, string groupName)
        {
            Boards = new List<BaseBoard>();
            this.groupName = groupName;
            this.startRow = startRow;
        }

        internal void Distanct()
        {
            var groups = this.Boards.GroupBy(x => new { x.Length, x.Width, x.Height });

            List<BaseBoard> boards = new List<BaseBoard>();

            foreach (var group in groups)
            {
                var totalsize = group.Sum(x => x.Size);

                var board = new BaseBoard(group.Key.Length, group.Key.Width, group.Key.Height, totalsize);

                boards.Add(board);
            }

            this.Boards = new List<BaseBoard>(boards);
        }

        internal static List<BaseInfoBoard> GetInfo(ExcelWorksheets sheets, SheetConfig config)
        {
            List<BaseInfoBoard> boards = new List<BaseInfoBoard>();

            var startRow = config?.Origin?.SheetStartRow;

            var worksheet = sheets[config?.Origin?.SheetName];

            if (worksheet == null)
            {
                throw new Exception($"生成总单失败源文件缺失{config?.Origin?.SheetName}");
            }

            int rows = worksheet.Dimension.End.Row;

            int cols = worksheet.Dimension.End.Column;

            for (int row = startRow.Value + 1; row <= rows; row++)
            {
                var cell = worksheet.Cells[row, 1];

                if (!cell.Merge && cell.Value != null)
                {
                    var rowData = new BaseInfoBoard();

                    try
                    {

                        rowData.Length = int.Parse(worksheet.Cells[row, ExcelHelper.GetColumn(worksheet, startRow.Value, "物料-长")].Text);

                        rowData.Width = int.Parse(worksheet.Cells[row, ExcelHelper.GetColumn(worksheet, startRow.Value, "展开面")].Text);

                        rowData.Height = int.Parse(worksheet.Cells[row, ExcelHelper.GetColumn(worksheet, startRow.Value, "物料-高")].Text);

                        rowData.Size = int.Parse(worksheet.Cells[row, ExcelHelper.GetColumn(worksheet, startRow.Value, "批量")].Text);

                        rowData.Model = worksheet.Cells[row, ExcelHelper.GetColumn(worksheet, startRow.Value, "型号")].Text;

                        rowData.ProcessInfo = worksheet.Cells[row, ExcelHelper.GetColumn(worksheet, startRow.Value, "加工说明")].Text;

                        boards.Add(rowData);
                    }
                    catch
                    {

                    }
                }
            }

            return boards;
        }

        internal static List<TotalSheetOriginData> Get(ExcelWorksheets sheets, SheetConfig config)
        {
            var startRow = config?.Origin?.SheetStartRow;

            List<TotalSheetOriginData> originDatas = new List<TotalSheetOriginData>();

            var worksheet = sheets[config?.Origin?.SheetName];

            if (worksheet == null)
            {
                throw new Exception($"生成总单失败源文件缺失{config?.Origin?.SheetName}");
            }

            int rows = worksheet.Dimension.End.Row;

            int cols = worksheet.Dimension.End.Column;

            int materialNameCell = 0;

            int brandCell = 0;

            int typeCell = 0;

            int bowlineCell = 0;

            int batchCell = 0;

            int totalCell = 0;

            for (int cell = 1; cell <= cols; cell++)
            {
                var value = worksheet.Cells[startRow.Value, cell].Value;

                if (value == null)
                {
                    value = "";
                }

                if (value.Equals("物料名称"))
                {
                    materialNameCell = cell;
                }
                else if (value.Equals("品牌"))
                {
                    brandCell = cell;
                }
                else if (value.Equals("型号"))
                {
                    typeCell = cell;
                }
                else if (value.Equals("单套"))
                {
                    bowlineCell = cell;
                }
                else if (value.Equals("批量"))
                {
                    batchCell = cell;
                }
                else if (value.Equals("合计"))
                {
                    totalCell = cell;
                }
            }

            for (int row = startRow.Value + 1; row <= rows; row++)
            {
                var cell = worksheet.Cells[row, 1];

                if (cell.Merge && cell.Value != null)
                {
                    var rowData = new TotalSheetOriginData(row, cell.Value.ToString());

                    rowData.MaterialName = worksheet.Cells[row + 1, materialNameCell].Value.ToString();

                    rowData.Brand = worksheet.Cells[row + 1, brandCell].Value.ToString();

                    rowData.Type = worksheet.Cells[row + 1, typeCell].Value.ToString();

                    string bowline = worksheet.Cells[row + 1, bowlineCell].Value.ToString();

                    string batch = worksheet.Cells[row + 1, batchCell].Value.ToString();

                    rowData.TotalNumber = int.Parse(batch) / int.Parse(bowline);

                    originDatas.Add(rowData);
                }
            }

            for (int i = 0; i < originDatas.Count; i++)
            {
                int eRow = 0;

                if (i < originDatas.Count - 1)
                {
                    eRow = originDatas[i + 1].startRow;
                }
                else
                {
                    eRow = rows;
                }

                for (int row = originDatas[i].startRow + 1; row < eRow; row++)
                {
                    var cell = worksheet.Cells[row, totalCell];

                    if (cell.Value != null)
                    {
                        var t = Regex.Replace(cell.Value.ToString(), @"[^0-9]+", "*").Split('*').ToList();

                        t.RemoveAll(x => string.IsNullOrEmpty(x));

                        if (t.Count % 4 == 0)
                        {
                            for (int j = 0; j < t.Count / 4; j++)
                            {
                                originDatas[i].Boards.Add(
                                new BaseBoard(
                                int.Parse(t[j * 4 + 0]),
                                int.Parse(t[j * 4 + 1]),
                                int.Parse(t[j * 4 + 2]),
                                int.Parse(t[j * 4 + 3])));
                            }
                        }
                        else
                        {

                        }
                    }
                }
            }

            foreach (var item in originDatas)
            {
                item.Distanct();
            }
            return originDatas;

        }
    }
}
