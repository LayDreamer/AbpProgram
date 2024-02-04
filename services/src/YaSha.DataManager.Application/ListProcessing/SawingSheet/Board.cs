namespace YaSha.DataManager.ListProcessing.SawingSheet
{
    /// <summary>
    /// 下料板
    /// </summary>
    [Serializable]
    internal class Board
    {
        private int parentid;

        private int id;

        private double length;

        private double width;

        private double cutLoss;

        internal Board(int parentid, int id, double length, double width, double cutLoss = 0.0)
        {
            this.parentid = parentid;
            this.id = id;
            this.length = length;
            this.width = width;
            this.cutLoss = cutLoss;
        }

        internal Board(Board board)
        {
            this.parentid = board.parentid;
            this.id = board.id;
            this.length = board.length;
            this.width = board.width;
            this.cutLoss = board.cutLoss;
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

        internal double GetCutLoss()
        {
            return cutLoss;
        }

        internal Board Clone()
        {
            return new Board(this);
        }
    }
}
