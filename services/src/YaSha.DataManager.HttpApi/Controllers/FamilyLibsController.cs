using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using YaSha.DataManager.FamilyLibs;
using YaSha.DataManager.FamilyTrees;
using YaSha.DataManager.ProductInventory.Dto;

namespace YaSha.DataManager.Controllers
{
    [Route("FamilyLibs")]
    public class FamilyLibsController : DataManagerController, IFamilyLibAppService
    {
        private readonly IFamilyLibAppService _service;

        public FamilyLibsController(IFamilyLibAppService service)
        {
            _service = service;
        }

        [HttpPost("Creat")]
        public async Task<FamilyLibDto> CreateAsync(FamilyLibCreateDto input)
        {
            return await _service.CreateAsync(input);
        }

        [HttpPost("Delete")]
        public async Task DeleteAsync(Guid id)
        {
            await _service.DeleteAsync(id);
        }

        [HttpPost("GetById")]
        public async Task<FamilyLibDto> GetAsync(Guid id)
        {
            return await _service.GetAsync(id);
        }

        [HttpPost("GetWithParamer")]
        public async Task<PagedResultDto<FamilyLibDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            return await _service.GetListAsync(input);
        }

        [HttpPost("Update")]
        public async Task<FamilyLibDto> UpdateAsync(Guid id, FamilyLibCreateDto input)
        {
            return await _service.UpdateAsync(id, input);
        }

        [HttpPost("GetListByTreeId")]
        public async Task<PagedResultDto<FamilyLibDto>> GetListByIdAsync(OrderNotificationSearchDto input)
        {
            return await _service.GetListByIdAsync(input);
        }

        [HttpPost("GetFamilyModule")]
        public async Task<List<FamilyLibDto>> GetFamilyModuleList(Guid guid)
        {
            return await _service.GetFamilyModuleList(guid);
        }

        [HttpPost("UploadFamilyLibImage")]
        public async Task<ImageResponseDto> UploadFamilyLibImage([FromForm] ImageFileDto dto)
        {
            return await _service.UploadFamilyLibImage(dto);
        }

        [HttpPost("Get")]
        public async Task<List<FamilyLibDto>> GetLibsAsync()
        {
            return await _service.GetLibsAsync();
        }


        [HttpPost("CreateList")]
        public async Task CreateListAsync(List<FamilyLibCreateDto> dtos)
        {
            await _service.CreateListAsync(dtos);
        }

        [HttpPost("DeleteList")]
        public async Task DeleteListAsync(List<FamilyLibDto> dtos)
        {
            await _service.DeleteListAsync(dtos);
        }
    }
}
