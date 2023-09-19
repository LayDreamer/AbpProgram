using Volo.Abp;
using YaSha.DataManager.ProductInventory.AggregateRoot;
using YaSha.DataManager.ProductInventory.Dto;
using YaSha.DataManager.Repository.ProductInventory;

namespace YaSha.DataManager.ProductInventory.Manager;

public class ProductInventTreeManager : DataManagerDomainService
{
    private readonly IProductInvTreeRepository _repository;
    
    public ProductInventTreeManager(IProductInvTreeRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ProductInventoryTreeDto>> GetRootTree()
    {
        var roots = await _repository.GetRootTree();
        return ObjectMapper.Map<List<ProductInventTree>, List<ProductInventoryTreeDto>>(roots);
    }

    public async Task<ProductInventoryTreeDto> UpdateRemark(Guid id, string remark)
    {
        var root = await _repository.FindAsync(id, false);
        Check.NotNull(root, "输入id有误");
        root.Remark = remark;
        var result = await _repository.UpdateAsync(root);
        return ObjectMapper.Map<ProductInventTree, ProductInventoryTreeDto>(result);
    }
}