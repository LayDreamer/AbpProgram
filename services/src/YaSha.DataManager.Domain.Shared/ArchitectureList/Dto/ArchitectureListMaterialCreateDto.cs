namespace YaSha.DataManager.ArchitectureList.Dto;

public class ArchitectureListMaterialCreateDto
{
    public string Composition { get; set; }
    
    public string Name { get; set; }

    public string Code { get; set; }

    public string Length { get; set; }

    public string Width { get; set; }

    public string Height { get; set; }
    
    public string MaterialQuality { get; set; }
    
    
    public string BasicPerformance { get; set; }
    
    public string Usage { get; set; }
    
    public string Unit { get; set; }
    
    public string IsProcess { get; set; }
    
    public string InstallationCode { get; set; }
    
    public string Remark { get; set; }
    
    public string OptionalSerial { get; set; } 
    
    public ArchitectureListMaterialTag Tag { get; set; }
}