using Microsoft.AspNetCore.Http;
using Volo.Abp.Application.Dtos;
using YaSha.DataManager.ProductRetrieval;
using YaSha.DataManager.ProductRetrieval.Dto;
using YaSha.DataManager.StandardAndPolicy.Dto;

namespace YaSha.DataManager.Controllers
{
    /// <summary>
    /// 项目信息
    /// </summary>
    [Route("ProjectInfo")]
    public class ProjectInfoController : DataManagerController, IProjectInfoAppService
    {
        private readonly IProjectInfoAppService _service;

        public ProjectInfoController(IProjectInfoAppService service)
        {
            _service = service;
        }

        [HttpPost("ImportFromExcel")]
        public async Task<ApiResultDto> ImportFromExcel(IFormFile file)
        {
            return await _service.ImportFromExcel(file);
        }

        [HttpPost("Parsing")]
        public async Task<List<ProjectInfoCodeResultDto>> Parsing(List<ProjectInfoSearchCodeDto> input)
        {
            return await _service.Parsing(input);
        }

        //[HttpPost("GetByErp")]
        //public async Task<List<ProjectInfoCodeResultDto>> GetByErp(List<ProjectInfoSearchCodeDto> input)
        //{
        //    var resultDto = await _service.GetByErp(input);
        //    return resultDto.Data as List<ProjectInfoCodeResultDto>;
        //}

        [HttpPost("GetByErp")]
        public async Task<ApiResultDto> GetByErp(List<ProjectInfoSearchCodeDto> input)
        {
            return await _service.GetByErp(input);
        }
    }
}
