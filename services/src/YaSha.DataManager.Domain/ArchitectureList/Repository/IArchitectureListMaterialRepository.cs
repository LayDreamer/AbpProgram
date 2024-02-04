using Volo.Abp.Domain.Repositories;
using YaSha.DataManager.ArchitectureList.AggregateRoot;

namespace YaSha.DataManager.ArchitectureList.Repository;

public interface IArchitectureListMaterialRepository : IBasicRepository<ArchitectureListMaterial, Guid>
{
    
}