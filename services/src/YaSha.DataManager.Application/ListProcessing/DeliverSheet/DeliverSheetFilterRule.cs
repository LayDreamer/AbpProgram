﻿using OfficeOpenXml;
using YaSha.DataManager.ListProcessing.CommonModel;

namespace YaSha.DataManager.ListProcessing.DeliverSheet
{
    internal class DeliverSheetFilterRule
    {
        internal string 序号 { get; set; }

        internal string 类型1 { get; set; }

        internal string 字段 { get; set; }

        internal string 算法 { get; set; }

        internal string 值 { get; set; }

        internal static List<DeliverSheetFilterRule> Get(string path)
        {
            List<DeliverSheetFilterRule> res = new List<DeliverSheetFilterRule>();

            using (ExcelPackage package = new ExcelPackage(path))
            {
                var worksheet = package.Workbook.Worksheets["发货五金-过滤设置"];

                int rows = worksheet.Dimension.End.Row;

                int cols = worksheet.Dimension.End.Column;

                for (int row = 2; row <= rows; row++)
                {
                    var data = new DeliverSheetFilterRule();

                    var value = worksheet.Cells[row, 1].Text;

                    if (string.IsNullOrEmpty(value))
                    {
                        break;
                    }
                    data.序号 = worksheet.Cells[row, 1].Text;

                    data.类型1 = worksheet.Cells[row, 2].Text;

                    data.字段 = worksheet.Cells[row, 3].Text;

                    data.算法 = worksheet.Cells[row, 4].Text;

                    data.值 = worksheet.Cells[row, 6].Text;

                    res.Add(data);
                }
            }
            return res;
        }

        internal static List<OriginData> Filter(List<OriginData> originDatas, List<DeliverSheetFilterRule> filterRules)
        {
            List<OriginData> filterDatas = new List<OriginData>();

            var groups = GetRules(filterRules);

            foreach (var item in originDatas)
            {
                bool flag = true;

                foreach (var group in groups)
                {
                    bool next = true;

                    foreach (var rule in filterRules)
                    {
                        if (rule.算法 == "包含")
                        {
                            var v = item.Datas.ContainsKey(rule.字段) ? item.Datas[rule.字段] : string.Empty;

                            if (v.Contains(rule.值))
                            {
                                if (rule.类型1 == "主值" || rule.类型1 == "且")
                                    next &= true;
                                else if (rule.类型1 == "或")
                                    next |= true;
                            }
                            else
                            {
                                if (rule.类型1 == "主值" || rule.类型1 == "且")
                                    next &= false;
                                else if (rule.类型1 == "或")
                                    next |= false;
                            }
                        }
                        else if (rule.算法 == "不包含")
                        {
                            var v = item.Datas.ContainsKey(rule.字段) ? item.Datas[rule.字段] : string.Empty;

                            if (!v.Contains(rule.值))
                            {
                                if (rule.类型1 == "主值" || rule.类型1 == "且")
                                    next &= true;
                                else if (rule.类型1 == "或")
                                    next |= true;
                            }
                            else
                            {
                                if (rule.类型1 == "主值" || rule.类型1 == "且")
                                    next &= false;
                                else if (rule.类型1 == "或")
                                    next |= false;
                            }
                        }
                        else
                        {

                        }
                    }

                    flag &= next;
                }

                if (flag)
                {
                    filterDatas.Add(item);
                }
                else
                {

                }
            }


            return filterDatas;
        }

        static List<List<DeliverSheetFilterRule>> GetRules(List<DeliverSheetFilterRule> rules)
        {
            List<List<DeliverSheetFilterRule>> results = new List<List<DeliverSheetFilterRule>>();

            var main = rules.Where(x => x.类型1 == "主值").ToList();

            for (int i = 0; i < main.Count; i++)
            {
                if (i == main.Count - 1)
                {
                    if (main[i].序号 == rules.Last().序号)
                    {
                        results.Add(new List<DeliverSheetFilterRule>() { main[i] });
                    }
                    else
                    {
                        var tmp = new List<DeliverSheetFilterRule>();
                        for (int j = rules.IndexOf(main[i]); j < rules.Count; j++)
                        {
                            tmp.Add(rules[j]);
                        }
                        results.Add(tmp);
                    }
                }
                else
                {
                    var tmp = new List<DeliverSheetFilterRule>();
                    for (int j = rules.IndexOf(main[i]); j < rules.IndexOf(main[i + 1]); j++)
                    {
                        tmp.Add(rules[j]);
                    }
                    results.Add(tmp);
                }
            }

            return results;
        }
    }

    internal class DeilverSheetSortRule
    {
        internal string 字段 { get; set; }

        internal string 排序规则 { get; set; }

