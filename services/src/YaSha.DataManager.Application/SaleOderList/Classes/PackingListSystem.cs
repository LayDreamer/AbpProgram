using System.ComponentModel;

namespace YaSha.DataManager.SaleOderList.Classes
{
    /// <summary>
    /// 包装单（系统）
    /// </summary>
    public class PackingListSystem : SaleOrderSystemBase
    {
        [Description("成品")]
        public string FinishProduct { get; set; }
    }
}
