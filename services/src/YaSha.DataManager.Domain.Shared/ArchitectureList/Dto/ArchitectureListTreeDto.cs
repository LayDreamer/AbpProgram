namespace YaSha.DataManager.ArchitectureList.Dto;

public class ArchitectureListTreeDto
{
    public Guid Id { get; set; }
    
    public Guid? ParentId { get; set; }
    
    public string Name { get; set; }
    
    public string Data { get; set; }
    public List<ArchitectureListTreeDto> Children { get; set; }
    
    public ArchitectureListTreeDto()
    {
        Children = new List<ArchitectureListTreeDto>();
    }
}