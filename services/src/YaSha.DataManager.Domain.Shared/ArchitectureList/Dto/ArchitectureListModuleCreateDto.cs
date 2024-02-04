namespace YaSha.DataManager.ArchitectureList.Dto;

public class ArchitectureListModuleCreateDto
{
    public string System { get; set; }
    
    public string Name { get; set; }
    
    public string Category { get; set; }
    
    public string Model { get; set; }
    
    public string ProcessingMode { get; set; }
    
    public string Length { get; set; }

    public string Width { get; set; }

    public string Height { get; set; }
    
    public string Unit { get; set; }
    
    public string ProcessingCode { get; set; }
    
    public string SupplyMode { get; set; }
    
    public string ModuleSpecification { get; set; }
    
    public string ProcessNum { get; set; }

    public string AssemblyDrawingNum { get; set; }

    public string DetailNum { get; set; }
    
    public string Remark { get; set; }
    
    public string Optional { get; set; }
    public List<ArchitectureListMaterialCreateDto> Materials { get; set; }

    public ArchitectureListModuleCreateDto()
    {
        Materials = new List<ArchitectureListMaterialCreateDto>();
    }
}