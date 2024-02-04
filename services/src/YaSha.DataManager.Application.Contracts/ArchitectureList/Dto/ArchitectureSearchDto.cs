using Volo.Abp.Application.Dtos;

namespace YaSha.DataManager.ArchitectureList.Dto;

public class ArchitectureSearchDto : PagedAndSortedResultRequestDto
{
    public string Key { get; set; }

    public string SearchValue { get; set; }

    public string SearchCode { get; set; }

    public ArchitectureListPublishStatus Status { get; set; }
}