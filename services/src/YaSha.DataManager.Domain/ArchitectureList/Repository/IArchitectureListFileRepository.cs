using Volo.Abp.Domain.Repositories;
using YaSha.DataManager.ArchitectureList.AggregateRoot;

namespace YaSha.DataManager.ArchitectureList.Repository;

public interface IArchitectureListFileRepository : IBasicRepository<ArchitectureListFile, Guid>
{
    Task<List<ArchitectureListFile>> FindByStatus(Guid treeId, ArchitectureListFileStatus status);

    Task Delete(List<Guid> ids);

    Task<List<ArchitectureListFile>> FindByIds(List<Guid> ids);
}