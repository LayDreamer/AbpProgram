namespace YaSha.DataManager.ProductRetrieval.Dto;

public class ProjectInfoCodeResultDto
{
    public string Code { get; set; }

    public List<ProjectInfoDto> Projects { get; set; } = new();
}