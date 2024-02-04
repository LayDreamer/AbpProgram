using YaSha.DataManager.ProductRetrieval.AggregateRoot;
using YaSha.DataManager.ProductRetrieval.Dto;
using YaSha.DataManager.ProductRetrieval.Repository;

namespace YaSha.DataManager.ProductRetrieval.Manager;

public class ProjectInfoManager : DataManagerDomainService
{
    private readonly IProjectInfoRepository _repository;

    public ProjectInfoManager(IProjectInfoRepository repository)
    {
        _repository = repository;
    }

    public async Task Insert(List<ProjectInfoCreateDto> input)
    {
        var ent = (await _repository.GetListAsync()).ToList();
        await _repository.DeleteBulk(ent);
        var materialInventory = ObjectMapper.Map<List<ProjectInfoCreateDto>, List<ProjectInfo>>(input);
        materialInventory.ForEach(x => x.SetId());
        await _repository.InsertBulk(materialInventory);
    }

    public async Task<List<ProjectInfoDto>> FindByCodeAndType(string code, ProjectInfoInputType type)
    {
        var db = await _repository.FindByCodeAndType(code, type);
        return ObjectMapper.Map<List<ProjectInfo>, List<ProjectInfoDto>>(db);
    }
}