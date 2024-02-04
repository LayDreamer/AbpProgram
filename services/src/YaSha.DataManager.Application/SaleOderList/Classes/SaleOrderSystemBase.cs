using System.ComponentModel;

namespace YaSha.DataManager.SaleOderList.Classes
{
    public class SaleOrderSystemBase
    {
        [Description("系统")]
        public string System { get; set; }

        [Description("产成品编码")]
        public string FinishedProductCode { get; set; }

        [Description("物料名称")]
        public string MaterialName { get; set; }

        [Description("物料编码")]
        public string MaterialCode { get; set; }

        [Description("物料-长")]
        public string MaterialLength { get; set; }

        [Description("物料-宽")]
        public string MaterialWidth { get; set; }

        [Description("物料-高")]
        public string MaterialHeight { get; set; }

        [Description("外包展开面")]
        public string ExtendedSurface { get; set; }

        [Description("型号")]
        public string Type { get; set; }

        [Description("单套")]
        public double SingleSet { get; set; }

        [Description("批量")]
        public double Batch { get; set; }

        [Description("单位")]
        public string Unit { get; set; }
    }
}
