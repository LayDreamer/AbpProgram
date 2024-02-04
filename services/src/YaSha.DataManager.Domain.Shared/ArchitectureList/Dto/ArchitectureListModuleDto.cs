namespace YaSha.DataManager.ArchitectureList.Dto;

public class ArchitectureListModuleDto : ArchitectureListModuleCreateDto
{
    public Guid Id { get; set; }
    
    public Guid ParentId { get; set; }
    
    public string Path { get; set; }

    public string CreateUser { get; set; }

    public string ModifyUser { get; set; }

    public DateTime CreationTime { get; set; }
    
    public DateTime? LastModificationTime { get; set; }
    
    public ArchitectureListPublishStatus Status { get; set; }
    public List<ArchitectureListFileDto> Files { get; set; }
    public ArchitectureListModifyStatus ModifyStatus { get; set; } =  ArchitectureListModifyStatus.Normal;
    public new List<ArchitectureListMaterialDto> Materials { get; set; }
}