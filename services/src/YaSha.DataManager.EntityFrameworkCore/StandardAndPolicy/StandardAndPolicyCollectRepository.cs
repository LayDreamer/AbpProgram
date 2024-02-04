using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using YaSha.DataManager.EntityFrameworkCore;
using YaSha.DataManager.StandardAndPolicy.AggregateRoot;
using YaSha.DataManager.StandardAndPolicy.Repository;

namespace YaSha.DataManager.StandardAndPolicy;

public class StandardAndPolicyCollectRepository: EfCoreRepository<DataManagerDbContext, StandardAndPolicyCollect, Guid>,
    IStandardAndPolicyCollectRepository
{
    public StandardAndPolicyCollectRepository(IDbContextProvider<DataManagerDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }
    
    public async Task<StandardAndPolicyCollect> FindByUserIdAndLibId(Guid userId, Guid libId)
    {
        return await (await GetDbSetAsync()).Where(x => x.UserId.Equals(userId) && x.LibId.Equals(libId)).FirstOrDefaultAsync();
    }

    public async Task Delete(List<Guid> libIds)
    {
        var entity = (await GetDbSetAsync()).Where(x => libIds.Any(y => y.Equals(x.LibId)));
        await base.DeleteManyAsync(entity, autoSave: true);
    }
}