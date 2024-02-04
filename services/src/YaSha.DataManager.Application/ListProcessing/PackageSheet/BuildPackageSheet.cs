using YaSha.DataManager.ListProcessing;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using YaSha.DataManager.ListProcessing.Base;
using YaSha.DataManager.ListProcessing.CommonModel;

namespace YaSha.DataManager.ListProcessing.PackageSheet
{
    public class BuildPackageSheet : ListProcessingBuildBase
    {
        string dt;

        string type;

        internal override string TableName { get => "包装单"; set => TableName = value; }

        public BuildPackageSheet(BuildFactory factory) : base(factory)
        {
            order = 16;
            dt = factory.dt;
            type = factory.sheetType;
        }

        internal override ExcelWorksheet ProcessExcel(ExcelWorksheets sheets)
        {
            //原始数据
            var originDatas = OriginData.Get(sheets, config);

            //过滤规则
            var filterRules = PackSheetFilterRule.Get(rulePath);

            //过滤后的数据
            var filterDatas = PackSheetFilterRule.Filter(originDatas, filterRules);

            //排序规则
            var sortRules = PackSheetSortRule.Get(rulePath);

            //排序后的数据
            var sortDatas = PackSheetSortRule.Sort(filterDatas, sortRules);

            //修改规则
            var eilverRule = PackSheetRule.Get(rulePath);


            AddPackSheetDatas(config, sortDatas, eilverRule, sheetName, dt, type);

            return config.Sheet;
        }

        static void AddPackSheetDatas(
          SheetConfig config,
          List<OriginData> originDatas,
          List<PackSheetRule> rules,
          string fileName,
          string dtnum,
           string type_1
           )
        {
            var template = config.Sheet;
            var startRow = config.SheetStartRow;
            string headName = fileName.Replace("内加工清单", "") + $"——包装合计[{dtnum}]套";
            template.Cells[1, 3].Value = headName;
            template.Cells[1, 3].Style.Font.Size = 20;

            var groupRules = GetRules(rules);

            int i = 0;

            foreach (var item in originDatas)
            {
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
                        else
                        {
                            if (item.Datas.ContainsKey(head))
                            {
                                var obj = item.Datas[head];

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
                    SetValue(rule, template, dtnum, startRow, row);
                }
            }

            var roomGroups = originDatas.GroupBy(x => x.Datas["房间"]).ToList();
            int roomcell = 1;
            for (int cell = 1; cell < template.Dimension.End.Column; cell++)
            {
                var value = template.Cells[startRow, cell].Text;
                if (value == "合计")
                {
                    roomcell = cell;
                    break;
                }
            }

            int s = startRow;

            foreach (var item in roomGroups)
            {
                int e = s + item.Count();

                try
                {
                    template.Cells[s + 1, roomcell, e, roomcell].Merge = true;
                    template.Cells[s + 1, roomcell, e, roomcell].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    template.Cells[s + 1, roomcell, e, roomcell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    template.Cells[s + 1, roomcell, e, roomcell].Style.WrapText = true;
                    template.Cells[s + 1, roomcell, e, roomcell].Value = "合计：" + item.Sum(x => {
                        if (x.Datas.ContainsKey("单套") && IsNumber(x.Datas["单套"]))
                        {
                            double v = 0;
                            double.TryParse(x.Datas["单套"], out v);
                            return v;
                        }
                        else
                        {
                            return 0;
                        }
                    });
                }
                catch (Exception ex)
                {
                    s = e;
                }

                s = e;
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

        static List<List<PackSheetRule>> GetRules(List<PackSheetRule> rules)
        {
            List<List<PackSheetRule>> results = new List<List<PackSheetRule>>();

            var main = rules.Where(x => x.类型1 == "主值").ToList();

            for (int i = 0; i < main.Count; i++)
            {
                if (i == main.Count - 1)
                {
                    if (main[i].序号 == rules.Last().序号)
                    {
                        results.Add(new List<PackSheetRule>() { main[i] });
                    }
                    else
                    {
                        var tmp = new List<PackSheetRule>();
                        for (int j = rules.IndexOf(main[i]); j < rules.Count; j++)
                        {
                            tmp.Add(rules[j]);
                        }
                        results.Add(tmp);
                    }
                }
                else
                {
                    var tmp = new List<PackSheetRule>();
                    for (int j = rules.IndexOf(main[i]); j < rules.IndexOf(main[i + 1]); j++)
                    {
                        tmp.Add(rules[j]);
                    }
                    results.Add(tmp);
                }
            }

            return results;
        }

        static void SetValue(List<PackSheetRule> rules, ExcelWorksheet excelWorksheet, string dtNum, int headRow, int row)
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
                    else if (rule.赋值类型 == "字段")
                    {

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
                    else if (rule.赋值类型 == "字段")
                    {

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
