using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using YaSha.DataManager.ArchitectureList;
using YaSha.DataManager.ArchitectureList.Dto;
using YaSha.DataManager.FamilyLibs;
using YaSha.DataManager.FamilyTrees;
using YaSha.DataManager.NewFamilyLibrary;
using YaSha.DataManager.ProductInventory.Dto;
using YaSha.DataManager.StandardAndPolicy.Dto;

namespace YaSha.DataManager.Controllers
{
    [Route("NewFamilyTree")]
    public class NewFamilyTreeController : DataManagerController, INewFamilyTreeAppService
    {

        private readonly INewFamilyTreeAppService _service;

        public NewFamilyTreeController(INewFamilyTreeAppService service)
        {
            _service = service;
        }


        [HttpPost("GetTree")]
        public async Task<List<NewFamilyTreeDto>> GetTreeRoot()
        {
            return await _service.GetTreeRoot();
        }
        [HttpPost("GetTreePage")]
        public async Task<PagedResultDto<NewFamilyTreeDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            return await _service.GetListAsync(input);
        }




        [HttpPost("Create")]
        public async Task<NewFamilyTreeDto> CreateAsync(NewFamilyTreeCreateDto input)
        {
            return await _service.CreateAsync(input);
        }


        [HttpPost("CreateList")]
        public async Task<List<NewFamilyTreeDto>> CreateListAsync(List<NewFamilyTreeCreateDto> dtos)
        {
            return await _service.CreateListAsync(dtos);
        }

        [HttpPost("Delete")]
        public async Task DeleteAsync(Guid id)
        {
            await _service.DeleteAsync(id);
        }

        [HttpPost("DeleteList")]
        public async Task DeleteListAsync(List<NewFamilyTreeDto> dtos)
        {
            await _service.DeleteListAsync(dtos);
        }

        [HttpPost("Update")]
        public async Task<NewFamilyTreeDto> UpdateAsync(Guid id, NewFamilyTreeCreateDto input)
        {
            return await _service.UpdateAsync(id, input);
        }

        [HttpPost("Get")]
        public async Task<NewFamilyTreeDto> GetAsync(Guid id)
        {
            return await _service.GetAsync(id);
        }



        [HttpPost("GetTreeList")]
        public async Task<List<NewFamilyTreeDto>> GetListAsync()
        {
            return await _service.GetListAsync();
        }

        [HttpPost("UploadFile")]
        public async Task<ApiResultDto> UploadFile(IFormFile file)
        {
            return await _service.UploadFile(file);

        }
    }
}
