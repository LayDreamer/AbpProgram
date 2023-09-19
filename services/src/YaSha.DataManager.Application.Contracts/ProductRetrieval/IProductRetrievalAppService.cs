using Volo.Abp.Application.Dtos;
using YaSha.DataManager.ProductRetrieval.Dto;

namespace YaSha.DataManager.ProductRetrieval;

public interface IProductRetrievalAppService:IApplicationService
{
    Task<PagedResultDto<ProductRetrievalDto>> PageProductRetrieval(ProductIndexSearchDto input);
    
    Task<PagedResultDto<ProductRetrievalResultDto>> FindProductRetrievalByInput(ProductRetrievalSearchDto input);
}