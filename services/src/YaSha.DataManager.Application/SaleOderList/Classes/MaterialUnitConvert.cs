using System.ComponentModel;

namespace YaSha.DataManager.SaleOderList.Classes
{
    /// <summary>
    /// 单位转换
    /// </summary>
    public class MaterialUnitConvert
    {
        [Description("材料编码")]
        public string MaterialCode { get; set; }

        [Description("材料名称")]
        public string MaterialName { get; set; }

        [Description("使用单位")]
        public string UsageUnit { get; set; }

        [Description("结算单位")]
        public string SettlementUnit { get; set; }

        [Description("采购单位")]
        public string PurchasingUnit { get; set; }

    }
}
