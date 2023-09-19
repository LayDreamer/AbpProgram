using Volo.Abp.Application.Dtos;
using Volo.Abp.Auditing;
namespace YaSha.DataManager.ProductInventory.Dto;

public class ProductInventoryProductDto  : AuditedEntityDto<Guid>
{
    public string Name { get; set; }
    
    public string Code { get; set; }
    
    public string Category { get; set; }
    
    public string Version { get; set; }

    public string Length { get; set; }
    
    public string Width { get; set; }
    
    public string Height { get; set; }
    
    public string MaterialQuality { get; set; }
    
    public string Color { get; set; }
    
    public string ProductSpecification { get; set; }
    
    public string ProcessNum { get; set; }
    
    public string AssemblyDrawingNum { get; set; }
    
    public string DetailNum { get; set; }
    
    public string Remark { get; set; }
    
    public string CreateUser { get; set; }
    
    public string ModifyUser { get; set; }
    
    public string System { get; set; }
    
    public string Series { get; set; }
    
    public string ProjectCode { get; set; }
    
    public string ProjectName { get; set; }
    
    public string LimitInfos { get; set; }
    
    public ProductInventoryPublishStatus Status { get; set; }
    public List<ProductInventoryModuleDto> Modules { get; set; }
    public List<ProductInventoryMaterialDto> Materials { get; set; }

    public ProductInventoryProductDto()
    {
        Modules = new List<ProductInventoryModuleDto>();
        Materials = new List<ProductInventoryMaterialDto>();
    }
}