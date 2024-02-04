using YaSha.DataManager.ProductRetrieval.AggregateRoot;
using YaSha.DataManager.ProductRetrieval.Dto;
using YaSha.DataManager.ProductRetrieval.Repository;

namespace YaSha.DataManager.ProductRetrieval.Manager;

public class MaterialInventoryManager : DataManagerDomainService
{
    private readonly IMaterialInventoryRepository _repository;

    public MaterialInventoryManager(IMaterialInventoryRepository repository)
    {
        _repository = repository;
    }
    
    public async Task Insert(List<MaterialInventoryCreateDto> input)
    {
        var ent = (await _repository.GetListAsync()).ToList();
        await _repository.DeleteBulk(ent);
        var materialInventory = ObjectMapper.Map<List<MaterialInventoryCreateDto>, List<MaterialInventory>>(input);
        materialInventory.ForEach(x=>x.SetId());
        await _repository.InsertBulk(materialInventory);
    }


    public async Task<List<MaterialInventoryDto>> Find(List<string> code)
    {
        var dbData = await _repository.FindByMaterialCode(code);
        return ObjectMapper.Map<List<MaterialInventory>, List<MaterialInventoryDto>>(dbData);
    }
}