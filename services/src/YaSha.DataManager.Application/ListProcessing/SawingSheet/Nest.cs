#define BEST
using System.Diagnostics;
using YaSha.DataManager.ListProcessing.SawingSheet;

namespace YaSha.DataManager.ListProcessing.SawingSheet
{
    /// <summary>
    /// 套料类
    /// </summary>
    internal class Nest
    {
        private List<MaterialGroup> materialGroups;
        private List<BoardGroup> boardGroups;
        private List<Board> allBoards;
        private List<Material> allMaterials;
        Dictionary<int, List<Result>> cache;
        internal Nest(List<MaterialGroup> materialGroups, List<BoardGroup> boardGroups)
        {
            this.materialGroups = materialGroups;
            this.boardGroups = boardGroups;
            this.allBoards = new List<Board>();
            this.allMaterials = new List<Material>();
            foreach (var item in this.boardGroups)
            {
                this.allBoards.AddRange(item.GetBoards());
            }
            foreach (var item in this.materialGroups)
            {
                this.allMaterials.AddRange(item.GetMaterials());
            }
            cache = new Dictionary<int, List<Result>>();
        }

        internal List<Result> Start()
        {
            List<Result> best = null;
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    List<Result> result = launchWorkers();
                    if (best == null)
                    {
                        best = result;
                    }
                    else
                    {
                        var bestfit = getfitness(best);
                        var resultfit = getfitness(result);
                        if (bestfit > resultfit)
                        {
                            best = result;
                        }
                    }
                }
                catch
                {
                }
            }
            return best;
        }

        private List<Result> launchWorkers()
        {
            var results = new List<List<Result>>();
            for (int i = 0; i < 100; i++)
            {
                var tmp = this.boardGroups.Select(x => x.Clone()).OrderBy(x => Guid.NewGuid().GetHashCode()).ToList();
                var placement = new Placementworker(materialGroups);
                var result = placement.Place(tmp);
                results.Add(result);
            }
            List<Result> allResults = new List<Result>();
            foreach (var item in results)
            {
                allResults.AddRange(item);
            }
            var random = new Random(Guid.NewGuid().GetHashCode());
            var crossDatas = new List<List<Result>>();
            foreach (var item in results)
            {
#if BEST
                #region 在原料可以看作无限的情况下速度快
                var clone = item.Select(x => x.Clone()).ToList();
                var best = new List<Result>();
                for (int i = 0; i < clone.Count; i++)
                {
                    if (cache.ContainsKey(clone[i].id) && cache[clone[i].id].Count() > 0)
                    {
                        best.Add(cache[clone[i].id].First());
                    }
                    else
                    {
                        var finds = allResults.FindAll(x => x.Equals(clone[i])).OrderBy(x => x.GetMaterialArea()).ToList();
                        cache.Add(clone[i].id, finds);
                        if (finds.Count > 0)
                        {
                            best.Add(finds[0]);
                        }
                        else
                        {
                            best.Add(clone[i]);
                        }
                    }
                }
                if (check(best))
                {
                    crossDatas.Add(best);
                }
                else
                {
                    var tmps = new List<List<Result>>();
                    for (int i = 0; i < 50; i++)
                    {
                        var tmp = item.Select(x => x.Clone()).OrderBy(x => random.NextDouble()).ToList();

                        for (int j = 0; j < tmp.Count; j++)
                        {
                            var tmpClone = tmp[j].Clone();
                            tmp[j] = cache[tmp[j].id].First();
                            if (!check(tmp))
                            {
                                tmp[j] = tmpClone;
                                break;
                            }
                        }
                        tmps.Add(tmp);
                    }
                    crossDatas.Add(tmps.OrderBy(x =>
                    {
                        var d = getfitness(x);
                        return d;
                    }).First());
                }
                #endregion
#endif

#if !BEST
                #region 全随机 运行速度跟原料数量无关

                var datas = new List<List<Result>>();
                for (int i = 0; i < 50; i++)
                {
                    List<Result> tmp = item.Select(x => x.Clone()).ToList();
                    var crossGeneCount = random.Next(0, item.Count);
                    List<int> indexLists = new List<int>();
                    while (crossGeneCount-- > 0)
                    {
                        int index = random.Next(0, item.Count);
                        if (indexLists.Contains(index))
                        {
                            continue;
                        }
                        else
                        {
                            indexLists.Add(index);
                        }
                        if (cache.ContainsKey(item[index].id) && cache[item[index].id].Count() > 0)
                        {
                            tmp[index] = cache[item[index].id].First();
                        }
                        else
                        {
                            var sameData = allResults.FindAll(x => x.Equals(item[index])).OrderBy(x => x.GetMaterialArea()).ToList();
                            cache.Add(item[index].id, sameData);
                            tmp[index] = sameData.First();
                        }
                    }
                    datas.Add(tmp);
                }
                var minCross = datas.Where(x => check(x)).ToList();
                if (0 == minCross.Count)
                {
                    crossDatas.Add(item);
                }
                else
                {
                    crossDatas.Add(minCross.OrderBy(x =>
                    {
                        var d = getfitness(x);
                        return d;
                    }).First());
                }

                #endregion
#endif
            }
            var min = crossDatas.OrderBy(x =>
            {
                var d = getfitness(x);
                return d;
            }).First();
            return min;
        }

        private double getfitness(List<Result> results)
        {
            double boardarea = 0;

            double materialarea = 0;

            foreach (var result in results)
            {
                foreach (var item in result.boardGroups)
                {
                    boardarea += (item.GetLength() / 1e3 * item.GetWidth() / 1e3 * item.GetSize());
                }

                foreach (var item in result.materials)
                {
                    materialarea += (item.GetLength() / 1e3 * item.GetWidth() / 1e3);
                }
            }
            if (materialarea == 0) return 9999;

            return (materialarea - boardarea) / materialarea;
        }

        private bool check(List<Result> results)
        {
            var allboards = new List<Board>();
            var allmaterials = new List<Material>();
            foreach (var item in results)
            {
                foreach (var boardGroup in item.boardGroups)
                {
                    allboards.AddRange(boardGroup.GetBoards());
                }
                allmaterials.AddRange(item.materials);
            }

            if (allboards.Count != this.allBoards.Count || allmaterials.Count > this.allMaterials.Count)
            {
                return false;
            }

            bool flag = true;
            var groups = allmaterials.GroupBy(x => new { l = x.GetLength(), w = x.GetWidth() });
            foreach (var group in groups)
            {
                var find = this.materialGroups.Find(x => x.GetLength() == group.Key.l && x.GetWidth() == group.Key.w);

                if (group.Count() > find.GetSize())
                {
                    flag = false;
                    break;
                }
            }
            if (!flag)
            {
                return false;
            }

            var boardgroup = allboards.GroupBy(x => x.GetParentId());

            foreach (var group in boardGroups)
            {
                var find = this.boardGroups.Find(x => x.GetGroupId() == group.GetGroupId());

                if (group.GetSize() != find.GetSize())
                {
                    flag = false;
                    break;
                }
            }
            if (!flag)
            {
                return false;
            }

            return true;
        }

        private void show(List<Result> results)
        {
            string msg = string.Empty;

            msg += "\n\n\n";

            var allMaterials = new List<Material>();

            foreach (var result in results)
            {
                foreach (var item in result.boardGroups)
                {
                    msg += $"{item.GetLength()}x{item.GetWidth()}={item.GetSize()}\n";
                }
                var groups = result.materials.GroupBy(x => new { length = x.GetLength(), width = x.GetWidth() });

                foreach (var group in groups)
                {
                    msg += $"消耗{group.Key.length}x{group.Key.width}={group.Count()}\n";
                }

                allMaterials.AddRange(result.materials);

                msg += "\n\n------------------------------------\n\n";
            }

            var materialGroups = allMaterials.GroupBy(x => new { l = x.GetLength(), w = x.GetWidth() });

            foreach (var group in materialGroups)
            {
                msg += $"{group.Key.l}x{group.Key.w}={group.Count()}\n";
            }

            Trace.WriteLine(msg);
        }
    }
}
