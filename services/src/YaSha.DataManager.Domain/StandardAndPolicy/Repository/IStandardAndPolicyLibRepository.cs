using Volo.Abp.Domain.Repositories;
using YaSha.DataManager.StandardAndPolicy.AggregateRoot;

namespace YaSha.DataManager.StandardAndPolicy.Repository;

public interface IStandardAndPolicyLibRepository : IBasicRepository<StandardAndPolicyLib, Guid>
{
    Task<List<StandardAndPolicyLib>> Page(string type, int category, string search, string province, string sorting);

    Task<List<StandardAndPolicyLib>> FindLibsByIds(List<Guid> libIds);
}