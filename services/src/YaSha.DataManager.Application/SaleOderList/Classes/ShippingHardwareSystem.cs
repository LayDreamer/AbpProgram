using System.ComponentModel;

namespace YaSha.DataManager.SaleOderList.Classes
{
    /// <summary>
    /// 发货五金（系统）
    /// </summary>
    public class ShippingHardwareSystem : SaleOrderSystemBase
    {
        [Description("备注")]
        public string Remark { get; set; }

        [Description("物料优先级与打孔信息")]
        public string PriorityInformation { get; set; }
    }
}
