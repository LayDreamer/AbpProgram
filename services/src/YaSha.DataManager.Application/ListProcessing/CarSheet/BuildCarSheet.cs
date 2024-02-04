using YaSha.DataManager.ListProcessing;
using OfficeOpenXml;
using YaSha.DataManager.ListProcessing.Base;
using YaSha.DataManager.ListProcessing.CommonModel;

namespace YaSha.DataManager.ListProcessing.CarSheet
{
    public class BuildCarSheet : ListProcessingBuildBase
    {
        string dt;

        internal override string TableName { get => "车间五金"; set => TableName = value; }

        public BuildCarSheet(BuildFactory factory) : base(factory)
        {
            order = 2;
            dt = factory.dt;
        }

        internal override ExcelWorksheet ProcessExcel(ExcelWorksheets sheets)
        {
            //原始数据
            var originDatas = OriginData.Get(sheets, config);

            //过滤规则
            var filterRules = CarSheetFilterRule.Get(rulePath);

            //过滤后的数据
            var filterDatas = CarSheetFilterRule.Filter(originDatas, filterRules);

            //排序规则
            var sortRules = CarSheetSortRule.Get(rulePath);

            //排序后的数据
            var sortDatas = CarSheetSortRule.Sort(filterDatas, sortRules);

            //规则
            var rules = CarSheetRule.Get(rulePath);

            AddCarSheetDatas(config, sortDatas, rules, sheetName, dt);

            return config.Sheet;
        }

