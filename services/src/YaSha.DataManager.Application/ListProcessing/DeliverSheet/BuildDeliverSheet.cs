using YaSha.DataManager.ListProcessing;
using OfficeOpenXml;
using YaSha.DataManager.ListProcessing.Base;
using YaSha.DataManager.ListProcessing.CommonModel;

namespace YaSha.DataManager.ListProcessing.DeliverSheet
{
    public class BuildDeliverSheet : ListProcessingBuildBase
    {
        string dt;

        string type;

        internal override string TableName { get => "发货五金"; set => TableName = value; }

        public BuildDeliverSheet(BuildFactory factory) : base(factory)
        {
            order = 4;
            dt = factory.dt;
            type = factory.sheetType;
        }

        internal override ExcelWorksheet ProcessExcel(ExcelWorksheets sheets)
        {
            //原始数据
            var originDatas = OriginData.Get(sheets, config);

            //过滤规则
            var filterRules = DeliverSheetFilterRule.Get(rulePath);

            //过滤后的数据
            var filterDatas = DeliverSheetFilterRule.Filter(originDatas, filterRules);

            //排序规则
            var sortRules = DeilverSheetSortRule.Get(rulePath);

            //排序后的数据
            var sortDatas = DeilverSheetSortRule.Sort(filterDatas, sortRules);

            //不合并规则
            var mergeCellRule = DeilverMaterial.Get(rulePath);

            //修改规则
            var eilverRule = DeilverSheetRule.Get(rulePath);

            AddDeiliverSheetDatas(config, sortDatas, mergeCellRule, eilverRule, sheetName, dt, type);

            return config.Sheet;
        }

