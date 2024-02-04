using Volo.Abp.Domain.Repositories;
using YaSha.DataManager.StandardAndPolicy.AggregateRoot;

namespace YaSha.DataManager.StandardAndPolicy.Repository;

public interface IStandardAndPolicyCollectRepository : IBasicRepository<StandardAndPolicyCollect, Guid>
{
    Task<StandardAndPolicyCollect> FindByUserIdAndLibId(Guid userId, Guid libId);

    Task Delete(List<Guid> libIds);
}