using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using YaSha.DataManager.MeasuringExcels;
using YaSha.DataManager.ProcessingLists;
using YaSha.DataManager.StandardAndPolicy.Dto;

namespace YaSha.DataManager.Controllers
{
    [Route("MeasuringExcel")]
    public  class MeasuringExcelController : DataManagerController, IMeasuringExcelAppService
    {

        private readonly IMeasuringExcelAppService _service;

        public MeasuringExcelController(IMeasuringExcelAppService service)
        {
            _service = service;
        }

        [HttpPost("Create")]
        public Task<MeasuringExcelDto> CreateAsync(MeasuringExcelDto input)
        {
            throw new NotImplementedException();
        }

        [HttpPost("Delete")]
        public async Task DeleteAsync(Guid id)
        {
             await _service.DeleteAsync(id);
        }

        [HttpPost("Get")]
        public async Task<MeasuringExcelDto> GetAsync(Guid id)
        {
            return await _service.GetAsync(id);
        }

        [HttpPost("GetList")]
        public async Task<PagedResultDto<MeasuringExcelDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            return await _service.GetListAsync(input);
        }

        [HttpPost("Update")]
        public Task<MeasuringExcelDto> UpdateAsync(Guid id, MeasuringExcelDto input)
        {
            throw new NotImplementedException();
        }

        [HttpPost("UploadFile")]
        public async Task<ApiResultDto> UploadFile(IFormFile file)
        {
            return await _service.UploadFile(file);
        }
        [HttpPost("DownloadFile")]
        public async Task<byte[]> DownloadExcelAsync(Guid id)
        {
            return await _service.DownloadExcelAsync(id);
        }



        [HttpPost("Add")]
        public async Task<ApiResultDto> AddAsync(Object input)
        {
            return await _service.AddAsync(input);
        }

        [HttpPost("CreateExcel")]
        public Task<MeasuringExcelDto> CreateAsync(MeasuringExcelCreateDto input)
        {
            throw new NotImplementedException();
        }

        [HttpPost("UpdateExcel")]
        public Task<MeasuringExcelDto> UpdateAsync(Guid id, MeasuringExcelCreateDto input)
        {
            throw new NotImplementedException();
        }
    }
}
