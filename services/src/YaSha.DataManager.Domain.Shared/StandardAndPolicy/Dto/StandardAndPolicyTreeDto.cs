namespace YaSha.DataManager.StandardAndPolicy.Dto;

public class StandardAndPolicyTreeDto
{
    public Guid Id { get; set; }
    
    public Guid? ParentId { get; set; }
    
    public int Code { get; set; }
    
    public string Name { get; set; }
    
    public bool Hide { get; set; }
    public List<StandardAndPolicyTreeDto> Children { get; set; }
    
    public StandardAndPolicyTreeDto()
    {
        Children = new List<StandardAndPolicyTreeDto>();
    }
}