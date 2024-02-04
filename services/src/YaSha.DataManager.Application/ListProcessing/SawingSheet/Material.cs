using YaSha.DataManager.ListProcessing.SawingSheet;

namespace YaSha.DataManager.ListProcessing.SawingSheet
{
    /// <summary>
    /// 原料板
    /// </summary>
    [Serializable]
    internal class Material
    {
        private int parentId;

        private int id;

        private double length;

        private double width;

        private Dictionary<string, int> maxcutcount;

        private List<Stick> container;

        internal Material(int parentid, int id, double length, double width, Dictionary<string, int> maxcutcount)
        {
            parentId = parentid;
            this.id = id;
            this.length = length;
            this.width = width;
            this.maxcutcount = maxcutcount;
            container = new List<Stick>();
        }

        internal Material(Material material)
        {
            this.parentId = material.parentId;
            this.id = material.id;
            this.length = material.length;
            this.width = material.width;
            this.maxcutcount = material.maxcutcount;
            this.container = new List<Stick>();
            foreach (var item in material.GetSticks())
            {
                this.container.Add(item.Clone());
            }
        }

        internal int GetParentId()
        {
            return parentId;
        }

        internal int GetId()
        {
            return id;
        }

        internal double GetLength()
        {
            return length;
        }

        internal double GetWidth()
        {
            return width;
        }

        internal List<Stick> GetSticks()
        {
            return container;
        }

        internal List<Board> GetBoards()
        {
            List<Board> tmps = new List<Board>();

            foreach (var item in GetSticks())
            {
                tmps.AddRange(item.GetBoards());
            }
            return tmps;
        }

        internal bool Add(Board board)
        {
            if (board.GetWidth() > GetWidth() || board.GetLength() > GetLength())
            {
                return false;
            }

            if (!CheckByRule()) return false;

            //TODO
            var sameWidhStick = container.FirstOrDefault(x => x.GetWidth() == board.GetWidth() && board.GetLength() <= x.GetSubLength());

            if (null == sameWidhStick)
            {
                //TODO
                var find = container.FirstOrDefault(x => x.GetWidth() >= board.GetWidth() && x.GetLength() >= board.GetLength() && x.GetBoards().Count == 0);

                if (find == null)
                {
                    if (0 == container.Count)
                    {
                        container.Add(new Stick(id, Guid.NewGuid().GetHashCode(), length, board.GetWidth()).Add(board));
                        double subWidth = width - board.GetWidth() - board.GetCutLoss();
                        if (subWidth > 0)
                        {
                            container.Add(new Stick(id, Guid.NewGuid().GetHashCode(), length, subWidth));
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    container.Remove(find);
                    container.Add(new Stick(id, Guid.NewGuid().GetHashCode(), length, board.GetWidth()).Add(board));
                    var subWidth = width - container.Sum(x =>
                    {
                        var d1 = x.GetWidth();
                        if (x.GetBoards().Count() > 0)
                        {
                            d1 += x.GetBoards()[0].GetCutLoss();
                        }
                        return d1;
                    });
                    if (subWidth > 0)
                    {
                        container.Add(new Stick(id, Guid.NewGuid().GetHashCode(), length, subWidth));
                    }

                }
            }
            else
            {
                sameWidhStick.Add(board);
            }
            return true;
        }

        bool CheckByRule()
        {
            var boards = GetBoards();

            if (boards.Count > this.maxcutcount["宽度"] || boards.Count > this.maxcutcount["长度"])
            {
                return false;
            }

            //长度检查
            foreach (var item in this.container)
            {
                if (item.GetBoards().Count > this.maxcutcount["长度"])
                {
                    return false;
                }
            }

            //宽度检查
            if (this.container.Count(x => x.GetBoards().Count > 0) > this.maxcutcount["宽度"])
            {
                return false;
            }

            return true;
        }

        internal Material Clone()
        {
            return new Material(this);
        }

        public override string ToString()
        {
            string str = string.Empty;

            str += $"共{container.Count}列 \n";

            for (int i = 0; i < container.Count; ++i)
            {
                str += $"第{i + 1}列 长{container[i].GetLength()} 宽{container[i].GetWidth()} \n";

                for (int j = 0; j < container[i].GetBoards().Count; j++)
                {
                    str += $"第{j + 1} 块 长{container[i].GetBoards()[j].GetLength()} 宽{container[i].GetBoards()[j].GetWidth()}  ";
                }

                str += "\n\n";
            }
            return str;
        }
    }
}
