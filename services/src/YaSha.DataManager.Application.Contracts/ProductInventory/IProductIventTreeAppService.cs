using YaSha.DataManager.ProductInventory.Dto;

namespace YaSha.DataManager.ProductInventory;

public interface IProductIventTreeAppService : IApplicationService
{
    Task<List<ProductInventoryTreeDto>> GetRoot();

    Task<ProductInventoryTreeDto> UpdateRemark(Guid id, string remark);
}