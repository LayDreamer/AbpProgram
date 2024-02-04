using Microsoft.AspNetCore.Http;

namespace YaSha.DataManager.ArchitectureList.Dto;

public class ArchitectureListInsertDto
{
    public ArchitectureListFileStatus Status { get; set; }

    public List<IFormFile> Files { get; set; }

}