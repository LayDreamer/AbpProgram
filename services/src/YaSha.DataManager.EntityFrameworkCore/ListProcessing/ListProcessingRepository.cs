using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using YaSha.DataManager.EntityFrameworkCore;

namespace YaSha.DataManager.ListProcessing;

public class ListProcessingRepository: EfCoreRepository<DataManagerDbContext, ListProcessing, Guid>,
    IListProcessingRepository
{
    public ListProcessingRepository(IDbContextProvider<DataManagerDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<List<ListProcessing>> Page(Guid? userId, string search, string sorting)
    {
        var query = (await GetDbSetAsync())
            .WhereIf(userId != null, x => x.CreatorId.Equals(userId.Value))
            .WhereIf(!string.IsNullOrEmpty(search), x => x.Name.Contains(search));
        
        if (!string.IsNullOrEmpty(sorting))
        {
            query = query.OrderBy(sorting);
        }
        else
        {
            query = query.OrderByDescending(
                (Expression<Func<ListProcessing, DateTime>>)(e => ((IHasCreationTime)e).CreationTime));
        }

        return await query.ToListAsync();
    }

    public async Task<List<ListProcessing>> FindByIds(List<Guid> ids)
    {
        var entity = (await GetDbSetAsync()).Where(x => ids.Any(y => y.Equals(x.Id)));
        return await entity.ToListAsync();
    }

    public async Task<ListProcessing> FindByName(string name)
    {
        return await (await GetDbSetAsync()).FirstOrDefaultAsync(x => x.Name.Equals(name));
    }
}