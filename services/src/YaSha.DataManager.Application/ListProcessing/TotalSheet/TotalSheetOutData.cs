using YaSha.DataManager.ListProcessing.CommonModel;

namespace YaSha.DataManager.ListProcessing.TotalSheet
{
    internal class TotalSheetOutData
    {
        internal string MaterialInfo { get; set; }


        internal int Length { get; set; }


        internal int Width { get; set; }


        internal int Height { get; set; }


        internal string Brand { get; set; }


        internal string Finish { get; set; }


        internal int Total { get; set; }


        internal int Numbers { get; set; }


        internal string MaterialSize { get; set; }


        internal double Area { get; set; }



        internal static List<TotalSheetOutData> Get(List<TotalSheetOriginData> datas)
        {
            List<TotalSheetOutData> results = new List<TotalSheetOutData>();

            var groups = datas.OrderByDescending(x => x.MaterialName).ThenByDescending(x => x.Type).GroupBy(x => new { x.MaterialName, x.Brand, x.Type });

            foreach (var group in groups)
            {
                List<OriginData> origins = new List<OriginData>();

                var sizes = new List<BaseBoard>();

                foreach (var item in group)
                {
                    sizes.AddRange(item.Boards);
                }

                var groupsizes = sizes.OrderByDescending(x => x.Length).ThenByDescending(x => x.Width).GroupBy(x => new { x.Length, x.Width, x.Height });

                var first = group.First();

                foreach (var item in groupsizes)
                {
                    TotalSheetOutData data = new TotalSheetOutData()
                    {
                        MaterialInfo = first.MaterialName,

                        Length = item.Key.Length,

                        Width = item.Key.Width,

                        Height = item.Key.Height,

                        Brand = first.Brand,

                        Finish = first.Type,

                        Numbers = first.TotalNumber,

                        Total = item.Sum(x => x.Size),
                    };

                    results.Add(data);
                }
            }

            return results;
        }
    }
}
