using Volo.Abp.Domain.Repositories;
using YaSha.DataManager.ProductInventory.AggregateRoot;
using YaSha.DataManager.ProductInventory.Dto;

namespace YaSha.DataManager.Repository.ProductInventory;

public interface IProductInvTreeRepository : IBasicRepository<ProductInventTree,Guid>
{
    Task InitProductInvTree(ProductInventTree root);
    Task<List<ProductInventTree>> GetRootTree(bool include = true);
    
    Task<List<ProductInventTree>> GetChildren(Guid id);
}