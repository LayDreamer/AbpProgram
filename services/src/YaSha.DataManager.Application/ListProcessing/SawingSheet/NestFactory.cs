//#define Test

using YaSha.DataManager.ListProcessing.CommonModel;
using YaSha.DataManager.ListProcessing.SawingSheet.SheetModel;

namespace YaSha.DataManager.ListProcessing.SawingSheet
{
    /// <summary>
    /// 数据工厂
    /// </summary>
    internal class NestFactory
    {
        List<OriginData> origindatas;
        List<MaterialSheetData> materialSheetDatas;
        List<NestChangeSheetData> nestChangeSheetDatas;
        List<NestSortSheetData> nestSortSheetDatas;
        List<NestSetSheetData> nestSetSheetDatas;

        internal NestFactory(List<OriginData> origindatas,
            List<MaterialSheetData> materialSheetDatas,
            List<NestChangeSheetData> nestChangeSheetDatas,
            List<NestSortSheetData> nestSortSheetDatas,
            List<NestSetSheetData> nestSetSheetDatas)
        {
            this.origindatas = origindatas;
            this.materialSheetDatas = materialSheetDatas;
            this.nestChangeSheetDatas = nestChangeSheetDatas;
            this.nestSortSheetDatas = nestSortSheetDatas;
            this.nestSetSheetDatas = nestSetSheetDatas;
        }

        /// <summary>
        /// 开料单修改数据  分组  大组排序  原材料  套料设置  计算出套料结果 修改数据先后及合并  组内排序  小组排序 
        /// </summary>
        /// <returns></returns>
        internal List<OutGroups> Start()
        {
#if Test
            string jsonPath = Environment.GetFolderPath(SpecialFolder.Desktop) + "\\" + "e7cf27d2-beb2-4d70-862f-e1dd399771b5" + ".json";
            if (System.IO.File.Exists(jsonPath))
            {
                string data = ListProcessing.Methods.AppMethod.GetTextFromFile(jsonPath);
                var datas = data.Split('\n').ToList();
                datas.RemoveAll(x => x.Length <= 1);
                var tmps = new List<OutGroups>();
                foreach (var item in datas)
                {
                    tmps.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<OutGroups>(item));
                }
                return tmps;
            }
#endif

            SetValueByNestChangeSheetRule(nestChangeSheetDatas, origindatas);

            var allGroups = GetAllOutGroups();

            allGroups = SortOutGroupsByNestSortSheetRule(nestSortSheetDatas, allGroups);

            foreach (var item in allGroups)
            {
                //if (item.Name != "12科耐板28514非标板") continue;
                try
                {
                    item.StartNest(materialSheetDatas, nestSetSheetDatas, nestSortSheetDatas);
                }
                catch(Exception ex)
                {

                }
            }

#if Test
            string msg = string.Empty;
            foreach (var item in allGroups)
            {
                var obj = Newtonsoft.Json.JsonConvert.SerializeObject(item);
                msg += (obj + "\n");
            }
            using (System.IO.StreamWriter output = System.IO.File.CreateText(jsonPath))
            {
                output.WriteLine(msg);
                output.Close();
            }
#endif

            return allGroups;
        }
        List<List<NestChangeSheetData>> GetNestChangeSheetRuleGroups(List<NestChangeSheetData> rules)
        {
            List<List<NestChangeSheetData>> results = new List<List<NestChangeSheetData>>();

            var main = rules.Where(x => x.类型1 == "主值").ToList();

            for (int i = 0; i < main.Count; i++)
            {
                if (i == main.Count - 1)
                {
                    if (main[i].序号 == rules.Last().序号)
                    {
                        results.Add(new List<NestChangeSheetData>() { main[i] });
                    }
                    else
                    {
                        var tmp = new List<NestChangeSheetData>();
                        for (int j = rules.IndexOf(main[i]); j < rules.Count; j++)
                        {
                            tmp.Add(rules[j]);
                        }
                        results.Add(tmp);
                    }
                }
                else
                {
                    var tmp = new List<NestChangeSheetData>();
                    for (int j = rules.IndexOf(main[i]); j < rules.IndexOf(main[i + 1]); j++)
                    {
                        tmp.Add(rules[j]);
                    }
                    results.Add(tmp);
                }
            }

            return results;
        }