        static void AddDeiliverSheetDatas(
          SheetConfig config,
          List<OriginData> originDatas,
          List<DeilverMaterial> mergeRules,
          List<DeilverSheetRule> rules,
          string fileName,
          string dtnum,
          string type_1
           )
        {
            var template = config.Sheet;
            var startRow = config.SheetStartRow;
            string headName = fileName.Replace("内加工清单", "") + $"——发货五金合计[{dtnum}]套";
            template.Cells[1, 3].Value = headName;
            template.Cells[1, 3].Style.Font.Size = 20;

            var groupRules = GetRules(rules);

            var groups = Group.Get(originDatas, mergeRules);

            int i = 0;

            foreach (var item in groups)
            {
                var clone = item.originDatas.FirstOrDefault();

                int row = i++ + startRow + 1;

                //不用修改
                for (int j = 1; j < template.Dimension.End.Column; j++)
                {
                    var head = template.Cells[startRow, j].Text;

                    if (head == "")
                    {
                        break;
                    }
                    try
                    {
                        if (head == "序号")
                        {
                            template.Cells[row, j].Value = i;

                        }
                        else if (head == "订单类别")
                        {
                            template.Cells[row, j].Value = type_1;
                        }
                        else if (head == "单套")
                        {
                            double sum = item.originDatas.Sum(x => {
                                if (x.Datas.ContainsKey("单套"))
                                {
                                    return double.Parse(x.Datas["单套"]);
                                }
                                else
                                { return 0; }
                            });
                            template.Cells[row, j].Value = Math.Round(sum, 2);
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
                    try
                    {
                        SetValue(rule, template, item, dtnum, startRow, row);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        static bool IsNumber(object value)
        {
            if (value is string)
            {
                var str = value as string;

                return System.Text.RegularExpressions.Regex.IsMatch(str, @"^\d+$");
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

        static List<List<DeilverSheetRule>> GetRules(List<DeilverSheetRule> rules)
        {
            List<List<DeilverSheetRule>> results = new List<List<DeilverSheetRule>>();

            var main = rules.Where(x => x.类型1 == "主值").ToList();

            for (int i = 0; i < main.Count; i++)
            {
                if (i == main.Count - 1)
                {
                    if (main[i].序号 == rules.Last().序号)
                    {
                        results.Add(new List<DeilverSheetRule>() { main[i] });
                    }
                    else
                    {
                        var tmp = new List<DeilverSheetRule>();
                        for (int j = rules.IndexOf(main[i]); j < rules.Count; j++)
                        {
                            tmp.Add(rules[j]);
                        }
                        results.Add(tmp);
                    }
                }
                else
                {
                    var tmp = new List<DeilverSheetRule>();
                    for (int j = rules.IndexOf(main[i]); j < rules.IndexOf(main[i + 1]); j++)
                    {
                        tmp.Add(rules[j]);
                    }
                    results.Add(tmp);
                }
            }

            return results;
        }

        static void SetValue(List<DeilverSheetRule> rules, ExcelWorksheet excelWorksheet, Group group, string dtNum, int headRow, int row)
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
                        var range = GetRange(excelWorksheet, row, headRow, rule.赋值字段);

                        if (range != null)
                        {
                            double d = 0;
                            double.TryParse(dtNum, out d);
                            var s = GetRange(excelWorksheet, row, headRow, "单套")?.Text;
                            double t = 0;
                            double.TryParse(s, out t);
                            range.Value = d * t;
                        }
                    }
                    else if (rule.赋值类型 == "字段")
                    {
                        var range = GetRange(excelWorksheet, row, headRow, rule.赋值字段);

                        if (range != null)
                        {
                            if (rule.赋值字段 == "单套")
                            {
                                double sum = 0;

                                foreach (var data in group.originDatas)
                                {
                                    var s = data.Datas.ContainsKey(rule.赋值) ? data.Datas[rule.赋值] : "0";

                                    var d = double.Parse(s);

                                    sum += d;
                                }

                                range.Value = sum;
                            }
                            else
                            {
                                var trange = GetRange(excelWorksheet, row, headRow, rule.赋值);

                                if (trange != null)
                                    range.Value = trange.Value;
                            }
                        }
                    }
                }
            }
            else
            {
                bool flag = false;
                foreach (var rule in rules)
                {
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
                        var range = GetRange(excelWorksheet, row, headRow, rule.赋值字段);

                        if (range != null)
                        {
                            var s = GetRange(excelWorksheet, row, headRow, "单套")?.Text;
                            double t = 0;
                            double.TryParse(s, out t);

                            double d = 0;
                            double.TryParse(group.长, out d);


                            if (rule.赋值.Contains("/"))
                                range.Value = t / d;
                            else if (rule.赋值.Contains("*"))
                            {
                                range.Value = t * d;
                            }
                        }
                    }
                    else if (rule.赋值类型 == "字段")
                    {
                        var range = GetRange(excelWorksheet, row, headRow, rule.赋值字段);

                        if (range != null)
                        {
                            if (rule.赋值字段 == "单套")
                            {
                                double sum = 0;

                                foreach (var data in group.originDatas)
                                {
                                    var s = data.Datas.ContainsKey(rule.赋值) ? data.Datas[rule.赋值] : "0";
                                    var d = double.Parse(s);
                                    sum += d;
                                }

                                range.Value = sum;
                            }
                            else
                            {
                                var trange = GetRange(excelWorksheet, row, headRow, rule.赋值);

                                if (trange != null)
                                    range.Value = trange.Value;
                            }
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

    class Group
    {
        internal string 物料名称;

        internal string 物料编码;

        internal string 长;

        internal string 宽;

        internal string 高;

        internal List<OriginData> originDatas;

        internal Group()
        {
            originDatas = new List<OriginData>();
        }

        internal static List<Group> Get(List<OriginData> datas, List<DeilverMaterial> merageRules)
        {
            List<Group> groups = new List<Group>();

            var tmps = datas.GroupBy(x => new
            {
                a = x.Datas.ContainsKey("物料名称") ? x.Datas["物料名称"] : string.Empty,
                b = x.Datas.ContainsKey("物料编码") ? x.Datas["物料编码"] : string.Empty,
                c = x.Datas.ContainsKey("物料-长") ? x.Datas["物料-长"] : string.Empty,
                d = x.Datas.ContainsKey("物料-宽") ? x.Datas["物料-宽"] : string.Empty,
                e = x.Datas.ContainsKey("物料-高") ? x.Datas["物料-高"] : string.Empty,
            });

            var rules = merageRules.Select(x => x.物料名称).ToList();

            foreach (var tmp in tmps)
            {
                if (rules.Contains(tmp.Key.a))
                {
                    foreach (var item in tmp)
                    {
                        groups.Add(new Group()
                        {
                            物料名称 = tmp.Key.a,
                            物料编码 = tmp.Key.b,
                            长 = tmp.Key.c,
                            宽 = tmp.Key.d,
                            高 = tmp.Key.e,
                            originDatas = new List<OriginData>() { item },
                        });
                    }
                }
                else
                {
                    var group = new Group
                    {
                        物料名称 = tmp.Key.a,
                        物料编码 = tmp.Key.b,
                        长 = tmp.Key.c,
                        宽 = tmp.Key.d,
                        高 = tmp.Key.e
                    };

                    foreach (var item in tmp)
                    {
                        group.originDatas.Add(item);
                    }
                    groups.Add(group);
                }
            }


            return groups;
        }
    }
}
