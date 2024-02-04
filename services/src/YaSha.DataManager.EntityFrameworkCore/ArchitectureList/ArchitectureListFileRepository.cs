using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using YaSha.DataManager.ArchitectureList.AggregateRoot;
using YaSha.DataManager.ArchitectureList.Repository;
using YaSha.DataManager.EntityFrameworkCore;

namespace YaSha.DataManager.ArchitectureList;

public class ArchitectureListFileRepository : EfCoreRepository<DataManagerDbContext, ArchitectureListFile, Guid>,
    IArchitectureListFileRepository
{
    public ArchitectureListFileRepository(IDbContextProvider<DataManagerDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<List<ArchitectureListFile>> FindByStatus(Guid treeId, ArchitectureListFileStatus status)
    {
        return await (await GetDbSetAsync()).Where(x => x.TreeId.Equals(treeId) && x.Type.Equals(status)).ToListAsync();
    }

    public async Task Delete(List<Guid> ids)
    {
        var ent = (await GetDbSetAsync()).Where(x => ids.Any(y => y.Equals(x.Id)));
        await DeleteManyAsync(ent, true);
    }

    public async Task<List<ArchitectureListFile>> FindByIds(List<Guid> ids)
    {
        return await (await GetDbSetAsync()).Where(x => ids.Any(y => y.Equals(x.Id))).ToListAsync();
    }
}