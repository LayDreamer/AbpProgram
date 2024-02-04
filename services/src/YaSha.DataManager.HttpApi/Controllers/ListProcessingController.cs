using Microsoft.AspNetCore.Http;
using Volo.Abp.Application.Dtos;
using YaSha.DataManager.ListProcessing;
using YaSha.DataManager.StandardAndPolicy.Dto;

namespace YaSha.DataManager.Controllers;

/// <summary>
/// 制造拆单
/// </summary>
[Route("ListProcessing")]
public class ListProcessingController: DataManagerController, IListProcessingAppService
{
    private readonly IListProcessingAppService _service;

    public ListProcessingController(IListProcessingAppService service)
    {
        _service = service;
    }
    
    [HttpPost("Page")]
    public async Task<PagedResultDto<ListProcessingDto>> Page(ListProcessingSearchDto input)
    {
        return await _service.Page(input);
    }

    [HttpPost("GetSelect")]
    public async Task<List<string>> GetSelectData(ListProcessingSelectEnum type)
    {
        return await _service.GetSelectData(type);
    }
    
    [HttpPost("Upload")]
    public async Task<ApiResultDto> UpLoadListProcessingRules(IFormFile file, ListProcessingSelectEnum type)
    {
        return await _service.UpLoadListProcessingRules(file, type);
    }

    [HttpPost("Build")]
    public async Task<ApiResultDto> BuildSheets(ListProcessingBuildDto input)
    {
        return await _service.BuildSheets(input);
    }

    [HttpPost("Delete")]
    public async Task<ApiResultDto> DeleteSheets(List<Guid> ids)
    {
        return await _service.DeleteSheets(ids);
    }
}