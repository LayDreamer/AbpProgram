using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using YaSha.DataManager.Common;

namespace YaSha.DataManager.FamilyLibs
{
    public interface IFamilyLibAppService :
        ICrudAppService<
            FamilyLibDto,
            Guid,
            PagedAndSortedResultRequestDto>
    {
        Task<PagedResultDto<FamilyLibDto>> GetListByIdAsync(OrderNotificationSearchDto input);

        Task<List<FamilyLibDto>> GetFamilyModuleList(Guid guid);
    }
}
