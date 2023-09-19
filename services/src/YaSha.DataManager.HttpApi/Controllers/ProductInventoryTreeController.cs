using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using YaSha.DataManager.FamilyLibs;
using YaSha.DataManager.ProductInventory;
using YaSha.DataManager.ProductInventory.Dto;

namespace YaSha.DataManager.Controllers
{
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