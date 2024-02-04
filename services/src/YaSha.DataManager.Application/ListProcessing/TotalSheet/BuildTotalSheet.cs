using OfficeOpenXml;
using OfficeOpenXml.Style;
using YaSha.DataManager.ListProcessing.Base;
using YaSha.DataManager.ListProcessing.CommonMethods;
using YaSha.DataManager.ListProcessing.CommonModel;

namespace YaSha.DataManager.ListProcessing.TotalSheet
{
    public class BuildTotalSheet : ListProcessingBuildBase
    {
        internal override string TableName { get => "总单"; set => TableName = value; }


        public BuildTotalSheet(BuildFactory factory) : base(factory)
        {
            order = 32;
        }


        internal override ExcelWorksheet ProcessExcel(ExcelWorksheets sheets)
        {
            //源数据
            var originDatas = TotalSheetOriginData.Get(sheets, config);

            //源数据
            var baseDatas = TotalSheetOriginData.GetInfo(sheets, config);

            //输出表单样式
            var outDatas = TotalSheetOutData.Get(originDatas);

            //规则表单数据
            var ruleDatas = TotalSheetRule.Get(rulePath);

            //总单宽度系
            var widthDatas = TotalSheetRule.GetWidthRule(rulePath);

            AddTotalSheetDatas(config.Sheet, baseDatas, outDatas, ruleDatas, widthDatas, config.SheetStartRow, sheetName);

            return config.Sheet;
        }

