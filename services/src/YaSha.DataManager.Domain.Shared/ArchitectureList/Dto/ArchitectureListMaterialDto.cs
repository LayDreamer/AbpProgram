namespace YaSha.DataManager.ArchitectureList.Dto;

public class ArchitectureListMaterialDto : ArchitectureListMaterialCreateDto
{
    public Guid Id { get; set; }

    public ArchitectureListModifyStatus ModifyStatus { get; set; } = ArchitectureListModifyStatus.Normal;
}