        void SetValueByNestChangeSheetRule(List<NestChangeSheetData> rules, OriginData originData)
        {
            if (rules.Count == 1)
            {
                var rule = rules[0];
                var targetValue = originData.Datas[rule.字段];
                var compareValue = string.Empty;
                if (rule.类型2 == "值")
                {
                    compareValue = rule.值;
                }
                else if (rule.类型2 == "字段")
                {
                    compareValue = originData.Datas[rule.值];
                }
                bool next = false;
                if (rule.算法 == "等于")
                {
                    next = targetValue.Equals(compareValue);
                }
                else if (rule.算法 == "不等于")
                {
                    next = !targetValue.Equals(compareValue);
                }
                else if (rule.算法 == "包含")
                {
                    next = targetValue.Contains(compareValue);
                }
                else if (rule.算法 == "不包含")
                {
                    next = !targetValue.Contains(compareValue);
                }
                if (next)
                {
                    if (rule.赋值类型 == "值")
                    {
                        originData.Datas[rule.赋值字段] = rule.赋值;
                    }
                    else if (rule.赋值类型 == "公式")
                    {
                        var lists = rule.赋值.Split(' ').ToList();
                        lists.RemoveAll(x => string.IsNullOrEmpty(x));
                        if (lists.Count == 3)
                        {
                            lists[0] = lists[0].Replace("[", "");
                            lists[0] = lists[0].Replace("]", "");
                            var tmp = originData.Datas[lists[0]];
                            switch (lists[1])
                            {
                                case "+": tmp = (double.Parse(tmp) + double.Parse(lists[2])).ToString(); break;
                                case "-": tmp = (double.Parse(tmp) - double.Parse(lists[2])).ToString(); break;
                                case "*": tmp = (double.Parse(tmp) * double.Parse(lists[2])).ToString(); break;
                                case "/": tmp = (double.Parse(tmp) / double.Parse(lists[2])).ToString(); break;
                            }
                            originData.Datas[rule.赋值字段] = tmp;
                        }
                    }
                    else if (rule.赋值类型 == "字段")
                    {
                        originData.Datas[rule.赋值字段] = originData.Datas[rule.赋值];
                    }
                }
            }
            else
            {
                bool flag = false;
                foreach (var rule in rules)
                {
                    var targetValue = originData.Datas[rule.字段];
                    var compareValue = string.Empty;
                    bool next = false;

                    if (rule.算法 == "等于")
                    {
                        next = targetValue.Equals(compareValue);
                    }
                    else if (rule.算法 == "不等于")
                    {
                        next = !targetValue.Equals(compareValue);
                    }
                    else if (rule.算法 == "包含")
                    {
                        next = targetValue.Contains(compareValue);
                    }
                    else if (rule.算法 == "不包含")
                    {
                        next = !targetValue.Contains(compareValue);
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
                        originData.Datas[rule.赋值字段] = rule.赋值;
                    }
                    else if (rule.赋值类型 == "公式")
                    {
                        var lists = rule.赋值.Split(' ').ToList();
                        lists.RemoveAll(x => string.IsNullOrEmpty(x));
                        if (lists.Count == 3)
                        {
                            lists[0] = lists[0].Replace("[", "");
                            lists[0] = lists[0].Replace("]", "");
                            var tmp = originData.Datas[lists[0]];
                            switch (lists[1])
                            {
                                case "+": tmp = (double.Parse(tmp) + double.Parse(lists[2])).ToString(); break;
                                case "-": tmp = (double.Parse(tmp) - double.Parse(lists[2])).ToString(); break;
                                case "*": tmp = (double.Parse(tmp) * double.Parse(lists[2])).ToString(); break;
                                case "/": tmp = (double.Parse(tmp) / double.Parse(lists[2])).ToString(); break;
                            }
                            originData.Datas[rule.赋值字段] = tmp;
                        }
                    }
                    else if (rule.赋值类型 == "字段")
                    {
                        originData.Datas[rule.赋值字段] = originData.Datas[rule.赋值];
                    }
                }
            }
        }

