using YaSha.DataManager.ProductInventory;
using Volo.Abp.Application.Dtos;

namespace YaSha.DataManager.ProductRetrieval.Dto;

public class ProductRetrievalSearchDto : PagedAndSortedResultRequestDto
{
    public Dictionary<Guid, ProductInventroyTag> SearchInfo { get; set; } = new();
}