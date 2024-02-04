using Volo.Abp.Domain.Repositories;
using YaSha.DataManager.ProductInventory.AggregateRoot;

namespace YaSha.DataManager.ProductInventory.Repository;

public interface IProductInvModuleRepository : IBasicRepository<ProductInventModule,Guid>
{
    Task DeleteModule(List<ProductInventModule> material);

    Task DeleteModule(List<Guid> ids);

    Task<List<ProductInventModule>> FindByCode(string code, bool include = false);
    
    Task<List<ProductInventModule>> FindModuleIndex(string system, string series, string name, string code, string sorting, bool include = false);
}