using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using YaSha.DataManager.Core;
using YaSha.DataManager.FamilyLibs;
using YaSha.DataManager.ProcessingLists;
using YaSha.DataManager.ProductInventory.Dto;
using YaSha.DataManager.ProductRetrieval;

namespace YaSha.DataManager.Controllers
{
    [Route("ProcessingList")]
    public class ProcessingListController : DataManagerController, IProcessingListAppService
    {

        private readonly IProcessingListAppService _service;

        public ProcessingListController(IProcessingListAppService service)
        {
            _service = service;
        }
        [HttpPost("SendEmail")]
        public async Task<string> SendEmail(Object json)
        {
            return await _service.SendEmail(json);
        }


        [HttpPost("UploadExcel")]
        public async Task<string> UploadExcel(IFormFile file)
        {
            return await _service.UploadExcel(file);
        }

        [HttpPost("GetAllList")]
        public async Task<PagedResultDto<ProcessingListDto>> GetAllListAsync(OrderNotificationSearchDto input)
        {
            return await _service.GetAllListAsync(input);
        }

        [HttpPost("DownloadExcel")]
        public async Task<byte[]> DownloadExcelAsync(Guid id)
        {
            return await _service.DownloadExcelAsync(id);
        }

        [HttpPost("Get")]
        public async Task<ProcessingListDto> GetAsync(Guid id)
        {
            return await _service.GetAsync(id);
        }

        [HttpPost("GetList")]
        public async Task<PagedResultDto<ProcessingListDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            return await _service.GetListAsync(input);
        }

        [HttpPost("Create")]
        public async Task<ProcessingListDto> CreateDataFromFile(ProcessingListCreateDto input)
        {
            return await _service.CreateDataFromFile(input);
        }

        [HttpPost("Delete")]
        public async Task DeleteFileAndData(  Guid id)
        {
            await _service.DeleteFileAndData(id);
        }
       
    }
}
