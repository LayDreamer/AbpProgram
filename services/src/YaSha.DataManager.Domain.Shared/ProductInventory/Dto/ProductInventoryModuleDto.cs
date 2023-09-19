using Volo.Abp.Application.Dtos;

namespace YaSha.DataManager.ProductInventory.Dto;

public class ProductInventoryModuleDto : AuditedEntityDto<Guid>
{
    public string Level { get; set; }
    
    public string Name { get; set; }
    
    public string Code { get; set; }
    
    public string Category { get; set; }
    
    public string Version { get; set; }
    
    public string Length { get; set; }
    
    public string Width { get; set; }
    
    public string Height { get; set; }
    
    public string SupplyMode { get; set; }
    
    public string ModuleSpecification { get; set; }

    public string LimitInfos { get; set; }
    
    public string System { get; set; }

    public string Series { get; set; }
    public List<ProductInventoryMaterialDto> Materials { get; set; }

    public ProductInventoryModuleDto()
    {
        Materials = new List<ProductInventoryMaterialDto>();
    }
}