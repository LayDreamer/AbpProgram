using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using YaSha.DataManager.ArchitectureList.AggregateRoot;
using YaSha.DataManager.ArchitectureList.Repository;
using YaSha.DataManager.EntityFrameworkCore;

namespace YaSha.DataManager.ArchitectureList;

public class ArchitectureListModuleFileRepository : EfCoreRepository<DataManagerDbContext, ArchitectureListModuleFile, Guid>,
    IArchitectureListModuleFileRepository
{
    public ArchitectureListModuleFileRepository(IDbContextProvider<DataManagerDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }
    
    public async Task<List<ArchitectureListModuleFile>> InsertAndUpdate(Guid moduleId, List<Guid> fileIds)
    {
        var finds = await (await GetDbSetAsync())
            .IncludeModuleFileDetails(false)
            .Where(x => x.ModuleId.Equals(moduleId))
            .ToListAsync();

        if (0 == finds.Count)
        {
            var inserts = new List<ArchitectureListModuleFile>();
            foreach (var id in fileIds)
            {
                var tmp = new ArchitectureListModuleFile()
                {
                    ModuleId = moduleId,
                    FileId = id
                };
                inserts.Add(await InsertAsync(tmp, true));
            }

            return inserts;
        }
        else
        {
            var inserts = new List<ArchitectureListModuleFile>();
            foreach (var id in fileIds)
            {
                var tmp = finds.FirstOrDefault(x => x.FileId.Equals(id));
                if (tmp == null)
                {
                    inserts.Add(await InsertAsync(new ArchitectureListModuleFile
                    {
                        ModuleId = moduleId,
                        FileId = id
                    }, true));
                }
                else
                {
                    inserts.Add(tmp);
                }
            }

            var deletes = finds.Where(x => !fileIds.Any(y => y.Equals(x.FileId))).ToList();

            if (deletes.Count > 0)
            {
                await DeleteManyAsync(deletes, true);
            }
            
            return inserts;
        }
    }

    public async Task DeleteByModuleId(List<Guid> id)
    {
        var finds = await (await GetDbSetAsync())
            .IncludeModuleFileDetails(false)
            .Where(x => id.Any(y=>y.Equals(x.ModuleId)))
            .ToListAsync();

        if (finds.Count > 0)
        {
            await DeleteManyAsync(finds, true);
        }
    }

    public async Task DeleteByFileId(List<Guid> id)
    {
        var finds = await (await GetDbSetAsync())
            .IncludeModuleFileDetails(false)
            .Where(x => id.Any(y=>y.Equals(x.FileId)))
            .ToListAsync();

        if (finds.Count > 0)
        {
            await DeleteManyAsync(finds, true);
        }
    }

    public async Task<List<ArchitectureListModuleFile>> FindByModuleId(Guid id, bool include = true)
    {
        return await (await GetDbSetAsync())
            .IncludeModuleFileDetails(include)
            .Where(x => x.ModuleId.Equals(id))
            .ToListAsync();
    }
}