namespace YaSha.DataManager.SaleOderList.Classes
{
    public abstract class SaleOrderListBase
    {
        public const string SquareMetre = "平方米";
        public const string Metre = "米";

        public List<SaleOrderDetail> resSaleOrders = new();

        public List<SaleOrderDetail> saleOrders = new();
        public SaleOrderDetail saleOrder;

        public List<MaterialUnitConvert> materialUnits = new();
        public MaterialUnitConvert materialUnit;

        public SaleOrderSystemBase systemBase;

        /// <summary>
        /// 执行
        /// </summary>
        public abstract void Excute();

        /// <summary>
        /// 操作赋值
        /// </summary>
        public void CalcData()
        {
            GetMaterialCode();
            GetPurchaseQuantity();
            GetUsageQuantity();
            GetPrice();
            GetTaxRate();
            GetRemark();
            GetArrivalDate();
            GetOrderType();
            GetArrivalCode();
            GetDeliveryFactoryCode();
        }

        /// <summary>
        /// 获取物料编码
        /// </summary>
        public virtual void GetMaterialCode()
        {
            if (systemBase.FinishedProductCode == null && systemBase.System == null)
                return;
            double.TryParse(systemBase.FinishedProductCode, out double codeRes);
            double.TryParse(systemBase.System, out double systemRes);
            saleOrder.MaterialCode = codeRes != 0 ? codeRes : systemRes;
        }

        /// <summary>
        /// 获取采购数量
        /// </summary>
        public virtual void GetPurchaseQuantity()
        {
            if (materialUnit == null)
                return;
            //当不存在结算单位时，读取采购单位的值
            materialUnit.SettlementUnit = string.IsNullOrEmpty(materialUnit.SettlementUnit) ? materialUnit.PurchasingUnit : materialUnit.SettlementUnit;
            if (materialUnit.UsageUnit == materialUnit.SettlementUnit)
            {
                saleOrder.PurchaseQuantity = systemBase.Batch;
            }
            else if (materialUnit.UsageUnit != materialUnit.SettlementUnit && materialUnit.SettlementUnit == SquareMetre)
            {
                string materialWidth = systemBase.MaterialWidth;
                if (systemBase.MaterialWidth.Contains("+"))
                {
                    materialWidth = systemBase.ExtendedSurface;
                }

                double.TryParse(systemBase.MaterialLength, out double length);
                double.TryParse(materialWidth, out double width);
                saleOrder.PurchaseQuantity = length * width * systemBase.Batch / 1000000;
                //saleOrder.PurchaseQuantity = double.Parse(systemBase.MaterialLength) * double.Parse(materialWidth)
                //                                * systemBase.Batch / 1000000;
            }
            else if (materialUnit.UsageUnit != materialUnit.SettlementUnit && materialUnit.SettlementUnit == Metre)
            {
                double.TryParse(systemBase.MaterialLength, out double length);
                saleOrder.PurchaseQuantity = length * systemBase.Batch / 1000;
                //saleOrder.PurchaseQuantity = double.Parse(systemBase.MaterialLength) * systemBase.Batch / 1000;
            }
        }

        /// <summary>
        /// 获取使用数量
        /// </summary>
        public virtual void GetUsageQuantity()
        {
            saleOrder.UsageQuantity = systemBase.Batch;
        }


        /// <summary>
        /// 获取销售单价
        /// </summary>
        public virtual void GetPrice()
        {
            saleOrder.Price = "1.";
        }


        /// <summary>
        /// 获取税率
        /// </summary>
        public virtual void GetTaxRate()
        {
            saleOrder.TaxRate = "13%";
        }

        /// <summary>
        /// 获取备注
        /// </summary>
        public virtual void GetRemark()
        {
        }

        /// <summary>
        /// 获取要求到货日期
        /// </summary>
        public virtual void GetArrivalDate()
        {

        }

        /// <summary>
        /// 获取订单行类型
        /// </summary>
        public virtual void GetOrderType()
        {

        }

        /// <summary>
        /// 获取送达客户编码
        /// </summary>
        public virtual void GetArrivalCode()
        {

        }

        /// <summary>
        /// 获取交货工厂编码
        /// </summary>
        public virtual void GetDeliveryFactoryCode()
        {
            saleOrder.DeliveryFactoryCode = "T19120003";
        }

        /// <summary>
        /// 物料编码一致的，相关字段需要累加
        /// </summary>
        public void AfterCalc()
        {
            var saleOrdersGroup = saleOrders.GroupBy(e => e.MaterialCode);
            foreach (var saleOrders in saleOrdersGroup)
            {
                double purchaseQuantity = 0;
                double usageQuantity = 0;
                SaleOrderDetail saleOrderDetail = saleOrders.FirstOrDefault();
                foreach (var item in saleOrders)
                {
                    purchaseQuantity += item.PurchaseQuantity;
                    usageQuantity += item.UsageQuantity;
                }
                saleOrderDetail.PurchaseQuantity = purchaseQuantity;
                saleOrderDetail.UsageQuantity = usageQuantity;

                resSaleOrders.Add(saleOrderDetail);
            }
        }
    }
}
