using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using YaSha.DataManager.FamilyLibs;
using YaSha.DataManager.FamilyTrees;

namespace YaSha.DataManager.Controllers
{
    [Route("FamilyTree")]
    public class FamilyTreeController : DataManagerController, IFamilyTreeAppService
    {

        private readonly IFamilyTreeAppService _service;

        public FamilyTreeController(IFamilyTreeAppService service)
        {
            _service = service;
        }

        [HttpPost("Create")]
        public async Task<FamilyTreeDto> CreateAsync(FamilyTreeCreateDto input)
        {
            return await _service.CreateAsync(input);
        }

        [HttpPost("Delete")]
        public async Task DeleteAsync(Guid id)
        {
            await _service.DeleteAsync(id);
        }

        [HttpPost("GetById")]
        public async Task<FamilyTreeDto> GetAsync(Guid id)
        {
            return await _service.GetAsync(id);
        }

        [HttpPost("GetWithParamer")]
        public async Task<PagedResultDto<FamilyTreeDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            return await _service.GetListAsync(input);
        }

        [HttpPost("Update")]
        public async Task<FamilyTreeDto> UpdateAsync(Guid id, FamilyTreeCreateDto input)
        {
            return await _service.UpdateAsync(id, input);
        }

        [HttpPost("Get")]
        public async Task<List<FamilyTreeDto>> GetTreeListAsync()
        {
            return await _service.GetTreeListAsync();
        }

        [HttpPost("GetChildrenById")]
        public async Task<List<Guid>> GetCategoryTreeChildrenGuids(Guid id)
        {
            return await _service.GetCategoryTreeChildrenGuids(id);
        }

      

        [HttpPost("CreateList")]
        public async Task<List< FamilyTreeDto>> CreateListAsync( List< FamilyTreeCreateDto> dtos)
        {
            return await _service.CreateListAsync(dtos);
        }

        [HttpPost("DeleteList")]
        public async Task DeleteListAsync(List<FamilyTreeDto> dtos)
        {
            await _service.DeleteListAsync(dtos);
        }




    }
}
