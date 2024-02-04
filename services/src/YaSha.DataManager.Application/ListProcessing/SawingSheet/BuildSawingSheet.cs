using OfficeOpenXml;
using OfficeOpenXml.Style;
using YaSha.DataManager.ListProcessing.Base;
using YaSha.DataManager.ListProcessing.CommonModel;
using YaSha.DataManager.ListProcessing.SawingSheet.SheetModel;

namespace YaSha.DataManager.ListProcessing.SawingSheet
{
    public class BuildSawingSheet : ListProcessingBuildBase
    {
        string nestRulePath;

        string dt;

        internal override string TableName { get => "开料单"; set => TableName = value; }

        public BuildSawingSheet(BuildFactory factory) : base(factory)
        {
            order = 8;
            this.nestRulePath = factory.nestrulePath;
            this.dt = factory.dt;
        }
        internal override ExcelWorksheet ProcessExcel(ExcelWorksheets sheets)
        {
            //原始数据
            var originDatas = OriginData.Get(sheets, config);

            //套料原材料
            var materialDatas = MaterialSheetData.Get(nestRulePath);

            //套料设置
            var materialSets = NestSetSheetData.Get(nestRulePath);

            //排序规则
            var materialsortRules = NestSortSheetData.Get(rulePath);

            //修改规则
            var materialchangeRules = NestChangeSheetData.Get(rulePath);

            NestFactory factory = new NestFactory(originDatas, materialDatas, materialchangeRules, materialsortRules, materialSets);

            var outgroups = factory.Start();

            AddCuttingSheetDatas(outgroups, config);

            return config.Sheet;
        }

        void AddCuttingSheetDatas(List<OutGroups> outGroups, SheetConfig config)
        {
            var table = config.Sheet;
            int startRow = config.SheetStartRow;
            string headName = sheetName.Replace("内加工清单", "") + $"——开料合计[{dt}]套";
            table.Cells[1, 3].Value = headName;
            table.Cells[1, 3].Style.Font.Size = 26;
            int endcol = GetCellNumByName(table, startRow, "标准板块");
            if (endcol == -1)
            {
                endcol = table.Cells["AW6"].End.Column - 1;
            }
            else
            {
                endcol = endcol - 1;
            }

            int row = startRow + 1;
            int id = 1;
            foreach (var group in outGroups)
            {
                WriteOneGroup(table, group, startRow, endcol, ref row, ref id);
            }

            WriteTotalInfo(table, outGroups, startRow);
        }

        static void WriteOneGroup(ExcelWorksheet template, OutGroups group, int startRow, int endCell, ref int row, ref int id)
        {
            WriteMergeCell(template, row, 1, row, endCell, group.Name);
            row += 1;
            int mergecell = GetCellNumByName(template, startRow, "合计");
            int startndAreaCell = GetCellNumByName(template, startRow, "标准板块");
            foreach (var excelResult in group.ExcelResults)
            {
                int mergerStartRow = row;
                foreach (var item in excelResult.OriginDatas)
                {
                    for (int cell = 1; cell <= endCell; cell++)
                    {
                        var head = template.Cells[startRow, cell].Text;

                        if (head == "")
                        {
                            break;
                        }
                        if (head == "序号")
                        {
                            template.Cells[row, cell].Value = id;
                        }
                        else if (head == "合计")
                        {
                            if (excelResult.OriginDatas.Count == 1)
                            {
                                if (excelResult.Materials.Count == 0)
                                {
                                    template.Cells[row, cell].Value = "未获取到合适规格的原材料";
                                }
                                else
                                {
                                    string str = string.Empty;
                                    for (int i = 0; i < excelResult.Materials.Count; i++)
                                    {
                                        str += excelResult.Materials[i].ToString();
                                        if (i < excelResult.Materials.Count - 1)
                                        {
                                            str += ";";
                                        }
                                    }
                                    template.Cells[row, cell].Value = str;
                                    //标准板块面积
                                    WriteCell(template, row, startndAreaCell, Math.Round(excelResult.StarandArea, 2));
                                }
                            }
                            else
                            {
                                template.Cells[row, cell].Value = "";
                            }
                        }
                        else if (item.Datas.ContainsKey(head))
                        {
                            var str = item.Datas[head];
                            if (System.Text.RegularExpressions.Regex.IsMatch(str, @"^\d+$"))
                            {
                                template.Cells[row, cell].Value = double.Parse(str);
                            }
                            else
                            {
                                template.Cells[row, cell].Value = str;
                            }
                        }
                    }

                    row++;
                    id++;
                }
                int mergerEndRow = row - 1;
                if (excelResult.OriginDatas.Count > 1)
                {
                    string str = string.Empty;
                    for (int i = 0; i < excelResult.Materials.Count; i++)
                    {
                        str += excelResult.Materials[i].ToString();
                        if (i < excelResult.Materials.Count - 1)
                        {
                            str += "\n";
                        }
                    }
                    WriteMergeCell(template, mergerStartRow, mergecell, mergerEndRow, mergecell, str);
                    WriteMergeCell(template, mergerStartRow, startndAreaCell, mergerEndRow, startndAreaCell, Math.Round(excelResult.StarandArea, 2), true);
                }
            }
        }

