namespace YaSha.DataManager.ListProcessing.CommonModel
{
    internal class BaseBoard
    {
        internal int Length { get; set; }

        internal int Width { get; set; }

        internal int Height { get; set; }

        internal int Size { get; set; }

        internal BaseBoard(int length, int width, int height, int size)
        {
            Length = length;
            Width = width;
            Height = height;
            Size = size;
        }

        internal BaseBoard() { }
    }

    internal class BaseInfoBoard : BaseBoard
    {
        internal string Model { get; set; }

        internal string ProcessInfo { get; set; }
    }
}
