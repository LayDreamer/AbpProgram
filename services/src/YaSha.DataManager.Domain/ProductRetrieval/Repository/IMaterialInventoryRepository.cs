using Volo.Abp.Domain.Repositories;
using YaSha.DataManager.ProductRetrieval.AggregateRoot;

namespace YaSha.DataManager.ProductRetrieval.Repository;

public interface IMaterialInventoryRepository: IBasicRepository<MaterialInventory,Guid>
{
    Task InsertBulk(List<MaterialInventory> input);

    Task DeleteBulk(List<MaterialInventory> input);
    
    Task<List<MaterialInventory>> FindByMaterialCode(List<string> code);
}