using YaSha.DataManager.ProductInventory;
using Volo.Abp.Application.Dtos;

namespace YaSha.DataManager.ProductRetrieval.Dto;

public class ProductRetrievalSearchDto : PagedAndSortedResultRequestDto
{
    //public Dictionary<string, ProductInventroyTag> SearchInfo { get; set; } = new();

    public List<SearchInfoDto> SearchInfo { get; set; } = new();

    public string CalculateCacheKey()
    {
        var results = "_" + this.Sorting + "_" + this.SkipCount + "_" + this.MaxResultCount;

        foreach (var item in SearchInfo)
        {
            results += ("_" + item.Code + "_" + item.Tag);
        }

        return results;
    }
}

public class SearchInfoDto
{
    public int Index { get; set; }
    public string Code { get; set; }
    public ProductInventroyTag Tag { get; set; }
}