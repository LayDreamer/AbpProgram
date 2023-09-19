using JetBrains.Annotations;
using Volo.Abp.Application.Dtos;
using YaSha.DataManager.Common;
using YaSha.DataManager.ProductInventory.Dto;
using YaSha.DataManager.ProductRetrieval.Dto;

namespace YaSha.DataManager.ProductInventory;

public interface IProductIventProductAppService : IApplicationService
{
    Task<ProductInventoryProductDto> Insert(Guid id, ProductInventoryProductCreateDto dto);

    Task<List<ProductInventoryProductDto>> ImportProductFromExcel(string system, string series,
        List<ProductInventoryProductCreateDto> dtos);
    
    Task<List<ProductInventoryProductDto>> AddProducts(List<ProductInventoryFullDto> dtos);

    Task<List<ProductInventoryProductDto>> CopyProducts(List<Guid> ids);
    
    Task<string> DeleteProducts(List<Guid> ids);
    Task<string> DeleteProductEdits(List<ProductInventoryFullDto> dtos);
    
    Task<ProductInventoryProductDto> UpdateProduct(ProductInventoryFullDto dto);

    Task<List<ProductInventoryProductDto>> PublishProducts(Dictionary<Guid, ProductInventoryPublishStatus> dto);
    
    Task<PagedResultDto<ProductInventoryProductDto>> PageProducts(OrderNotificationSearchDto input);
    
    Task<ProductInventoryFullDto> GetProductDetails(Guid id);
}