using Volo.Abp.Domain.Repositories;
using YaSha.DataManager.ArchitectureList.AggregateRoot;

namespace YaSha.DataManager.ArchitectureList.Repository;

public interface IArchitectureListTreeRepository : IBasicRepository<ArchitectureListTree, Guid>
{
    Task InitTree(List<ArchitectureListTree> roots);
    Task<List<ArchitectureListTree>> GetRootTree(bool include = true);
    Task<List<ArchitectureListTree>> GetChildren(Guid id);
    Task<ArchitectureListTree> GetTreeByName(string name, bool include = true);
    Task<string> GetTreePath(Guid id);
}