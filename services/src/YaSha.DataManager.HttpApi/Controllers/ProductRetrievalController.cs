using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using Volo.Abp.Application.Dtos;
using YaSha.DataManager.ProductRetrieval;
using YaSha.DataManager.ProductRetrieval.Dto;
using YaSha.DataManager.StandardAndPolicy.Dto;

namespace YaSha.DataManager.Controllers;

/// <summary>
/// 产品检索
/// </summary>
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

    [HttpPost("NoPage")]
    public async Task<List<ProductRetrievalDto>> NoPageProductRetrieval(ProductIndexSearchDto input)
    {
        return await _service.NoPageProductRetrieval(input);
    }


    [HttpPost("FindByInput")]
    public async Task<PagedResultDto<ProductRetrievalResultDto>> FindProductRetrievalByInput(ProductRetrievalSearchDto input)
    {
        return await _service.FindProductRetrievalByInput(input: input);
    }

    [HttpPost("FindAllByInput")]
    public async Task<List<ProductRetrievalResultDto>> FindAllProductRetrievalByInput(ProductRetrievalSearchDto input)
    {
        return await _service.FindAllProductRetrievalByInput(input: input);
    }


    [HttpPost("ImportFromExcel")]
    public async Task<ApiResultDto> ImportElementsFromExcel(IFormFile file)
    {
        return await _service.ImportElementsFromExcel(file);
    }

    [HttpPost("ExportToExcel")]
    public async Task<byte[]> ExportElementsToExcel(List<ProductRetrievalResultDto> input)
    {
      return await _service.ExportElementsToExcel(input);
    }
}