using Volo.Abp.Application.Dtos;
using YaSha.DataManager.ProductRetrieval.Dto;
using YaSha.DataManager.ProductRetrieval.Manager;

namespace YaSha.DataManager.ProductRetrieval;

[Authorize(DataManagerPermissions.ProductRetrieval.Default)]
public class ProductRetrievalAppService : DataManagerAppService, IProductRetrievalAppService, ITransientDependency
{
    
    private readonly ProductRetrievalManager _productService;

    public ProductRetrievalAppService(ProductRetrievalManager productService)
    {
        _productService = productService;
    }

    public async Task<PagedResultDto<ProductRetrievalDto>> PageProductRetrieval(ProductIndexSearchDto input)
    {
        return await _productService.PageProductRetrieval(input);
    }

    public async Task<PagedResultDto<ProductRetrievalResultDto>> FindProductRetrievalByInput(ProductRetrievalSearchDto input)
    {
        return await _productService.FindProductRetrievalByInputs(input);
    }
}