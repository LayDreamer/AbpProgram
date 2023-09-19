namespace YaSha.DataManager.ProductInventory.Dto;

public class ProductInventoryModuleCreateDto
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
    public List<ProductInventoryMaterialCreateDto> Materials { get; set; }

    public ProductInventoryModuleCreateDto()
    {
        Materials = new List<ProductInventoryMaterialCreateDto>();
    }
}