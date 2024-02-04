using Volo.Abp.Application.Dtos;

namespace YaSha.DataManager.ListProcessing;

public class ListProcessingManager : DataManagerDomainService
{
    private readonly IListProcessingRepository _repository;

    public ListProcessingManager(IListProcessingRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResultDto<ListProcessingDto>> Page(Guid? userId, string search, string sorting, int skip, int maxCount)
    {
        var results = await _repository.Page(userId, search, sorting);

        var totalCount = results.Count;

        results = results.Skip((skip - 1) * maxCount).Take(maxCount).ToList();

        return new PagedResultDto<ListProcessingDto>
        {
            Items = ObjectMapper.Map<List<ListProcessing>,List<ListProcessingDto>>(results),
            TotalCount = totalCount,
        };
    }
    
    public async Task<ListProcessingDto> CreateAndUpdate(string name)
    {
        var find = await _repository.FindByName(name);
        if (find == null)
        {
            find = new ListProcessing()
            {
                Name = name,
                FilePath = "https://bds.chinayasha.com/bdsfileservice/ListProcessing/Result/" + name
            };
            await _repository.InsertAsync(find, autoSave: true);
        }
        else
        {
            await _repository.UpdateAsync(find, autoSave: true);
        }

        return ObjectMapper.Map<ListProcessing, ListProcessingDto>(find);
    }
    
    public async Task<List<ListProcessingDto>> Delete(List<Guid> ids)
    {
        var ent = await _repository.FindByIds(ids);
        await _repository.DeleteManyAsync(ent, autoSave: true);
        return ObjectMapper.Map<List<ListProcessing>, List<ListProcessingDto>>(ent);
    }
}