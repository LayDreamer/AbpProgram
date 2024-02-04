namespace YaSha.DataManager.ListProcessing.SawingSheet
{
    internal class Result
    {
        internal int id;

        internal List<BoardGroup> boardGroups;

        internal List<Material> materials;

        internal Result(List<BoardGroup> groups, List<Material> materials, int id)
        {
            this.id = id;
            this.boardGroups = groups;
            this.materials = materials;
        }
        internal Result()
        {
            this.id = Guid.NewGuid().GetHashCode();
            this.boardGroups = new List<BoardGroup>();
            this.materials = new List<Material>();
        }
        internal Result(Result result)
        {
            this.id = result.id;
            this.boardGroups = new List<BoardGroup>();
            this.materials = new List<Material>();
            foreach (var item in result.boardGroups)
            {
                this.boardGroups.Add(new BoardGroup(item));
            }
            foreach (var item in result.materials)
            {
                this.materials.Add(new Material(item));
            }
        }

        internal double GetMaterialArea()
        {
            double area = 0;

            foreach (var item in materials)
            {
                area += (item.GetLength() / 1e3 * item.GetWidth() / 1e3);
            }

            return area;
        }

        internal bool Equals(Result other)
        {
            bool iRet = false;

            if (other.boardGroups.Count == this.boardGroups.Count)
            {
                var ct = new List<int>();
                foreach (var item in this.boardGroups)
                {
                    bool flag = false;
                    foreach (var o in other.boardGroups)
                    {
                        if (item.Equals(o))
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        break;
                    }
                    else
                    {
                        ct.Add(0);
                    }
                }

                if (ct.Count == this.boardGroups.Count)
                {
                    iRet = true;
                }
            }
            return iRet;
        }

        internal Result Clone()
        {
            return new Result(this);
        }
    }
}
