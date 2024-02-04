using Microsoft.AspNetCore.Http;
using Volo.Abp.Application.Dtos;
using YaSha.DataManager.StandardAndPolicy;
using YaSha.DataManager.StandardAndPolicy.Dto;

namespace YaSha.DataManager.Controllers
{
    /// <summary>
    /// 标准和政策数据库
    /// </summary>
    [Route("StandardAndPolicyLib")]
    public class StandardAndPolicyThemeController : DataManagerController, IStandardAndPolicyThemeAppService
    {
        private readonly IStandardAndPolicyThemeAppService _service;

        public StandardAndPolicyThemeController(IStandardAndPolicyThemeAppService service)
        {
            _service = service;
        }

        [HttpPost("UploadFile")]
        public async Task<ApiResultDto> UploadFile(IFormFile file)
        {
            return await _service.UploadFile(file);
        }

        [HttpPost("Insert")]
        public async Task<List<StandardAndPolicyThemeDto>> InsertStandardAndPolicyTheme(StandardAndPolicyCreateAndUpdateDto input)
        {
            return await _service.InsertStandardAndPolicyTheme(input);
        }

        [HttpPost("PageHome")]
        public async Task<List<StandardAndPolicyPageDto>> PageHome(int showCount = 4)
        {
            return await _service.PageHome(showCount);
        }

        [HttpPost("PageByTree")]
        public async Task<List<StandardAndPolicyPageDto>> PageSelectTree(Guid id, int showCount)
        {
            return await _service.PageSelectTree(id, showCount);
        }

        [HttpPost("PageBySearch")]
        public async Task<PagedResultDto<StandardAndStandLibWithAllTreeDto>> PageSearchDetail(StandardAndPolicySearchDto input)
        {
            return await _service.PageSearchDetail(input);
        }

        [HttpPost("GetCardDetail")]
        public async Task<StandardAndPolicyCardDetailDto> GetCardDetail(Guid id)
        {
            return await _service.GetCardDetail(id);
        }

        [HttpPost("UpdateCollect")]
        public async Task<StandardAndPolicyCollectDto> UpdateCollectStatus(Guid id, bool status)
        {
            return await _service.UpdateCollectStatus(id, status);
        }

        [HttpPost("UpdateLib")]
        public async Task<StandardAndPolicyLibDto> UpdateLib(Guid id, StandardAndPolicyCreateAndUpdateDto input)
        {
            return await _service.UpdateLib(id, input);
        }

        [HttpPost("Delete")]
        public async Task<ApiResultDto> DeleteLibs(List<Guid> ids)
        {
            return await _service.DeleteLibs(ids);
        }

        [HttpPost("Export")]
        public async Task<byte[]> ExportExcel()
        {
            return await _service.ExportExcel();
        }
    }
}