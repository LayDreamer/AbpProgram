using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using YaSha.DataManager.EntityFrameworkCore;
using YaSha.DataManager.StandardAndPolicy.AggregateRoot;
using YaSha.DataManager.StandardAndPolicy.Repository;

namespace YaSha.DataManager.StandardAndPolicy;

public class StandardAndPolicyThemeRepository : EfCoreRepository<DataManagerDbContext, StandardAndPolicyTheme, Guid>,
    IStandardAndPolicyThemeRepository
{
    public StandardAndPolicyThemeRepository(IDbContextProvider<DataManagerDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<List<StandardAndPolicyTheme>> FindById(Guid id, bool includeLib = true, bool includeTree = false)
    {
        return await (await GetDbSetAsync())
            .IncludeStandardAndPolicyThemes(includeLib, includeTree)
            .Where(x => x.LibId == id)
            .ToListAsync();
    }

    public async Task<List<StandardAndPolicyTheme>> FindByTreeId(Guid id, bool includeLib = true, bool includeTree = false, bool maxSize = false, int size = 4)
    {
        var query = (await GetDbSetAsync())
            .IncludeStandardAndPolicyThemes(includeLib, includeTree)
            .Where(x => x.TreeId == id).OrderByDescending(x=>x.Lib.CreationTime);

        if (maxSize)
        {
            return await query.Take(size).ToListAsync();
        }
        return await query.ToListAsync();
    }

    public async Task<List<StandardAndPolicyTheme>> FindBySearch(Guid? id, string name, string number, string unit)
    {
        var query = (await GetDbSetAsync())
            .IncludeStandardAndPolicyThemes(true, false);

        if (id != null)
        {
            query = query.Where(x => x.TreeId.Equals(id));
        }

        if (!string.IsNullOrEmpty(number))
        {
            query = query.Where(x => x.Lib.Number.Contains(number));
        }

        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(unit))
        {
            query = query.Where(x => x.Lib.Name.Contains(name) || x.Lib.PublishingUnit.Contains(unit));
        }
        return await query.ToListAsync();
    }

    public async Task Delete(List<Guid> libIds)
    {
        var entity = (await GetDbSetAsync()).Where(x => libIds.Any(y => y.Equals(x.LibId)));
        await base.DeleteManyAsync(entity, autoSave: true);
    }
}