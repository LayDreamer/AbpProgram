using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using YaSha.DataManager.ProductInventory.Dto;
using YaSha.DataManager.ProductRetrieval.Dto;

namespace YaSha.DataManager.ProductInventory;

public interface IProductIventProductAppService : IApplicationService
{
    Task<ProductInventoryProductDto> Insert(Guid id, ProductInventoryProductCreateDto dto);

    Task<List<ProductInventoryProductDto>> ImportProductFromExcel(string system, string series,string seriesRemark,
        List<ProductInventoryProductCreateDto> dtos);
    
    Task<List<ProductInventoryProductDto>> AddProducts(List<ProductInventoryFullDto> dtos);

    Task<List<ProductInventoryProductDto>> CopyProducts(List<Guid> ids);
    
    Task<string> DeleteProducts(List<Guid> ids);
    Task<string> DeleteProductEdits(List<ProductInventoryFullDto> dtos);
    
    Task<ProductInventoryProductDto> UpdateProduct(ProductInventoryFullDto dto);

    Task<ImageResponseDto> UploadProductImage([FromForm] ImageFileDto dto);
    
    Task<List<ProductInventoryProductDto>> PublishProducts(Dictionary<Guid, ProductInventoryPublishStatus> dto);
    
    Task<PagedResultDto<ProductInventoryProductDto>> PageProducts(OrderNotificationSearchDto input);
    
    Task<ProductInventoryFullDto> GetProductDetails(Guid id);

    Task<string> GetErpTokenAsync();



    Task<List<ProductInventoryEditDto>> GetErpData(string accessToken, string postData);

}