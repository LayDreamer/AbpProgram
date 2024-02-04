using Volo.Abp.Domain.Repositories;
using YaSha.DataManager.ArchitectureList.AggregateRoot;

namespace YaSha.DataManager.ArchitectureList.Repository;

public interface IArchitectureListModuleFileRepository : IBasicRepository<ArchitectureListModuleFile, Guid>
{
    Task<List<ArchitectureListModuleFile>> InsertAndUpdate(Guid moduleId, List<Guid> fileIds);

    Task DeleteByModuleId(List<Guid> id);

    Task DeleteByFileId(List<Guid> id);

    Task<List<ArchitectureListModuleFile>> FindByModuleId(Guid id, bool include = true);
}