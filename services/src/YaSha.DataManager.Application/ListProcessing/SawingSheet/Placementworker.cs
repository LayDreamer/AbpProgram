using YaSha.DataManager.ListProcessing.SawingSheet;

namespace YaSha.DataManager.ListProcessing.SawingSheet
{
    /// <summary>
    /// 放置板块类
    /// </summary>
    internal class Placementworker
    {
        /// <summary>
        /// 原材料
        /// </summary>
        internal List<MaterialGroup> binGroups;


        internal Placementworker(List<MaterialGroup> binGroups)
        {
            this.binGroups = binGroups.Select(x => x.Clone()).ToList();
        }


        internal List<Result> Place(List<BoardGroup> paths)
        {
            //List<MaterialGroup> test = new List<MaterialGroup>();

            //for (int i = 0; i < 1e4; i++)
            //{
            //    var d = SelectMaterialGroup(this.binGroups, paths[0]);
            //    test.Add(d);
            //}

            //var g = test.GroupBy(x=>new { l = x.GetLength(), w = x.GetWidth() });
            //string msg = string.Empty;
            //foreach (var item in g)
            //{
            //    msg += $"{item.Key.l}x{item.Key.w}={item.Count()}\n"; 
            //}
            //Console.WriteLine(msg);

            //return null;

            List<Result> results = new List<Result>();
            Random random = new Random(Guid.NewGuid().GetHashCode());
            while (paths.Count > 0)
            {
                var path = paths[0];
                Result replace = null;
                foreach (var item in results.OrderBy(x => Guid.NewGuid().GetHashCode()))
                {
                    if (Add(item, path, out Result data))
                    {
                        replace = data;
                        //TODO 多个加工板合并起来了
                        break;
                    }
                }
                if (replace != null)
                {
                    var find = results.Find(x => x.id == replace.id);
                    var index = results.IndexOf(find);
                    results[index] = replace;
                    paths.Remove(path);
                    continue;
                }
                var result = new Result();
                result.boardGroups.Add(path);
                var boards = path.GetBoards().Select(x => x.Clone()).ToList();

                //赌轮盘方式选取
                MaterialGroup group = SelectMaterialGroup(this.binGroups, path);
                //无法从原材料中进行加工
                if (group == null)
                {
                    results.Add(result);
                    paths.Remove(path);
                    continue;
                }
                Material material = SelectMaterial(group);
                while (boards.Count > 0)
                {
                    double n = random.NextDouble();
                    if (0 == result.materials.Count)
                    {
                        material.Add(boards[0]);
                        result.materials.Add(material);
                    }
                    else
                    {
                        //增加原材料规格
                        if (n < 0.1)
                        {
                            group = SelectMaterialGroup(this.binGroups, path);
                            material = SelectMaterial(group);
                            material.Add(boards[0]);
                            result.materials.Add(material);
                        }
                        else
                        {
                            var place = false;
                            var clone = result.materials.Select(x => x.Clone()).OrderBy(x => Guid.NewGuid().GetHashCode()).ToList();
                            for (int i = 0; i < clone.Count; i++)
                            {
                                if (clone[i].Add(boards[0]))
                                {
                                    place = true;
                                    var find = result.materials.Find(x => x.GetId() == clone[i].GetId());
                                    result.materials[result.materials.IndexOf(find)].Add(boards[0]);
                                    break;
                                }
                            }
                            if (!place)
                            {
                                var ids = result.materials.Select(x => x.GetParentId()).ToList();
                                var groups = this.binGroups.Where(x => x.GetSize() > 0 && ids.Contains(x.GetId())).ToList();
                                if (groups.Count > 0)
                                {
                                    group = SelectMaterialGroup(groups, path);
                                    material = SelectMaterial(group);
                                }
                                else
                                {
                                    groups = this.binGroups.Where(x => x.GetSize() > 0).ToList();
                                    group = SelectMaterialGroup(groups, path);
                                    material = SelectMaterial(group);
                                }
                                material.Add(boards[0]);
                                result.materials.Add(material);
                            }
                        }
                    }

                    boards.Remove(boards[0]);
                }
                results.Add(result);
                paths.Remove(path);
            }
            return results;
        }

        bool Add(Result result, BoardGroup boardGroup, out Result data)
        {
            data = null;
            var boards = boardGroup.GetBoards().OrderBy(x => Guid.NewGuid().GetHashCode()).ToList();
            var materials = result.materials.Select(x => x.Clone()).OrderBy(x => Guid.NewGuid()).ToList();
            while (boards.Count > 0)
            {
                bool flag = false;
                for (int i = 0; i < materials.Count; i++)
                {
                    if (materials[i].Add(boards[0]))
                    {
                        flag = true; break;
                    }
                }
                if (!flag)
                {
                    return false;
                }
                boards.Remove(boards[0]);
            }
            var b = result.boardGroups;
            b.Add(boardGroup);
            var m = materials;
            data = new Result(b, m, result.id);
            return true;
        }

        MaterialGroup SelectMaterialGroup(List<MaterialGroup> materialgroups, BoardGroup path)
        {
            var materialPlaceGroups = materialgroups.Where(x => x.GetLength() >= path.GetLength() && x.GetWidth() >= path.GetWidth() && x.GetSize() > 0).
                OrderBy(x => x.GetWidth()).ThenBy(x => x.GetLength()).ToList();
            if (materialPlaceGroups.Count == 0)
            {
                return null;
            }
            var loss = materialPlaceGroups.Select(x => 1.0 * path.GetLength() * path.GetWidth() / x.GetLength() / x.GetWidth()).ToList();
            var total = loss.Sum(x => x);
            var tmp = loss.Select(x => x / total).ToList();
            var weight = new List<double>();
            for (int i = 0; i < tmp.Count; i++)
            {
                double a = 0;
                for (int j = 0; j <= i; j++)
                {
                    a += tmp[j];
                }
                weight.Add(a);
            }
            var random = new Random(Guid.NewGuid().GetHashCode());

            var d = random.NextDouble();
            var find = weight.Find(x => d < x);
            var index = weight.IndexOf(find);
            return materialPlaceGroups[index];
        }

        Material SelectMaterial(MaterialGroup materialGroup)
        {
            var materials = materialGroup.GetMaterials();
            if (materials.Count == 0)
            {
                return null;
            }
            var material = materials[0];
            materialGroup.Remove(material.GetId());
            return material;
        }


    }
}
