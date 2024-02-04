using Volo.Abp.Application.Dtos;
using YaSha.DataManager.ProductRetrieval;
using YaSha.DataManager.ProductRetrieval.Dto;
using YaSha.DataManager.StandardAndPolicy.Dto;

namespace YaSha.DataManager.Controllers
{
    /// <summary>
    /// 物料库存
    /// </summary>
    [Route("MaterialInventory")]
    public class MaterialInventoryController : DataManagerController, IMaterialInventoryAppService
    {
        private readonly IMaterialInventoryAppService _service;

        public MaterialInventoryController(IMaterialInventoryAppService service)
        {
            _service = service;
        }

        [HttpPost("Upload")]
        public async Task<ApiResultDto> UploadDataAsync(List<MaterialInventoryCreateDto> input)
        {
            return await _service.UploadDataAsync(input);
        }

        [HttpPost("Parsing")]
        public async Task<List<MaterialInventoryDto>> ParsingListDataAsync(List<string> code)
        {
            return await _service.ParsingListDataAsync(code);
        }

        [HttpPost("GetByErp")]
        public async Task<ApiResultDto> GetByErpAsync(List<string> code)
        {
            return await _service.GetByErpAsync(code);
        }
    }
}
