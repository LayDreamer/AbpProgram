namespace YaSha.DataManager.ListProcessing.SawingSheet
{
    /// <summary>
    /// 下料板组
    /// </summary>
    internal class BoardGroup
    {
        private int groupId;

        private double length;

        private double width;

        private double cutLoss;

        private List<Board> allBoards;

        internal BoardGroup(int groupId, double length, double width, double size, double cutLoss = 0.0)
        {
            this.groupId = groupId;
            this.length = length;
            this.width = width;
            this.cutLoss = cutLoss;
            allBoards = new List<Board>();
            for (int i = 0; i < size; ++i)
            {
                allBoards.Add(new Board(groupId, Guid.NewGuid().GetHashCode(), length, width, cutLoss));
            }
        }

        internal BoardGroup(BoardGroup boardGroup)
        {
            this.groupId = boardGroup.groupId;
            this.length = boardGroup.length;
            this.width = boardGroup.width;
            this.cutLoss = boardGroup.cutLoss;
            this.allBoards = boardGroup.GetBoards().Select(x => x.Clone()).ToList();
        }

        internal int GetGroupId()
        {
            return groupId;
        }

        internal double GetLength()
        {
            return length;
        }

        internal double GetWidth()
        {
            return width;
        }

        internal double GetSize()
        {
            return GetBoards().Count();
        }

        internal List<Board> GetBoards()
        {
            return allBoards;
        }

        internal double GetCutLoss()
        {
            return cutLoss;
        }

        internal BoardGroup Clone()
        {
            return new BoardGroup(this);
        }

        internal bool Equals(BoardGroup other)
        {
            return this.groupId == other.groupId;
        }

        public override string ToString()
        {
            return $"{this.length}x{this.width}={this.allBoards.Count}";
        }
    }
}
