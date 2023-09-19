using Volo.Abp.Domain.Repositories;
using YaSha.DataManager.ProductInventory.AggregateRoot;

namespace YaSha.DataManager.Repository.ProductInventory;

public interface IProductInvMaterialRepository : IBasicRepository<ProductInventMaterial,Guid>
{
    Task DeleteMaterial(List<ProductInventMaterial> material);

    Task DeleteMaterial(List<Guid> ids);
    
    Task<List<ProductInventMaterial>> FindMaterialIndex(string system, string series, string name, string code, string sorting);
}