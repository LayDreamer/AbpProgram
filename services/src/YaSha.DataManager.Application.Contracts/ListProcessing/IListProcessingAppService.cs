using Microsoft.AspNetCore.Http;
using Volo.Abp.Application.Dtos;
using YaSha.DataManager.StandardAndPolicy.Dto;

namespace YaSha.DataManager.ListProcessing;

public interface IListProcessingAppService: IApplicationService
{
    Task<PagedResultDto<ListProcessingDto>> Page(ListProcessingSearchDto input);

    Task<List<string>> GetSelectData(ListProcessingSelectEnum type);
    
    Task<ApiResultDto> UpLoadListProcessingRules(IFormFile file, ListProcessingSelectEnum type);

    Task<ApiResultDto> BuildSheets(ListProcessingBuildDto input);


    Task<ApiResultDto> DeleteSheets(List<Guid> ids);
}