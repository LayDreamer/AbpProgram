using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using YaSha.DataManager.ProductInventory.Dto;

namespace YaSha.DataManager.FamilyLibs
{
    public interface IFamilyLibAppService :
        ICrudAppService<
            FamilyLibDto,
            Guid,
            PagedAndSortedResultRequestDto,
             FamilyLibCreateDto>
    {
        Task<PagedResultDto<FamilyLibDto>> GetListByIdAsync(OrderNotificationSearchDto input);

        Task<List<FamilyLibDto>> GetFamilyModuleList(Guid guid);

        Task<ImageResponseDto> UploadFamilyLibImage(ImageFileDto dto);

        Task<List<FamilyLibDto>> GetLibsAsync();


        Task DeleteListAsync(List<FamilyLibDto> dtos);

        Task CreateListAsync(List<FamilyLibCreateDto> dtos);

       
    }
}
