using System.ComponentModel;

namespace YaSha.DataManager.SaleOderList.Classes
{
    /// <summary>
    /// 销售订单
    /// </summary>
    public class SaleOrderDetail
    {
       

        [Description("物料编码")]
        public double MaterialCode { get; set; }

        [Description("客户物料编码")]
        public string CustomMaterialCode { get; set; }

        [Description("采购数量")]
        public double PurchaseQuantity { get; set; }

        [Description("使用数量")]
        public double UsageQuantity { get; set; }

        [Description("销售单价")]
        public string Price { get; set; }

        [Description("税率")]
        public string TaxRate { get; set; }

        [Description("备注")]
        public string Remark { get; set; }

        [Description("要求到货日期")]
        public string ArrivalDate { get; set; }

        [Description("订单行类型")]
        public string OrderType { get; set; }

        [Description("送达客户编码")]
        public string ArrivalCode { get; set; }

        [Description("送货地点")]
        public string DeliveryLocation { get; set; }

        [Description("详细地址")]
        public string Address { get; set; }

        [Description("收货人")]
        public string Consignee { get; set; }

        [Description("收货人联系电话")]
        public string ConsigneePhoneNumber { get; set; }

        [Description("送货方式")]
        public string DeliveryMethod { get; set; }

        [Description("交货工厂编码")]
        public string DeliveryFactoryCode { get; set; }

        [Description("销售合同")]
        public string SalesContract { get; set; }
    }


    public class FinishedCodeDetail
    {
        [Description("产成品编码")]
        public string FinishedCode { get; set; }

        [Description("模块名称")]
        public string ModuleName { get; set; }

        [Description("模块编码")]
        public string ModuleCode { get; set; }

        [Description("安装码")]
        public string InstallationCode { get; set; }

        [Description("模块-长")]
        public string ModuleLength { get; set; }

        [Description("模块-宽")]
        public string ModuleWidth { get; set; }

        [Description("模块-高")]
        public string ModuleHeight { get; set; }

        [Description("模组类型")]
        public string ModuleType { get; set; }

        [Description("模块-小计")]
        public string ModuleCount { get; set; }


    }

}
