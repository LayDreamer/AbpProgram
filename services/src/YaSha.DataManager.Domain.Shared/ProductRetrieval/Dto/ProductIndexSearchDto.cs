using YaSha.DataManager.Common;

namespace YaSha.DataManager.ProductRetrieval.Dto;

public class ProductIndexSearchDto : OrderNotificationSearchDto
{
    public string System { get; set; }

    public string Series { get; set; }
}