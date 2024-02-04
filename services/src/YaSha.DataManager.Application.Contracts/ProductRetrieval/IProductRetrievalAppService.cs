using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;
using Volo.Abp.Application.Dtos;
using YaSha.DataManager.ProductRetrieval.Dto;
using YaSha.DataManager.StandardAndPolicy.Dto;

namespace YaSha.DataManager.ProductRetrieval;

public interface IProductRetrievalAppService:IApplicationService
{
    Task<PagedResultDto<ProductRetrievalDto>> PageProductRetrieval(ProductIndexSearchDto input);

    Task<List<ProductRetrievalDto>> NoPageProductRetrieval(ProductIndexSearchDto input);

    Task<PagedResultDto<ProductRetrievalResultDto>> FindProductRetrievalByInput(ProductRetrievalSearchDto input);
    Task<List<ProductRetrievalResultDto>> FindAllProductRetrievalByInput(ProductRetrievalSearchDto input);

    Task<ApiResultDto> ImportElementsFromExcel(IFormFile file);
    Task<byte[]> ExportElementsToExcel(List<ProductRetrievalResultDto> input);
}