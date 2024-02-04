using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using YaSha.DataManager.FamilyLibs;
using YaSha.DataManager.ProductInventory.Dto;

namespace YaSha.DataManager.NewFamilyLibrary
{
    public interface INewFamilyLibAppService :
        ICrudAppService<
            NewFamilyLibDto,
            Guid,
            PagedAndSortedResultRequestDto,
             NewFamilyLibCreateDto>
    {


        Task<PagedResultDto<NewFamilyLibDto>> GetListByTreeIdAsync(NewFamilyLibSearchDto searchDto);

        Task<List<NewFamilyLibDto>> GetFamilyInfo(Guid guid);

        Task<ImageResponseDto> UploadFamilyLibImage(ImageFileDto dto);

        Task<List<NewFamilyLibDto>> CreateListAsync(List<NewFamilyLibCreateDto> dtos);

        Task DeleteListAsync(List<NewFamilyLibDto> dtos);

        Task<List<NewFamilyLibDto>> GetListAsync();

        Task<NewFamilyLibDto> ArchSearch(string name, string type, string processMode);
    }
}
