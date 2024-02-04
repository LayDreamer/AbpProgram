using Volo.Abp.Domain.Repositories;
using YaSha.DataManager.ProductRetrieval.AggregateRoot;

namespace YaSha.DataManager.ProductRetrieval.Repository;

public interface IProjectInfoRepository: IBasicRepository<ProjectInfo,Guid>
{
    Task InsertBulk(List<ProjectInfo> input);

    Task DeleteBulk(List<ProjectInfo> input);

    Task<List<ProjectInfo>> FindByCodeAndType(string code, ProjectInfoInputType type);
}