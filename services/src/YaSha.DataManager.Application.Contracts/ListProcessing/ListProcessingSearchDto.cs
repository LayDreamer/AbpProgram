

using Volo.Abp.Application.Dtos;

namespace YaSha.DataManager.ListProcessing;

public class ListProcessingSearchDto : PagedAndSortedResultRequestDto
{
    public ListProcessingSearchDto(string search)
    {
        Search = search;
    }

    public string Search { get; }
}