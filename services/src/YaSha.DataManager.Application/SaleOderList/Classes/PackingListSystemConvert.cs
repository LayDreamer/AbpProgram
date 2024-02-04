using Volo.Abp;

namespace YaSha.DataManager.SaleOderList.Classes
{
    /// <summary>
    /// 领料单-板
    /// </summary>

    public class PackingListSystemConvert : SaleOrderListBase
    {
        public List<PackingListSystem> packingListSystems = new();
        public PackingListSystem packingListSystem;

        public override void Excute()
        {
            try
            {
                foreach (var item in packingListSystems)
                {
                    saleOrder = new SaleOrderDetail();
                    packingListSystem = item;
                    systemBase = item;
                    materialUnit = materialUnits.Find(e => e.MaterialCode == item.MaterialCode);

                    CalcData();
                    saleOrders.Add(saleOrder);
                }
                AfterCalc();
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException($"处理表单：【包装】数据错误！{ex.Message}");
            }
        }

        public override void GetMaterialCode()
        {
            string code = string.IsNullOrEmpty(packingListSystem.FinishedProductCode) ? packingListSystem.System : packingListSystem.FinishedProductCode;
            string value = string.IsNullOrEmpty(code) ? packingListSystem.FinishProduct : code;
            //string value = string.IsNullOrEmpty(packingListSystem.FinishedProductCode) ?
            //  packingListSystem.FinishProduct : packingListSystem.FinishedProductCode;
            if (value == null)
                return;
            double.TryParse(value, out double res);
            saleOrder.MaterialCode = res;
        }
    }
}
