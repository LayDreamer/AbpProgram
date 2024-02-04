namespace YaSha.DataManager.ListProcessing.SawingSheet
{
    /// <summary>
    /// 原料板中一列
    /// </summary>
    [Serializable]
    internal class Stick
    {
        private int parentid;

        private int id;

        private double length;

        private double width;

        private List<Board> boards;

        internal Stick(int parentid, int id, double length, double width)
        {
            this.parentid = parentid;
            this.id = id;
            this.length = length;
            this.width = width;
            boards = new List<Board>();
        }

        internal Stick(Stick stick)
        {
            this.parentid = stick.parentid;
            this.id = stick.id;
            this.length = stick.length;
            this.width = stick.width;
            this.boards = new List<Board>();
            foreach (var item in stick.GetBoards())
            {
                this.boards.Add(item.Clone());
            }

        }

        internal int GetParentId()
        {
            return parentid;
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

        internal List<Board> GetBoards()
        {
            return boards;
        }

        internal double GetSubLength()
        {
            return length - boards.Sum(x => x.GetLength());
        }

        internal Stick Add(Board board)
        {
            boards.Add(board);
            return this;
        }

        internal Stick Clone()
        {
            return new Stick(this);
        }

        /// <summary>
        /// 转为JSON 作为KEY
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"parentid={parentid},id={id},length={length},width={width}";
        }
    }
}
