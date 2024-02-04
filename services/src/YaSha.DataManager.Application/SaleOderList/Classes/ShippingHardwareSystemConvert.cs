using Volo.Abp;

namespace YaSha.DataManager.SaleOderList.Classes
{
    /// <summary>
    /// 发货五金（系统）
    /// </summary>

    public class ShippingHardwareSystemConvert : SaleOrderListBase
    {
        public List<ShippingHardwareSystem> shippingHardwareSystems = new();

        public override void Excute()
        {
            try
            {
                foreach (var item in shippingHardwareSystems)
                {
                    try
                    {
                        saleOrder = new SaleOrderDetail();
                        systemBase = item;
                        materialUnit = materialUnits.Find(e => e.MaterialCode == item.MaterialCode);
                        CalcData();
                        saleOrders.Add(saleOrder);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                AfterCalc();
            }
            catch (Exception)
            {
                throw new UserFriendlyException("处理表单：【发货五金】数据错误！");
            }
        }
    }
}