        static void AddCarSheetDatas(
           SheetConfig config,
           List<OriginData> originDatas,
           List<CarSheetRule> rules,
           string fileName,
           string dt_num)
        {
            var template = config.Sheet;
            var startRow = config.SheetStartRow;
            var groups = originDatas.GroupBy(x => new
            {
                v = x.Datas.ContainsKey("物料名称") ? x.Datas["物料名称"] : string.Empty,
                m = x.Datas.ContainsKey("物料编码") ? x.Datas["物料编码"] : string.Empty
            });
            template.Cells[startRow + 1, 1, startRow + 1 + groups.Count(), template.Dimension.End.Column].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

            int i = 0;

            string headName = fileName.Replace("内加工清单", "") + $"——车间五金合计[{dt_num}]套";
            template.Cells[1, 3].Value = headName;
            template.Cells[1, 3].Style.Font.Size = 20;

            var groupRules = GetRules(rules);
            foreach (var item in groups)
            {
                var clone = item.FirstOrDefault();

                int row = i++ + startRow + 1;

                for (int j = 1; j < template.Dimension.End.Column; j++)
                {
                    var head = template.Cells[startRow, j].Text;

                    if (head == "")
                    {
                        break;
                    }
                    try
                    {
                        if (head == "单套")
                        {
                            template.Cells[row, j].Value = item.Sum(x =>
                            {
                                if (x.Datas.ContainsKey("总配额"))
                                {
                                    return double.Parse(x.Datas["总配额"]);
                                }
                                else
                                {
                                    return 0;
                                }
                            });
                        }
                        else if (head == "序号")
                        {
                            template.Cells[row, j].Value = i;
                        }
                        else if (head == "物料-长")
                        {
                            int count = item.Select(x =>
                            {
                                if (x.Datas.ContainsKey("物料-长"))
                                {
                                    return x.Datas["物料-长"];
                                }
                                else
                                {
                                    return string.Empty;
                                }
                            }).Distinct().Count();

                            if (count == 1 && clone.Datas.ContainsKey("物料-长") && IsNumber(clone.Datas["物料-长"]))
                            {
                                template.Cells[row, j].Value = Convert.ToDouble(clone.Datas["物料-长"]);
                            }
                            else
                            {
                                template.Cells[row, j].Value = "/";
                            }
                        }
                        else if (head == "物料-宽")
                        {
                            int count = item.Select(x =>
                            {
                                if (x.Datas.ContainsKey("物料-宽"))
                                {
                                    return x.Datas["物料-宽"];
                                }
                                else
                                {
                                    return string.Empty;
                                }
                            }).Distinct().Count();

                            if (count == 1 && clone.Datas.ContainsKey("物料-宽") && IsNumber(clone.Datas["物料-宽"]))
                            {
                                template.Cells[row, j].Value = Convert.ToDouble(clone.Datas["物料-宽"]);
                            }
                            else
                            {
                                template.Cells[row, j].Value = "/";
                            }
                        }
                        else if (head == "物料-高")
                        {
                            int count = item.Select(x =>
                            {
                                if (x.Datas.ContainsKey("物料-高"))
                                {
                                    return x.Datas["物料-高"];
                                }
                                else
                                {
                                    return string.Empty;
                                }
                            }).Distinct().Count();

                            if (count == 1 && clone.Datas.ContainsKey("物料-高") && IsNumber(clone.Datas["物料-高"]))
                            {
                                template.Cells[row, j].Value = Convert.ToDouble(clone.Datas["物料-高"]);
                            }
                            else
                            {
                                template.Cells[row, j].Value = "/";
                            }
                        }
                        else if (head == "批量")
                        {
                            double v1 = double.Parse(template.Cells[row, j - 1].Text);

                            double v2 = dt_num == "" ? 0 : double.Parse(dt_num);


                            template.Cells[row, j].Value = v1 * v2;
                        }
                        else
                        {
                            if (clone.Datas.ContainsKey(head))
                            {
                                var obj = clone.Datas[head];

                                if (IsNumber(obj))
                                {
                                    template.Cells[row, j].Value = Convert.ToDouble(obj);
                                }
                                else
                                    template.Cells[row, j].Value = obj;
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }

                //需要修改
                foreach (var rule in groupRules)
                {
                    SetValue(rule, template, startRow, row);
                }
            }
        }

        static bool IsNumber(object value)
        {
            if (value is string)
            {
                var str = value as string;
                return double.TryParse(str, out _);
            }
            else
                return value is sbyte
                        || value is byte
                        || value is short
                        || value is ushort
                        || value is int
                        || value is uint
                        || value is long
                        || value is ulong
                        || value is float
                        || value is double
                        || value is decimal;
        }

        static List<List<CarSheetRule>> GetRules(List<CarSheetRule> rules)
        {
            List<List<CarSheetRule>> results = new List<List<CarSheetRule>>();

            var main = rules.Where(x => x.类型1 == "主值").ToList();

            for (int i = 0; i < main.Count; i++)
            {
                if (i == main.Count - 1)
                {
                    if (main[i].序号 == rules.Last().序号)
                    {
                        results.Add(new List<CarSheetRule>() { main[i] });
                    }
                    else
                    {
                        var tmp = new List<CarSheetRule>();
                        for (int j = rules.IndexOf(main[i]); j < rules.Count; j++)
                        {
                            tmp.Add(rules[j]);
                        }
                        results.Add(tmp);
                    }
                }
                else
                {
                    var tmp = new List<CarSheetRule>();
                    for (int j = rules.IndexOf(main[i]); j < rules.IndexOf(main[i + 1]); j++)
                    {
                        tmp.Add(rules[j]);
                    }
                    results.Add(tmp);
                }
            }

            return results;
        }

        static void SetValue(List<CarSheetRule> rules, ExcelWorksheet excelWorksheet, int headRow, int row)
        {

            if (rules.Count == 1)
            {
                var rule = rules[0];

                var targetValue = GetRange(excelWorksheet, row, headRow, rule.字段).Text;

                bool next = false;

                if (rule.算法 == "等于")
                {
                    if (rule.值 == "空")
                    {
                        next = targetValue == "";
                    }
                    else
                    {
                        next = rule.值.Equals(targetValue);
                    }
                }
                else if (rule.算法 == "不等于")
                {
                    if (rule.值 == "空")
                    {
                        next = !string.IsNullOrEmpty(targetValue);
                    }
                    else
                    {
                        next = !rule.值.Equals(targetValue);
                    }
                }
                else if (rule.算法 == "包含")
                {
                    next = targetValue.Contains(rule.值);
                }
                else if (rule.算法 == "不包含")
                {
                    next = !targetValue.Contains(rule.值);
                }

                if (next)
                {
                    if (rule.赋值类型 == "值")
                    {
                        var range = GetRange(excelWorksheet, row, headRow, rule.赋值字段);

                        if (range != null)
                        {
                            range.Value = rule.赋值;
                        }
                    }
                    else if (rule.赋值类型 == "公式")
                    {

                    }
                    else if (rule.赋值类型 == "字段" && rule.字段 != "序号")
                    {
                        var range = GetRange(excelWorksheet, row, headRow, rule.赋值字段);

                        if (range != null)
                        {
                            range.Value = GetRange(excelWorksheet, row, headRow, rule.赋值).Value;
                        }
                    }
                }
            }
            else
            {
                bool flag = false;
                foreach (var rule in rules)
                {
                    var targetValue = GetRange(excelWorksheet, row, headRow, rule.字段)?.Text;

                    bool next = false;

                    if (targetValue != null)
                    {
                        if (rule.算法 == "等于")
                        {
                            if (rule.值 == "空")
                            {
                                next = targetValue == "";
                            }
                            else
                            {
                                next = rule.值.Equals(targetValue);
                            }
                        }
                        else if (rule.算法 == "不等于")
                        {
                            if (rule.值 == "空")
                            {
                                next = !string.IsNullOrEmpty(targetValue);
                            }
                            else
                            {
                                next = !rule.值.Equals(targetValue);
                            }
                        }
                        else if (rule.算法 == "包含")
                        {
                            next = targetValue.Contains(rule.值);
                        }
                        else if (rule.算法 == "不包含")
                        {
                            next = !targetValue.Contains(rule.值);
                        }
                    }

                    if (rule.类型1 == "或" || rule.类型1 == "主值")
                    {
                        flag |= next;
                    }
                    else if (rule.类型1 == "且")
                    {
                        flag &= next;
                    }
                }
                if (flag)
                {
                    var rule = rules.Last();

                    if (rule.赋值类型 == "值")
                    {
                        var range = GetRange(excelWorksheet, row, headRow, rule.赋值字段);

                        if (range != null)
                        {
                            range.Value = rule.赋值;
                        }
                    }
                    else if (rule.赋值类型 == "公式")
                    {

                    }
                    else if (rule.赋值类型 == "字段" && rule.字段 != "序号")
                    {
                        var range = GetRange(excelWorksheet, row, headRow, rule.赋值字段);

                        if (range != null)
                        {
                            range.Value = GetRange(excelWorksheet, row, headRow, rule.赋值).Value;
                        }
                    }
                }
            }
        }

        static ExcelRange GetRange(ExcelWorksheet excelWorksheet, int row, int startRow, string headName)
        {
            ExcelRange range = null;

            for (int cell = 1; cell < excelWorksheet.Dimension.End.Column; cell++)
            {
                var value = excelWorksheet.Cells[startRow, cell].Text;

                if (headName.Equals(value))
                {
                    range = excelWorksheet.Cells[row, cell];

                    return range;
                }
            }

            return null;
        }
    }
}
