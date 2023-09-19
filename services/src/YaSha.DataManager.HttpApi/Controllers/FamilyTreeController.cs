using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;
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

        [HttpPost("Creat")]
        public async Task<FamilyTreeDto> CreateAsync(FamilyTreeDto input)
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
        public async Task<FamilyTreeDto> UpdateAsync(Guid id, FamilyTreeDto input)
        {
            return await _service.UpdateAsync(id, input);
        }

        [HttpPost("Get")]
        public async Task<PagedResultDto<FamilyTreeDto>> GetTreeListAsync()
        {
            return await _service.GetTreeListAsync();
        }


    }
}
