using Volo.Abp.Application.Dtos;
using YaSha.DataManager.ProductRetrieval;
using YaSha.DataManager.ProductRetrieval.Dto;

namespace YaSha.DataManager.Controllers;

[Route("ProductRetrieval")]
public class ProductRetrievalController : DataManagerController, IProductRetrievalAppService
{
    private readonly IProductRetrievalAppService _service;


    public ProductRetrievalController(IProductRetrievalAppService service)
    {
        _service = service;
    }
    
    [HttpPost("Page")]
    public async Task<PagedResultDto<ProductRetrievalDto>> PageProductRetrieval(ProductIndexSearchDto input)
    {
        return await _service.PageProductRetrieval(input);
    }

    [HttpPost("FindByInput")]
    public async Task<PagedResultDto<ProductRetrievalResultDto>> FindProductRetrievalByInput(ProductRetrievalSearchDto input)
    {
        return await _service.FindProductRetrievalByInput(input: input);
    }
}