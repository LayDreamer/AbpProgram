using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using YaSha.DataManager.NewFamilyLibrary;
using YaSha.DataManager.ProductInventory.Dto;

namespace YaSha.DataManager.Controllers
{
    [Route("NewFamilyLib")]
    public class NewFamilyLibController : DataManagerController, INewFamilyLibAppService
    {
        private readonly INewFamilyLibAppService _service;

        public NewFamilyLibController(INewFamilyLibAppService service)
        {
            _service = service;
        }

        [HttpPost("UploadFamilyLibImage")]
        public async Task<ImageResponseDto> UploadFamilyLibImage([FromForm] ImageFileDto dto)
        {
            return await _service.UploadFamilyLibImage(dto);
        }

        [HttpPost("GetListByTreeId")]
        public async Task<PagedResultDto<NewFamilyLibDto>> GetListByTreeIdAsync(NewFamilyLibSearchDto searchDto)
        {
            return await _service.GetListByTreeIdAsync(searchDto);
        }

        [HttpPost("GetListPage")]
        public async Task<PagedResultDto<NewFamilyLibDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            return await _service.GetListAsync(input);
        }


        [HttpPost("GetFamilyInfo")]
        public async Task<List<NewFamilyLibDto>> GetFamilyInfo(Guid guid)
        {
            return await _service.GetFamilyInfo(guid);
        }

        [HttpPost("Create")]
        public async Task<NewFamilyLibDto> CreateAsync(NewFamilyLibCreateDto input)
        {
            return await _service.CreateAsync(input);
        }

        [HttpPost("CreateList")]
        public async Task<List<NewFamilyLibDto>> CreateListAsync(List<NewFamilyLibCreateDto> dtos)
        {
            return await _service.CreateListAsync(dtos);
        }

        [HttpPost("Delete")]
        public async Task DeleteAsync(Guid id)
        {
            await _service.DeleteAsync(id);
        }


        [HttpPost("DeleteList")]
        public async Task DeleteListAsync(List<NewFamilyLibDto> dtos)
        {
            await _service.DeleteListAsync(dtos);
        }

        [HttpPost("Update")]
        public async Task<NewFamilyLibDto> UpdateAsync(Guid id, NewFamilyLibCreateDto input)
        {
            return await _service.UpdateAsync(id, input);
        }

        [HttpPost("Get")]
        public async Task<NewFamilyLibDto> GetAsync(Guid id)
        {
            return await _service.GetAsync(id);
        }


        [HttpPost("GetList")]
        public async Task<List<NewFamilyLibDto>> GetListAsync()
        {
            return await _service.GetListAsync();
        }

        [HttpPost("ArchSearch")]
        public async Task<NewFamilyLibDto> ArchSearch(string name, string type, string processMode)
        {
            return await _service.ArchSearch(name, type, processMode);
        }
    }
}