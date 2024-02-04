namespace YaSha.DataManager.ArchitectureList.Dto;

public class ArchitectureUpdateStatus
{
    public List<Guid> Id { get; set; }
    
    public ArchitectureListPublishStatus Status { get; set; }

    public ArchitectureUpdateStatus()
    {
        Id = new List<Guid>();
    }
}