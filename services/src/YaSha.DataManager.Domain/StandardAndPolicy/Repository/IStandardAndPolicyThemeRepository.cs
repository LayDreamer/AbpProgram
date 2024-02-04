using Volo.Abp.Domain.Repositories;
using YaSha.DataManager.StandardAndPolicy.AggregateRoot;

namespace YaSha.DataManager.StandardAndPolicy.Repository;

public interface IStandardAndPolicyThemeRepository : IBasicRepository<StandardAndPolicyTheme, Guid>
{
    Task<List<StandardAndPolicyTheme>> FindById(Guid id, bool includeLib = true, bool includeTree = false);

    Task<List<StandardAndPolicyTheme>> FindByTreeId(Guid id, bool includeLib = true, bool includeTree = false, bool maxSize = false, int size = 4);

    Task<List<StandardAndPolicyTheme>> FindBySearch(Guid? id, string name, string number, string unit);

    Task Delete(List<Guid> libIds);
}