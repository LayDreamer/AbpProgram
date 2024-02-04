using Autofac.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using YaSha.DataManager.FamilyTrees;
using YaSha.DataManager.ProductInventory.Dto;
using YaSha.DataManager.StandardAndPolicy.Dto;

namespace YaSha.DataManager.NewFamilyLibrary
{
    public interface INewFamilyTreeAppService :
        ICrudAppService<
            NewFamilyTreeDto,
            Guid,
            PagedAndSortedResultRequestDto,
            NewFamilyTreeCreateDto
           >
    {

        Task<List<NewFamilyTreeDto>> GetTreeRoot();


        Task<List<NewFamilyTreeDto>> GetListAsync();


        Task<List<NewFamilyTreeDto>> CreateListAsync(List<NewFamilyTreeCreateDto> dtos);

        Task DeleteListAsync(List<NewFamilyTreeDto> dtos);

        Task<ApiResultDto> UploadFile(IFormFile file);

    }
}