        void SetValueByNestChangeSheetRule(List<NestChangeSheetData> rules, List<OriginData> origindatas)
        {
            var groups = GetNestChangeSheetRuleGroups(rules);
            foreach (var group in groups)
            {
                foreach (var item in origindatas)
                {
                    try
                    {
                        SetValueByNestChangeSheetRule(group, item);
                    }
                    catch (Exception e)
                    {

                    }
                }
            }
        }

        string GetBoardNameByNestSetSheetRule(List<NestSetSheetData> rules, OriginData originData)
        {
            foreach (var rule in rules)
            {
                if (rule.类型1 != "下拉选项") continue;

                if (originData.Datas["物料名称"].Contains(rule.内容2))
                {
                    return rule.内容2;
                }
            }

            return originData.Datas["物料名称"];
        }

        bool GetBoardStarandByNestSetSheetRule(List<NestSetSheetData> rules, OriginData originData)
        {
            foreach (var rule in rules)
            {
                if (rule.类型1 != "标准规格") continue;
                var targetValue = originData.Datas[rule.类型2];
                var next = false;
                switch (rule.算法)
                {
                    case "等于": next = targetValue.Equals(rule.内容1); break;
                    case "不等于": next = !targetValue.Equals(rule.内容1); break;
                    case "包含": if (rule.内容1 == "所有") next = true; else next = targetValue.Contains(rule.内容1); break;
                    case "不包含": if (rule.内容1 != "所有") next = !targetValue.Contains(rule.内容1); break;
                }
                if (next)
                {
                    var value = rule.内容2.Split('、');
                    var tag = originData.Datas["展开面"];
                    if (value.Contains(tag))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        List<OutGroups> GetAllOutGroups()
        {
            var special = origindatas.Where(x => x.Datas["物料名称"].Contains("科耐板") && x.Datas["型号"].Contains("毛料")).ToList();
            var normal = origindatas.Where(x => !special.Contains(x)).ToList();
            var groups = normal.GroupBy(x =>
            {
                var str = GetBoardNameByNestSetSheetRule(nestSetSheetDatas, x) + x.Datas["型号"];
                var starand = GetBoardStarandByNestSetSheetRule(nestSetSheetDatas, x);
                if (starand)
                {
                    str += "标准板";
                }
                else
                {
                    str += "非标板";
                }
                return str;
            }).ToList();

            List<OutGroups> datas = new List<OutGroups>();

            foreach (var group in groups)
            {
                var tmp = new List<OriginData>();
                foreach (var item in group)
                {
                    tmp.Add(item);
                }
                datas.Add(new OutGroups(GetBoardNameByNestSetSheetRule(nestSetSheetDatas, tmp[0]), group.Key, tmp));
            }

            //特殊情况 科耐板型号为毛料 归类到 木塑板中 放集合底部
            foreach (var item in datas)
            {
                if (item.Name.Contains("木塑"))
                {
                    item.OriginDatas.AddRange(special);
                    break;
                }
            }

            return datas;
        }

        List<List<NestSortSheetData>> GetNestSortSheetRuleGroups(List<NestSortSheetData> rules)
        {
            List<List<NestSortSheetData>> results = new List<List<NestSortSheetData>>();

            var main = rules.Where(x => x.类型 == "主值" && !string.IsNullOrEmpty(x.规则)).ToList();

            for (int i = 0; i < main.Count; i++)
            {
                if (i == main.Count - 1)
                {
                    if (main[i].优先级 == rules.Last().优先级)
                    {
                        results.Add(new List<NestSortSheetData>() { main[i] });
                    }
                    else
                    {
                        var tmp = new List<NestSortSheetData>();
                        for (int j = rules.IndexOf(main[i]); j < rules.Count; j++)
                        {
                            tmp.Add(rules[j]);
                        }
                        results.Add(tmp);
                    }
                }
                else
                {
                    var tmp = new List<NestSortSheetData>();
                    for (int j = rules.IndexOf(main[i]); j < rules.IndexOf(main[i + 1]); j++)
                    {
                        tmp.Add(rules[j]);
                    }
                    results.Add(tmp);
                }
            }

            return results;
        }

        List<OutGroups> SortOutGroupsByNestSortSheetRule(List<NestSortSheetData> nestSortSheetDatas, List<OutGroups> allGroups)
        {
            var results = new List<OutGroups>();

            var ruleGroups = GetNestSortSheetRuleGroups(nestSortSheetDatas);

            //按规则优先级分组物料名称不同
            List<List<OutGroups>> sameMaterialNameGroup = new List<List<OutGroups>>();

            foreach (var ruleGroup in ruleGroups)
            {
                var tmps = new List<OutGroups>();
                foreach (var item in allGroups)
                {
                    var next = false;
                    foreach (var rule in ruleGroup)
                    {
                        if (!string.IsNullOrEmpty(rule.排序规则) && rule.类型 != "主值")
                        {
                            continue;
                        }
                        var targetName = item.OriginDatas[0].Datas[rule.字段];
                        var compareName = rule.内容;

                        var ruleFlag = false;

                        switch (rule.规则)
                        {
                            case "等于": ruleFlag = targetName.Equals(compareName); break;
                            case "不等于": ruleFlag = !targetName.Equals(compareName); break;
                            case "包含": ruleFlag = targetName.Contains(compareName); break;
                            case "不包含": ruleFlag = !targetName.Contains(compareName); break;
                        }

                        if (rule.类型 == "主值")
                        {
                            next |= ruleFlag;
                            if (!next)
                            {
                                break;
                            }
                        }
                        else if (rule.类型 == "且")
                        {
                            next &= ruleFlag;
                        }
                        else if (rule.类型 == "或")
                        {
                            next |= ruleFlag;
                        }
                    }
                    if (next)
                    {
                        tmps.Add(item);
                        item.SetNestSortSheetRules(ruleGroup);
                    }
                }
                if (tmps.Count > 0)
                {
                    if (string.IsNullOrEmpty(ruleGroup[0].排序规则) || ruleGroup[0].排序规则 == "升序")
                    {
                        sameMaterialNameGroup.Add(tmps.OrderBy(x => x.Name).ToList());
                    }
                    else
                    {
                        sameMaterialNameGroup.Add(tmps.OrderByDescending(x => x.Name).ToList());
                    }
                }
            }

            foreach (var item in sameMaterialNameGroup)
            {
                results.AddRange(item);
            }

            return results;
        }
    }

    [Serializable]
    public class OutGroups
    {
        public string Material { get; set; }

        public string Name { get; set; }

        internal List<OriginData> OriginDatas { get; set; }

        public List<ExcelResult> ExcelResults { get; set; }

        internal double ProcessBoardArea
        {
            get
            {
                double area = 0;
                foreach (var item in ExcelResults)
                {
                    area += item.ProceBoardArea;
                }
                return area;
            }
        }

        internal double MaterialArea
        {
            get
            {
                double area = 0;
                foreach (var item in ExcelResults)
                {
                    area += item.StarandArea;
                }
                return area;
            }
        }

        private List<NestSortSheetData> rules;

        private bool setRule = false;

        public OutGroups()
        {
            rules = new List<NestSortSheetData>();
        }

        internal OutGroups(string materialName, string name, List<OriginData> originDatas)
        {
            Material = materialName;
            Name = name;
            OriginDatas = originDatas;
            ExcelResults = new List<ExcelResult>();
            rules = new List<NestSortSheetData>();
        }

        internal void StartNest(List<MaterialSheetData> materialSheetDatas, List<NestSetSheetData> nestSetSheetDatas, List<NestSortSheetData> nestSortSheetDatas)
        {
            var cutLossRules = nestSetSheetDatas.Where(x => x.类型1 == "刀损").ToList();
            var boardLossRules = nestSetSheetDatas.Where(x => x.类型1 == "板材损耗").ToList();
            var cutCountRules = nestSetSheetDatas.Where(x => x.类型1 == "切割数量").ToList();
            var starandLossRules = nestSetSheetDatas.Where(x => x.类型1 == "标准板块计算").ToList();
            var usedMaterials = materialSheetDatas.Where(x => this.Name.Contains(x.材料名称)).ToList();
            if (0 == usedMaterials.Count)
            {
                foreach (var item in OriginDatas)
                {
                    ExcelResults.Add(new ExcelResult(item));
                }

                SortByNestSortRules();
                return;
            }
            Dictionary<int, OriginData> boardKeys = new Dictionary<int, OriginData>();
            Dictionary<int, MaterialSheetData> materialKeys;
            //原材料
            List<MaterialGroup> materialGroups = GetLossMaterialGroupByNestSetRule(usedMaterials, boardLossRules, cutCountRules, out materialKeys);
            //加工板
            List<BoardGroup> allBoards = new List<BoardGroup>();
            foreach (var item in OriginDatas)
            {
                var cutloss = GetBoardLossByNestSetRules(cutLossRules, item);
                BoardGroup boardGroup = new BoardGroup(Guid.NewGuid().GetHashCode(),
                    double.Parse(item.Datas["物料-长"]), double.Parse(item.Datas["展开面"]),
                    double.Parse(item.Datas["批量"]), cutloss);
                allBoards.Add(boardGroup);
                boardKeys.Add(boardGroup.GetGroupId(), item);
            }

            Nest nest = new Nest(materialGroups, allBoards);
            var bests = nest.Start();
            foreach (var best in bests)
            {
                var boards = new List<OriginData>();
                foreach (var item in best.boardGroups)
                {
                    boards.Add(boardKeys[item.GetGroupId()]);
                }
                var materials = new List<MaterialSheetData>();
                var groups = best.materials.GroupBy(x => x.GetParentId()).ToList();
                foreach (var item in groups)
                {
                    materials.Add(new MaterialSheetData(materialKeys[item.Key], item.Count()));
                }
                materials = materials.OrderBy(x => double.Parse(x.宽度)).ThenBy(x => double.Parse(x.长度)).ToList();
                ExcelResults.Add(new ExcelResult(boards, materials));
            }
            CombineSameSizeExcelResult();
            foreach (var item in ExcelResults)
            {
                item.Calcute(starandLossRules);
                item.Sort(rules);
            }
            SortByNestSortRules();
        }

        internal void SetNestSortSheetRules(List<NestSortSheetData> datas)
        {
            if (!setRule)
            {
                this.rules = datas;
                setRule = true;
            }
        }

        int GetBoardLossByNestSetRules(List<NestSetSheetData> rules, OriginData origin)
        {
            foreach (var rule in rules)
            {
                string target = origin.Datas[rule.类型2];
                string compare = rule.内容1;
                bool next = rule.Compare(target, compare);
                //switch (rule.算法)
                //{
                //    case "等于": next = target.Equals(compare); break;
                //    case "不等于": next = !target.Equals(compare); break;
                //    case "包含": next = target.Contains(compare); break;
                //    case "不包含": next = !target.Contains(compare); break;
                //}
                if (next)
                {
                    int d = 0;
                    if (rule.内容2.Contains("mm"))
                    {
                        d = int.Parse(rule.内容2.Replace("mm", ""));
                    }
                    else if (rule.内容2.Contains("cm"))
                    {
                        d = int.Parse(rule.内容2.Replace("cm", "")) * 10;
                    }
                    else if (rule.内容2.Contains("m"))
                    {
                        d = int.Parse(rule.内容2.Replace("m", "")) * 1000;
                    }
                    return d;
                }
            }
            return 0;
        }

        List<MaterialGroup> GetLossMaterialGroupByNestSetRule(List<MaterialSheetData> materialSheetDatas, List<NestSetSheetData> lossrules, List<NestSetSheetData> cutcoutRules, out Dictionary<int, MaterialSheetData> keys)
        {
            List<MaterialGroup> materialGroups = new List<MaterialGroup>();
            keys = new Dictionary<int, MaterialSheetData>();
            foreach (var item in materialSheetDatas)
            {
                var type = item.GetType();
                var l = double.Parse(item.长度);
                var w = double.Parse(item.宽度);
                Dictionary<string, int> maxcutCount = new Dictionary<string, int>()
                {
                    {"宽度",9999 },
                    {"长度",9999 },
                };
                foreach (var rule in lossrules)
                {
                    string target = string.Empty;
                    string compare = rule.内容1;
                    if (rule.类型2.Contains("原材料"))
                    {
                        if (rule.类型2.Contains("宽度"))
                        {
                            target = item.宽度;
                        }
                        else if (rule.类型2.Contains("长度"))
                        {
                            target = item.长度;
                        }
                    }
                    else
                    {
                        target = this.OriginDatas[0].Datas[rule.类型2];
                    }
                    bool next = rule.Compare(target, compare);
                    //switch (rule.算法)
                    //{
                    //    case "等于": next = target.Equals(compare); break;
                    //    case "不等于": next = !target.Equals(compare); break;
                    //    case "包含": next = target.Contains(compare); break;
                    //    case "不包含": next = !target.Contains(compare); break;
                    //}
                    if (next)
                    {
                        int d = 0;
                        if (rule.内容2.Contains("mm"))
                        {
                            d = int.Parse(rule.内容2.Replace("mm", ""));
                        }
                        else if (rule.内容2.Contains("cm"))
                        {
                            d = int.Parse(rule.内容2.Replace("cm", "")) * 10;
                        }
                        else if (rule.内容2.Contains("m"))
                        {
                            d = int.Parse(rule.内容2.Replace("m", "")) * 1000;
                        }

                        if (rule.标题 == "长度")
                        {
                            l -= d;
                        }
                        else if (rule.标题 == "宽度")
                        {
                            w -= d;
                        }
                    }
                }
                foreach (var rule in cutcoutRules)
                {
                    string target = string.Empty;
                    string compare = rule.内容1;
                    if (rule.类型2.Contains("原材料"))
                    {
                        var split = rule.类型2.Split('-');
                        target = type?.GetProperties().FirstOrDefault(x => x.Name == split[1])?.GetValue(item) as string;
                    }
                    else
                    {
                        target = this.OriginDatas[0].Datas[rule.类型2];
                    }
                    bool next = rule.Compare(target, compare);
                    //switch (rule.算法)
                    //{
                    //    case "等于": next = target.Equals(compare); break;
                    //    case "不等于": next = !target.Equals(compare); break;
                    //    case "包含": next = target.Contains(compare); break;
                    //    case "不包含": next = !target.Contains(compare); break;
                    //}
                    if (next)
                    {
                        if (maxcutCount.ContainsKey(rule.标题))
                            maxcutCount[rule.标题] = int.Parse(rule.内容2);
                    }
                }

                var material = new MaterialGroup(Guid.NewGuid().GetHashCode(), l, w, int.Parse(item.数量), maxcutCount);
                materialGroups.Add(material);
                keys.Add(material.GetId(), item);
            }

            return materialGroups;
        }

        void CombineSameSizeExcelResult()
        {
            if (!this.Name.Contains("标准板"))
            {
                return;
            }
            var groups = this.ExcelResults.GroupBy(x =>
            {
                string str = string.Empty;
                if (0 == x.Materials.Count())
                {
                    return str;
                }
                var tmps = x.Materials.Select(y => y.GetSize()).Distinct().ToList();
                if (tmps.Count > 1)
                {
                    return str;
                }
                foreach (var item in tmps)
                {
                    str += item;
                }
                return str;
            });

            foreach (var group in groups)
            {
                if (string.IsNullOrEmpty(group.Key)) continue;
                List<ExcelResult> tmps = new List<ExcelResult>();
                foreach (var item in group)
                {
                    tmps.Add(item);
                }
                var boards = new List<OriginData>();
                var allmaterials = new List<MaterialSheetData>();
                foreach (var item in tmps)
                {
                    boards.AddRange(item.OriginDatas);
                    allmaterials.AddRange(item.Materials);
                }
                var materialGroups = allmaterials.GroupBy(x => new { l = x.长度, w = x.宽度 });
                var materials = new List<MaterialSheetData>();
                foreach (var materialgroup in materialGroups)
                {
                    materials.Add(new MaterialSheetData(materialgroup.First(), (int)(materialgroup.Sum(x => double.Parse(x.数量)))));
                }
                this.ExcelResults.RemoveAll(x => tmps.Contains(x));
                this.ExcelResults.Add(new ExcelResult(boards, materials));
            }
        }

        void SortByNestSortRules()
        {
            bool first = true;

            IOrderedEnumerable<ExcelResult> orders = null;

            foreach (var item in rules)
            {
                if (!string.IsNullOrEmpty(item.规则)) continue;

                if (first)
                {
                    first = false;

                    if (item.排序规则 == "升序")
                    {
                        orders = this.ExcelResults.OrderBy(x =>
                        {
                            if (x.OriginDatas[0].Datas.ContainsKey(item.字段))
                            {
                                double tmp = 0;
                                double.TryParse(x.OriginDatas[0].Datas[item.字段], out tmp);
                                return tmp;
                            }
                            return -1;
                        });
                    }
                    else if (item.排序规则 == "降序")
                    {
                        orders = this.ExcelResults.OrderByDescending(x =>
                        {
                            if (x.OriginDatas[0].Datas.ContainsKey(item.字段))
                            {
                                double tmp = 0;
                                double.TryParse(x.OriginDatas[0].Datas[item.字段], out tmp);
                                return tmp;
                            }
                            return -1;
                        });
                    }
                }
                else
                {
                    if (item.排序规则 == "升序")
                    {
                        orders = orders.ThenBy(x =>
                        {
                            if (x.OriginDatas[0].Datas.ContainsKey(item.字段))
                            {
                                double tmp = 0;
                                double.TryParse(x.OriginDatas[0].Datas[item.字段], out tmp);
                                return tmp;
                            }
                            return -1;
                        });
                    }
                    else if (item.排序规则 == "降序")
                    {
                        orders = orders.ThenByDescending(x =>
                        {
                            if (x.OriginDatas[0].Datas.ContainsKey(item.字段))
                            {
                                double tmp = 0;
                                double.TryParse(x.OriginDatas[0].Datas[item.字段], out tmp);
                                return tmp;
                            }
                            return -1;
                        });
                    }
                }
            }

            if (orders != null)
            {
                this.ExcelResults = orders.ToList();

                //无效的放底部
                var invalid = this.ExcelResults.Where(x => x.Materials.Count() == 0).ToList();

                this.ExcelResults.RemoveAll(x => invalid.Contains(x));

                this.ExcelResults.AddRange(invalid);
            }
        }

        public override string ToString()
        {
            return this.Name;
        }
    }

    [Serializable]
    public class ExcelResult
    {
        public List<OriginData> OriginDatas { get; set; }

        public List<MaterialSheetData> Materials { get; set; }

        public double StarandArea { get; set; }

        internal double ProceBoardArea
        {
            get
            {
                double area = 0;
                if (Materials.Count == 0) return area;
                foreach (var item in this.OriginDatas)
                {
                    area += (1.0 * double.Parse(item.Datas["物料-长"]) / 1e3 * double.Parse(item.Datas["展开面"]) / 1e3 *
                    double.Parse(item.Datas["批量"]));
                }
                return area;
            }
        }

        public ExcelResult()
        {
            this.OriginDatas = new List<OriginData>();
            this.Materials = new List<MaterialSheetData>();
        }

        internal ExcelResult(List<OriginData> originDatas, List<MaterialSheetData> materials)
        {
            OriginDatas = originDatas;
            Materials = materials.OrderBy(x => double.Parse(x.长度)).ThenBy(x => double.Parse(x.宽度)).ToList();
        }

        internal ExcelResult(OriginData origin)
        {
            this.OriginDatas = new List<OriginData>();
            this.Materials = new List<MaterialSheetData>();
            this.OriginDatas.Add(origin);
        }

        internal void Calcute(List<NestSetSheetData> rules)
        {
            foreach (var materials in this.Materials)
            {
                var l = double.Parse(materials.长度);
                var w = double.Parse(materials.宽度);
                var c = double.Parse(materials.数量);
                var type = materials.GetType();
                foreach (var rule in rules)
                {
                    var target = type.GetProperties().FirstOrDefault(x => x.Name == rule.类型2)?.GetValue(materials) as string;
                    bool next = rule.Compare(target, rule.内容1);
                    if (next)
                    {
                        if (rule.标题.Contains("长度"))
                        {
                            var str = rule.内容2.Replace("长度", "");
                            if (!string.IsNullOrEmpty(str))
                            {
                                if (str.Contains("-"))
                                {
                                    str = str.Replace("-", "");
                                    if (!string.IsNullOrEmpty(str))
                                    {
                                        int p = int.Parse(str);
                                        l -= p;
                                    }
                                }
                                else if (str.Contains("+"))
                                {
                                    str = str.Replace("+", "");
                                    if (!string.IsNullOrEmpty(str))
                                    {
                                        int p = int.Parse(str);
                                        l += p;
                                    }
                                }
                                else if (str.Contains("*"))
                                {
                                    str = str.Replace("*", "");
                                    if (!string.IsNullOrEmpty(str))
                                    {
                                        int p = int.Parse(str);
                                        l *= p;
                                    }
                                }
                                else if (str.Contains("/"))
                                {
                                    str = str.Replace("/", "");
                                    if (!string.IsNullOrEmpty(str))
                                    {
                                        int p = int.Parse(str);
                                        l /= p;
                                    }
                                }
                            }
                        }
                        else if (rule.标题.Contains("宽度"))
                        {
                            var str = rule.内容2.Replace("宽度", "");
                            if (!string.IsNullOrEmpty(str))
                            {
                                if (str.Contains("-"))
                                {
                                    str = str.Replace("-", "");
                                    if (!string.IsNullOrEmpty(str))
                                    {
                                        int p = int.Parse(str);
                                        l -= p;
                                    }
                                }
                                else if (str.Contains("+"))
                                {
                                    str = str.Replace("+", "");
                                    if (!string.IsNullOrEmpty(str))
                                    {
                                        int p = int.Parse(str);
                                        l += p;
                                    }
                                }
                                else if (str.Contains("*"))
                                {
                                    str = str.Replace("*", "");
                                    if (!string.IsNullOrEmpty(str))
                                    {
                                        int p = int.Parse(str);
                                        l *= p;
                                    }
                                }
                                else if (str.Contains("/"))
                                {
                                    str = str.Replace("/", "");
                                    if (!string.IsNullOrEmpty(str))
                                    {
                                        int p = int.Parse(str);
                                        l /= p;
                                    }
                                }
                            }
                        }
                    }
                }
                this.StarandArea += (1.0 * l / 1e3 * w / 1e3 * c);
                //this.StarandArea += (1.0 * int.Parse(materials.长度) / 1e3 * int.Parse(materials.宽度) / 1e3 * c);
            }
        }

        internal void Sort(List<NestSortSheetData> rules)
        {
            if (this.OriginDatas.Count <= 1)
            {
                return;
            }

            bool first = true;

            IOrderedEnumerable<OriginData> orders = null;

            foreach (var item in rules)
            {
                if (!string.IsNullOrEmpty(item.规则)) continue;

                if (first)
                {
                    first = false;

                    if (item.排序规则 == "升序")
                    {
                        orders = OriginDatas.OrderBy(x =>
                        {
                            if (x.Datas.ContainsKey(item.字段))
                            {
                                double tmp = 0;
                                double.TryParse(x.Datas[item.字段], out tmp);
                                return tmp;
                            }
                            return -1;
                        });
                    }
                    else if (item.排序规则 == "降序")
                    {
                        orders = OriginDatas.OrderByDescending(x =>
                        {
                            if (x.Datas.ContainsKey(item.字段))
                            {
                                double tmp = 0;
                                double.TryParse(x.Datas[item.字段], out tmp);
                                return tmp;
                            }
                            return -1;
                        });
                    }
                }
                else
                {
                    if (item.排序规则 == "升序")
                    {
                        orders = orders.ThenBy(x =>
                        {
                            if (x.Datas.ContainsKey(item.字段))
                            {
                                double tmp = 0;
                                double.TryParse(x.Datas[item.字段], out tmp);
                                return tmp;
                            }
                            return -1;
                        });
                    }
                    else if (item.排序规则 == "降序")
                    {
                        orders = orders.ThenByDescending(x =>
                        {
                            if (x.Datas.ContainsKey(item.字段))
                            {
                                double tmp = 0;
                                double.TryParse(x.Datas[item.字段], out tmp);
                                return tmp;
                            }
                            return -1;
                        });
                    }
                }
            }

            if (orders != null)
            {
                this.OriginDatas = orders.ToList();
            }
        }
    }
}
