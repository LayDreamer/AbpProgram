using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using YaSha.DataManager.EntityFrameworkCore;
using YaSha.DataManager.StandardAndPolicy.AggregateRoot;
using YaSha.DataManager.StandardAndPolicy.Repository;

namespace YaSha.DataManager.StandardAndPolicy;

public class StandardAndPolicyLibRepository : EfCoreRepository<DataManagerDbContext, StandardAndPolicyLib, Guid>,
    IStandardAndPolicyLibRepository
{
    public StandardAndPolicyLibRepository(IDbContextProvider<DataManagerDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<List<StandardAndPolicyLib>> Page(string type, int category, string search, string province, string sorting)
    {
        var query = (await GetDbSetAsync())
            .WhereIf(!string.IsNullOrEmpty(type), x => x.Type.Equals(type))
            .WhereIf(category != 0, x => x.StandardCategory.Equals((StandardAndPolicyCategory)category))
            .WhereIf(!string.IsNullOrEmpty(province) && province != "全部", x => x.PublishingUnit.Contains(province));

        if (!string.IsNullOrEmpty(search))
        {
            var terms = search.ToCharArray().Select(c => c.ToString()).ToList();
            Expression<Func<StandardAndPolicyLib, bool>> expression1 = ex => true;
            Expression<Func<StandardAndPolicyLib, bool>> expression2 = ex => true;
            Expression<Func<StandardAndPolicyLib, bool>> expression3 = ex => true;
            foreach (var term in terms)
            {
                //分词模糊匹配 上面True改false
                // expression1 = expression1.Or(x => x.Name.Contains(term));
                // expression2 = expression2.Or(x => x.PublishingUnit.Contains(term));
                // expression3 = expression3.Or(x => x.Number.Contains(term));

                //分词全匹配 
                expression1 = expression1.And(x => x.Name.Contains(term));
                expression2 = expression2.And(x => x.PublishingUnit.Contains(term));
                expression3 = expression3.And(x => x.Number.Contains(term));
            }

            query = query.Where(expression1.Or(expression2).Or(expression3));
        }

        if (!string.IsNullOrEmpty(sorting))
        {
            query = query.OrderBy(sorting);
        }
        else
        {
            query = query.OrderByDescending<StandardAndPolicyLib, DateTime>(
                (Expression<Func<StandardAndPolicyLib, DateTime>>)(e => ((IHasCreationTime)e).CreationTime));
        }

        return await query.ToListAsync();
    }

    public async Task<List<StandardAndPolicyLib>> FindLibsByIds(List<Guid> libIds)
    {
        var entity = (await GetDbSetAsync()).Where(x => libIds.Any(y => y.Equals(x.Id)));
        return await entity.ToListAsync();
    }
}