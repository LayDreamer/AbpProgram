﻿using Volo.Abp.Application.Dtos;

namespace YaSha.DataManager.ProductInventory.Dto;

public class ProductInventoryMaterialDto : AuditedEntityDto<Guid>
{
    public string Name { get; set; }
    
    public string Code { get; set; }
    
    public string Length { get; set; }
    
    public string Width { get; set; }
    
    public string Height { get; set; }
    
    public string SupplyMode { get; set; }
    
    public string ModuleSpecification { get; set; }
    
    public string Property { get; set; }
    
    public string MaterialQuality { get; set; }
    
    public string Color { get; set; }
    
    public string Usage { get; set; }
    
    public string Unit { get; set; }
    
    public string IsProcess { get; set; }
    
    public string Remark { get; set; }
    
    public string System { get; set; }

    public string Series { get; set; }
    
    public string MaterialUsageFormula { get; set; }
}