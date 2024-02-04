using YaSha.DataManager.ProductInventory;
using YaSha.DataManager.ProductInventory.Dto;

namespace YaSha.DataManager.Controllers
{
    /// <summary>
    /// 架构清单树
    /// </summary>
    [Route("ProductInventoryTree")]
    public class ProductInventoryTreeController : DataManagerController, IProductIventTreeAppService
    {
        private readonly IProductIventTreeAppService _service;


        public ProductInventoryTreeController(IProductIventTreeAppService service)
        {
            _service = service;
        }

        [HttpPost("GetTree")]
        public async Task<List<ProductInventoryTreeDto>> GetRoot()
        {
            return await _service.GetRoot();
        }

        [HttpPost("UpdateRemark")]
        public async Task<ProductInventoryTreeDto> UpdateRemark(Guid id, string remark)
        {
            return await _service.UpdateRemark(id, remark);
        }
    }
}