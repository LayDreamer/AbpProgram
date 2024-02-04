using Volo.Abp.Application.Dtos;

namespace YaSha.DataManager.ProductRetrieval.Dto;

public class ProductIndexSearchDto : PagedAndSortedResultRequestDto
{
    public string Key { get; set; }
    
    public string SearchValue { get; set; }

    public string SearchCode { get; set; }
    
    public string System { get; set; }

    public string Series { get; set; }

    public string Level { get; set; }

    public string CalculateCacheKey()
    {
        return "_" + this.System + "_" + this.Series + "_" + this.Level + "_" + this.SearchValue + "_" + this.SearchCode + "_" + this.Sorting + "_" + this.SkipCount + "_" + this.MaxResultCount;
    }
}