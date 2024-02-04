using Volo.Abp.Application.Dtos;

namespace YaSha.DataManager.ProductInventory.Dto;

public class ProductInventoryEditDto : AuditedEntityDto<Guid>
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
    public string Property { get; set; }
    public string MaterialQuality { get; set; }
    public string ProductSpecification { get; set; }
    public string ProcessNum { get; set; }
    public string AssemblyDrawingNum { get; set; }
    public string DetailNum { get; set; }
    public string Color { get; set; }
    public string Usage { get; set; }
    public string Unit { get; set; }
    public string IsProcess { get; set; }
    public string Remark { get; set; }
    public string System { get; set; }
    public string Series { get; set; }
    public string ProjectCode { get; set; }
    public string ProjectName { get; set; }
    public string LimitInfos { get; set; }
    public bool InModuleMaterial { get; set; }
    
    public string CreateUser { get; set; }
    
    public string ModifyUser { get; set; }

    public string MaterialUsageFormula { get; set; }

    public string UnitBase { get; set; }
}