using Volo.Abp.Application.Dtos;

namespace YaSha.DataManager.ProductInventory.Dto;

public class OrderNotificationSearchDto : PagedAndSortedResultRequestDto
{
    public string Key { get; set; }
    
    public string SearchValue { get; set; }

    public string SearchCode { get; set; }
    
    public ProductInventoryPublishStatus Status { get; set; }
}