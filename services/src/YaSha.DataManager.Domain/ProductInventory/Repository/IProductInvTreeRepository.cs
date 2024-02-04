using Volo.Abp.Domain.Repositories;
using YaSha.DataManager.ProductInventory.AggregateRoot;

namespace YaSha.DataManager.ProductInventory.Repository;

public interface IProductInvTreeRepository : IBasicRepository<ProductInventTree,Guid>
{
    Task InitProductInvTree(List<ProductInventTree> roots);
    Task<List<ProductInventTree>> GetRootTree(bool include = true);
    Task<List<ProductInventTree>> GetChildren(Guid id);
    Task<ProductInventTree> GetTreeByName(string name, bool include = false);
}