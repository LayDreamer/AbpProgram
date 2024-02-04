using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using YaSha.DataManager.MeasuringExcels;
using YaSha.DataManager.ProcessingLists;
using YaSha.DataManager.ProductInventory.Dto;
using YaSha.DataManager.SaleOderList;
using YaSha.DataManager.StandardAndPolicy.Dto;

namespace YaSha.DataManager.Controllers
{

    [Route("SaleOder")]

    public class SaleOderController : DataManagerController, ISaleOderListAppService
    {

        private readonly ISaleOderListAppService _service;

        public SaleOderController(ISaleOderListAppService service)
        {
            _service = service;
        }

        [HttpPost("UploadFactoryFile")]
        public async Task<ApiResultDto> UploadFactoryFile(IFormFile file)
        {
            return await _service.UploadFactoryFile(file);
        }

        [HttpPost("CreateNewSplitFile")]
        public string CreateNewSplitFile(SaleOderFormDto input)
        {
            return _service.CreateNewSplitFile(input);
        }


        [HttpPost("CreateSaleOderFile")]
        public async Task<ApiResultDto> CreateSaleOderFile(object json)
        {
            return await _service.CreateSaleOderFile(json);
        }

        [HttpPost("GetSaleOrderList")]
        public async Task<PagedResultDto<SaleOderDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            return await _service.GetListAsync(input);
        }

        [HttpPost("Get")]
        public async Task<SaleOderDto> GetAsync(Guid id)
        {
            return await _service.GetAsync(id);
        }

        [HttpPost("Delete")]
        public async Task DeleteAsync(Guid id)
        {
            await _service.DeleteAsync(id);
        }


    }
}
