using Volo.Abp.Domain.Repositories;

namespace YaSha.DataManager.ListProcessing;

public interface IListProcessingRepository: IBasicRepository<ListProcessing, Guid>
{
    Task<List<ListProcessing>> Page(Guid? userId, string search, string sorting);

    Task<List<ListProcessing>> FindByIds(List<Guid> ids);


    Task<ListProcessing> FindByName(string name);
}