        internal  bool AddTotalSheetDatas(ExcelWorksheet template,
            List<BaseInfoBoard> baseInfoBoards,
            List<TotalSheetOutData> outdatas,
            List<TotalSheetRule> rules,
            Dictionary<int, double> widthRules, int startRow, string fileName)
        {
            bool iRet = false;

            //去除序号字段
            var cRules = rules.Where(x => !x.field.Equals("序号") && x.type1.Equals("主值") && x.first).ToList();

            List<List<TotalSheetRule>> rRules = new List<List<TotalSheetRule>>();

            foreach (var item in cRules)
            {
                var temp = new List<TotalSheetRule>() { item };

                if (!string.IsNullOrEmpty(item.moreValue))
                {
                    var chars = item.moreValue.Split('、');

                    foreach (var ch in chars)
                    {
                        temp.Add(rules.Find(x => x.id.Equals(ch)));
                    }
                }

                rRules.Add(temp);
            }

            var kits = outdatas.First()?.Numbers;

            string headName = fileName.Replace("内加工清单", "") + $"和PVC膜总料单合计[{kits.Value}]套";

            string name = fileName.Replace("内加工清单", "") + $"(共计{kits.Value}套)";

            template.InsertRow(startRow + 1, outdatas.Count + 1);

            template.Cells[1, 2].Value = headName;
            template.Cells[1, 2].Style.Font.Size = 20;

            template.Cells[startRow + 1, 1, startRow + 1 + outdatas.Count, 1].Merge = true;
            template.Cells[startRow + 1, 1, startRow + 1 + outdatas.Count, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            template.Cells[startRow + 1, 1, startRow + 1 + outdatas.Count, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            template.Cells[startRow + 1, 1, startRow + 1 + outdatas.Count, 1].Style.WrapText = true;
            template.Cells[startRow + 1, 1, startRow + 1 + outdatas.Count, 1].Value = name;

            template.Cells[startRow + 1, 2, startRow + 1 + outdatas.Count, 2].Merge = true;
            template.Cells[startRow + 1, 2, startRow + 1 + outdatas.Count, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            template.Cells[startRow + 1, 2, startRow + 1 + outdatas.Count, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            template.Cells[startRow + 1, 2, startRow + 1 + outdatas.Count, 2].Value = kits.Value;


            for (int i = 0; i < outdatas.Count; i++)
            {
                template.Cells[startRow + i + 1, 3].Value = outdatas[i].MaterialInfo;

                template.Cells[startRow + i + 1, 4].Value = outdatas[i].Length;

                template.Cells[startRow + i + 1, 5].Value = outdatas[i].Width;

                template.Cells[startRow + i + 1, 6].Value = outdatas[i].Height;

                template.Cells[startRow + i + 1, 7].Value = outdatas[i].Brand;

                template.Cells[startRow + i + 1, 8].Value = outdatas[i].Finish;

                template.Cells[startRow + i + 1, 9].Value = outdatas[i].Total;

                string tmp = GetSize(outdatas[i].MaterialInfo, outdatas[i].Length, outdatas[i].Width, outdatas[i].Height, rRules);

                template.Cells[startRow + i + 1, 10].Value = tmp;

                template.Cells[startRow + i + 1, 11].Value = Math.Round(1.0 * outdatas[i].Length * outdatas[i].Width * outdatas[i].Total / 1e6, 2);
            }

            template.Cells[startRow + 1 + outdatas.Count, 3, startRow + 1 + outdatas.Count, 8].Merge = true;
            template.Cells[startRow + 1 + outdatas.Count, 3, startRow + 1 + outdatas.Count, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            template.Cells[startRow + 1 + outdatas.Count, 3, startRow + 1 + outdatas.Count, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            template.Cells[startRow + 1 + outdatas.Count, 3, startRow + 1 + outdatas.Count, 8].Value = "合计量";

            template.Cells[startRow + 1 + outdatas.Count, 9].Formula = $"SUM(I{startRow + 1}:I{startRow + outdatas.Count})";
            template.Cells[startRow + 1 + outdatas.Count, 11].Formula = $"SUM(K{startRow + 1}:K{startRow + outdatas.Count})";

            template.Cells[startRow + 1 + outdatas.Count, 9, startRow + 1 + outdatas.Count, 11].Style.Fill.PatternType = ExcelFillStyle.Solid;
            template.Cells[startRow + 1 + outdatas.Count, 9, startRow + 1 + outdatas.Count, 11].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);

            using (var range = template.Cells[startRow + 1, 1, startRow + 1 + outdatas.Count, 11])
            {
                range.Style.Font.Bold = true;
            }

            #region 总表底部的表

            var finish = outdatas.GroupBy(x => x.Finish).Where(x => !x.Key.Contains("毛料")).ToList();

            var tmpFinish = baseInfoBoards.Where(x => (x.ProcessInfo.Contains("B:封边") || x.ProcessInfo.Contains("D:封边")) && x.Model != "黑色" && x.Model != "毛料").GroupBy(x => x.Model).ToList();

            int row = startRow + outdatas.Count + 3;
            var tmpRules = rules.Where(x => !x.first).ToList();
            var maxRow = finish.Count + tmpFinish.Count;
            var column = ExcelHelper.GetColumn(template, row, "区域");
            template.Cells[row + 1, column, row + maxRow, column].Merge = true;
            template.Cells[row + 1, column, row + maxRow, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            template.Cells[row + 1, column, row + maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            template.Cells[row + 1, column, row + maxRow, column].Value = "顶墙车间用";
            column = ExcelHelper.GetColumn(template, row, "单位");
            for (int i = 1; i <= maxRow; i++)
            {
                template.Cells[row + i, column, row + i, column + 1].Merge = true;
                template.Cells[row + i, column, row + i, column + 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                template.Cells[row + i, column, row + i, column + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                template.Cells[row + i, column].Value = "米";
            }
            try
            {
                column = ExcelHelper.GetColumn(template, row, "名称");
                template.Cells[row + 1, column, row + finish.Count, column].Merge = true;
                template.Cells[row + 1, column, row + finish.Count, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                template.Cells[row + 1, column, row + finish.Count, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                template.Cells[row + 1, column, row + finish.Count, column].Value = "顶墙面PVC膜";
            }
            catch (Exception ex)
            {

            }

            try
            {
                column = ExcelHelper.GetColumn(template, row, "备注");
                template.Cells[row + 1, column, row + finish.Count, column].Merge = true;
                template.Cells[row + 1, column, row + finish.Count, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                template.Cells[row + 1, column, row + finish.Count, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                template.Cells[row + 1, column, row + finish.Count, column].Value = "1260/1370宽，实际用量为准";
            }
            catch (Exception ex)
            {

            }
            try
            {
                if (tmpFinish.Count > 0)
                {
                    column = ExcelHelper.GetColumn(template, row, "名称");
                    template.Cells[row + finish.Count + 1, column, row + finish.Count + tmpFinish.Count, column].Merge = true;
                    template.Cells[row + finish.Count + 1, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    template.Cells[row + finish.Count + 1, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    template.Cells[row + finish.Count + 1, column].Value = "封边条";
                }
            }
            catch (Exception ex)
            {

            }

            var d1 = outdatas.Where(x => !x.MaterialInfo.Contains("琉晶颗粒板") && x.Finish != "毛料").GroupBy(x => x.MaterialInfo + "-" + x.Width + "-" + x.Height + "-" + x.Finish).ToList();

            var d2 = outdatas.Where(x => x.MaterialInfo.Contains("琉晶颗粒板") && x.Finish != "毛料").GroupBy(x => "琉晶颗粒板" + "-" + x.Width + "-" + x.Height + "-" + x.Finish).ToList();

            int tmpRow = row + 1;

            Dictionary<string, double> tmpDatas = new Dictionary<string, double>();

            foreach (var item in d1)
            {
                template.Cells[tmpRow, ExcelHelper.GetColumn(template, row, "分组")].Value = item.Key;

                double sum = 0;

                foreach (var t in item)
                {
                    var v = widthRules[t.Width] * (t.Length + 20) * t.Total / 1000.0;

                    sum += v;
                }

                double guocheng1 = Math.Round(sum + 5 * widthRules[item.First().Width], 2);

                template.Cells[tmpRow, ExcelHelper.GetColumn(template, row, "过程一")].Value = guocheng1;

                if (guocheng1 > 250)
                {
                    double tValue = (guocheng1 - 250) / 250.0;

                    double guocheng2 = Math.Ceiling(tValue);

                    template.Cells[tmpRow, ExcelHelper.GetColumn(template, row, "过程二")].Value = guocheng2;

                    double zongyongliang2 = guocheng1 + guocheng2 * 5 * widthRules[item.First().Width];

                    template.Cells[tmpRow, ExcelHelper.GetColumn(template, row, "总用量2")].Value = zongyongliang2;

                    tmpDatas.Add(item.Key, zongyongliang2);
                }
                else
                {
                    template.Cells[tmpRow, ExcelHelper.GetColumn(template, row, "总用量2")].Value = guocheng1;
                    tmpDatas.Add(item.Key, guocheng1);
                }

                tmpRow++;
            }

            foreach (var item in d2)
            {
                template.Cells[tmpRow, ExcelHelper.GetColumn(template, row, "分组")].Value = item.Key;

                double sum = 0;

                foreach (var t in item)
                {
                    var v = widthRules[t.Width] * (t.Length + 20) * t.Total / 1000.0;

                    sum += v;
                }

                double guocheng1 = Math.Round(sum + 5 * widthRules[item.First().Width], 2);

                template.Cells[tmpRow, ExcelHelper.GetColumn(template, row, "过程一")].Value = guocheng1;

                if (guocheng1 > 250)
                {
                    double tValue = (guocheng1 - 250) / 250.0;

                    double guocheng2 = Math.Ceiling(tValue);

                    template.Cells[tmpRow, ExcelHelper.GetColumn(template, row, "过程二")].Value = guocheng2;

                    double zongyongliang2 = guocheng1 + guocheng2 * 5 * widthRules[item.First().Width];

                    template.Cells[tmpRow, ExcelHelper.GetColumn(template, row, "总用量2")].Value = zongyongliang2;

                    tmpDatas.Add(item.Key, zongyongliang2);
                }
                else
                {
                    template.Cells[tmpRow, ExcelHelper.GetColumn(template, row, "总用量2")].Value = guocheng1;
                    tmpDatas.Add(item.Key, guocheng1);
                }

                tmpRow++;
            }

            using (var range = template.Cells[row + 1, 1, tmpRow, template.Dimension.Columns])
            {
                range.Style.Font.Bold = true;
            }
            tmpRow = row + 1;
            foreach (var item in finish)
            {
                template.Cells[tmpRow, 4].Value = item.Key;
                template.Cells[tmpRow, 4, tmpRow, 7].Merge = true;
                template.Cells[tmpRow, 4, tmpRow, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                template.Cells[tmpRow, 4, tmpRow, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                var v = tmpDatas.ToList().FindAll(x => x.Key.Contains(item.Key)).Sum(x => x.Value);
                template.Cells[tmpRow, ExcelHelper.GetColumn(template, row, "总用量")].Value = Math.Ceiling(v);

                tmpRow++;
            }

            tmpRow = row + 1 + finish.Count;

            foreach (var item in tmpFinish)
            {
                template.Cells[tmpRow, 4].Value = item.Key;
                template.Cells[tmpRow, 4, tmpRow, 7].Merge = true;
                template.Cells[tmpRow, 4, tmpRow, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                template.Cells[tmpRow, 4, tmpRow, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                double v = 0;

                foreach (var d in item)
                {
                    int bianshu = 0;

                    if (d.ProcessInfo.Contains("B") && d.ProcessInfo.Contains("D"))
                    {
                        bianshu = 2;
                    }
                    else
                    {
                        bianshu = 1;
                    }

                    var t = d.Size * (d.Width * bianshu + 60) / 1000.0;

                    v += t;
                }

                template.Cells[tmpRow, ExcelHelper.GetColumn(template, row, "总用量")].Value = Math.Ceiling(v);

                tmpRow++;
            }

            #endregion

            return iRet;
        }

        internal string GetSize(string s, int l, int w, int h, List<List<TotalSheetRule>> rules)
        {
            string r = string.Empty;

            if (string.IsNullOrEmpty(s))
            {
                return r;
            }

            foreach (var item in rules)
            {
                r = GetValue(s, l, w, h, item);

                if (!string.IsNullOrEmpty(r))
                {
                    break;
                }
            }

            return r;
        }

        internal static string GetValue(string s, int l, int w, int h, List<TotalSheetRule> rules)
        {
            string iRet = string.Empty;

            var main = rules.Find(x => x.type1 == "主值");

            var index = rules.IndexOf(main);

            if (main.field.Contains("长"))
            {
                var value = rules.Find(x => !string.IsNullOrEmpty(x.evaluationValue))?.evaluationValue;

                if (rules[index + 1].type1 == "或")
                {
                    var flag = false;
                    foreach (var item in rules)
                    {
                        if (item.value.Equals(l.ToString()))
                        {
                            flag = true;
                        }
                    }
                    if (flag)
                    {
                        iRet = $"{l}*{w}*{h}";
                    }
                }
                else if (rules[index + 1].type1 == "且")
                {
                    var flag = false;

                    foreach (var item in rules)
                    {
                        if (item.field == "物料名称")
                        {
                            if (s.Contains("科岩"))
                            {
                                flag = false;
                            }
                        }
                        else
                        {
                            if (item.value.Equals(l.ToString()))
                            {
                                flag = true;
                            }
                        }
                    }

                    if (flag)
                    {
                        iRet = $"{value}*{w}*{h}";
                    }
                }
            }
            else if (main.field.Contains("宽"))
            {
                var value = rules.Find(x => !string.IsNullOrEmpty(x.evaluationValue))?.evaluationValue;

                if (rules[index + 1].type1 == "或")
                {
                    var flag = false;
                    foreach (var item in rules)
                    {
                        if (item.value.Equals(w.ToString()))
                        {
                            flag = true;
                        }
                    }
                    if (flag)
                    {
                        iRet = $"{l}*{w}*{h}";
                    }
                }
                else if (rules[index + 1].type1 == "且")
                {
                    var flag = false;

                    foreach (var item in rules)
                    {
                        if (item.field == "物料名称")
                        {
                            if (s.Contains(item.value))
                            {
                                flag = false;
                            }
                        }
                        else
                        {
                            if (item.value.Equals(w.ToString()))
                            {
                                flag = true;
                            }
                        }
                    }

                    if (flag)
                    {
                        iRet = $"{l}*{value}*{h}";
                    }
                }
            }

            return iRet;
        }
    }
}