        internal static List<DeilverSheetSortRule> Get(string path)
        {
            List<DeilverSheetSortRule> res = new List<DeilverSheetSortRule>();

            using (ExcelPackage package = new ExcelPackage(path))
            {
                var worksheet = package.Workbook.Worksheets["发货五金-排序设置"];

                int rows = worksheet.Dimension.End.Row;

                int cols = worksheet.Dimension.End.Column;

                for (int row = 2; row <= rows; row++)
                {
                    var data = new DeilverSheetSortRule();

                    var value = worksheet.Cells[row, 1].Text;

                    if (string.IsNullOrEmpty(value))
                    {
                        break;
                    }
                    data.字段 = worksheet.Cells[row, 3].Text;

                    data.排序规则 = worksheet.Cells[row, 7].Text;

                    res.Add(data);
                }
            }
            return res;
        }


        internal static List<OriginData> Sort(List<OriginData> datas, List<DeilverSheetSortRule> rules)
        {
            IOrderedEnumerable<OriginData> tmp = null;

            if (rules[0].排序规则 == "升序")
            {
                tmp = datas.OrderBy(x =>
                {
                    if (x.Datas.ContainsKey(rules[0].字段))
                        return x.Datas[rules[0].字段];
                    else
                        return string.Empty;
                });
            }
            else if (rules[0].排序规则 == "降序")
            {
                tmp = datas.OrderByDescending(x =>
                {
                    if (x.Datas.ContainsKey(rules[0].字段))
                        return x.Datas[rules[0].字段];
                    else
                        return string.Empty;
                });
            }

            for (int i = 1; i < rules.Count; i++)
            {
                if (rules[i].排序规则 == "升序")
                {
                    tmp = tmp.ThenBy(x =>
                    {
                        if (x.Datas.ContainsKey(rules[i].字段))
                            return x.Datas[rules[i].字段];
                        else
                            return string.Empty;
                    });
                }
                else if (rules[i].排序规则 == "降序")
                {
                    tmp = tmp.ThenByDescending(x =>
                    {
                        if (x.Datas.ContainsKey(rules[i].字段))
                            return x.Datas[rules[i].字段];
                        else
                            return string.Empty;
                    });
                }
            }
            return tmp.ToList();
        }
    }

    internal class DeilverMaterial
    {
        public string 序号 { get; set; }

        public string 物料名称 { get; set; }

        public static List<DeilverMaterial> Get(string path)
        {
            List<DeilverMaterial> res = new List<DeilverMaterial>();

            using (ExcelPackage package = new ExcelPackage(path))
            {
                var worksheet = package.Workbook.Worksheets["发货五金-不合并物料名称"];

                int rows = worksheet.Dimension.End.Row;

                int cols = worksheet.Dimension.End.Column;

                for (int row = 2; row <= rows; row++)
                {
                    var data = new DeilverMaterial();

                    var value = worksheet.Cells[row, 1].Text;

                    if (string.IsNullOrEmpty(value))
                    {
                        break;
                    }
                    data.序号 = worksheet.Cells[row, 1].Text;

                    data.物料名称 = worksheet.Cells[row, 2].Text;

                    res.Add(data);
                }
            }
            return res;
        }
    }

    internal class DeilverSheetRule
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
        internal string 备注 { get; set; }

        internal static List<DeilverSheetRule> Get(string path)
        {
            List<DeilverSheetRule> res = new List<DeilverSheetRule>();

            using (ExcelPackage package = new ExcelPackage(path))
            {
                var worksheet = package.Workbook.Worksheets["发货五金-修改设置"];

                int rows = worksheet.Dimension.End.Row;

                int cols = worksheet.Dimension.End.Column;

                for (int row = 2; row <= rows; row++)
                {
                    if (worksheet.Cells[row, 1].Text == "")
                    {
                        break;
                    }

                    DeilverSheetRule data = new DeilverSheetRule
                    {
                        序号 = worksheet.Cells[row, 1].Text,
                        类型1 = worksheet.Cells[row, 2].Text,
                        字段 = worksheet.Cells[row, 3].Text,
                        算法 = worksheet.Cells[row, 4].Text,
                        类型2 = worksheet.Cells[row, 5].Text,
                        值 = worksheet.Cells[row, 6].Text,
                        多条件 = worksheet.Cells[row, 7].Text,
                        赋值字段 = worksheet.Cells[row, 8].Text,
                        赋值类型 = worksheet.Cells[row, 9].Text,
                        赋值 = worksheet.Cells[row, 10].Text,
                        备注 = worksheet.Cells[row, 11].Text
                    };

                    res.Add(data);
                }
            }

            return res;
        }

        public override string ToString()
        {
            return this.序号;
        }
    }
}