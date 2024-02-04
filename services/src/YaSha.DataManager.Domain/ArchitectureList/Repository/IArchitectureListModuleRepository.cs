using System.Diagnostics;
using System.Xml.Linq;
using Volo.Abp.Domain.Repositories;
using YaSha.DataManager.ArchitectureList.AggregateRoot;
using YaSha.DataManager.ProductInventory;

namespace YaSha.DataManager.ArchitectureList.Repository;

public interface IArchitectureListModuleRepository : IBasicRepository<ArchitectureListModule, Guid>
{
    Task<ArchitectureListModule> FindById(Guid id, bool include = true);
    Task<List<ArchitectureListModule>> FindByIds(List<Guid> ids, bool include = true);
    Task<ArchitectureListModule> FindByModel(string model, string system, bool include = true);
    Task<ArchitectureListModule> Exists(Guid treeId, string name, string code, string processingMode, bool include = true);

    Task<Tuple<int, List<ArchitectureListModule>>> PageModule(
        List<Guid> ids,
        string value,
        string code,
        ArchitectureListPublishStatus status,
        string sorting,
        int skipCount,
        int maxCount,
        bool include = true);

    Task<ArchitectureListModule> FindByNameAndProcess(string name, string process, bool include = true);

    Task<ArchitectureListModule>  FindByTypeAndProcess(string name, string process, bool include = false);

}