        static void WriteTotalInfo(ExcelWorksheet sheet, List<OutGroups> groups, int startRow)
        {
            int startCell = GetCellNumByName(sheet, startRow, "材质（含毛料）");
            var datas = groups.GroupBy(x => x.Material).ToList();
            int row = startRow + 1;
            double totalArea1 = 0, totalArea2 = 0;
            foreach (var data in datas)
            {
                int cell = startCell;
                WriteCell(sheet, row, cell++, data.Key);
                List<OutGroups> tmp = new List<OutGroups>();
                foreach (var item in data)
                {
                    tmp.Add(item);
                }
                double area1 = tmp.Sum(x => x.ProcessBoardArea);
                double area2 = tmp.Sum(x => x.MaterialArea);
                totalArea1 += area1;
                totalArea2 += area2;
                WriteCell(sheet, row, cell++, Math.Round(area1, 2));
                WriteCell(sheet, row, cell++, Math.Round(area2, 2));
                WriteCell(sheet, row, cell++, Math.Round(area1 / area2, 2));
                WriteCell(sheet, row, cell++, Math.Round(area2 / area1, 2));
                row++;
            }
            WriteCell(sheet, row, startCell++, "合计:");
            WriteCell(sheet, row, startCell++, Math.Round(totalArea1, 2));
            WriteCell(sheet, row, startCell++, Math.Round(totalArea2, 2));
            WriteCell(sheet, row, startCell++, Math.Round(totalArea1 / totalArea2, 2));
            WriteCell(sheet, row, startCell++, Math.Round(totalArea2 / totalArea1, 2));

        }

        static void WriteMergeCell(ExcelWorksheet sheet, int sr, int sc, int er, int ec, object obj, bool bold = false)
        {
            sheet.Cells[sr, sc, er, ec].Merge = true;
            sheet.Cells[sr, sc, er, ec].Style.Font.Size = 16;
            sheet.Cells[sr, sc, er, ec].Style.Font.Bold = bold;
            sheet.Cells[sr, sc, er, ec].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[sr, sc, er, ec].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[sr, sc].Value = obj;
        }

        static void WriteCell(ExcelWorksheet sheet, int row, int cell, object value)
        {
            sheet.Cells[row, cell].Value = value;
            sheet.Cells[row, cell].Style.Font.Size = 16;
            sheet.Cells[row, cell].Style.Font.Bold = true;
            sheet.Cells[row, cell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[row, cell].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells[row, cell].Style.Numberformat.Format = "";
        }

        static int GetCellNumByName(ExcelWorksheet sheet, int startRow, string name)
        {
            int col = sheet.Dimension.End.Column;

            for (int i = 1; i <= col; i++)
            {
                string value = sheet.Cells[startRow, i].Text;
                if (value == name)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
