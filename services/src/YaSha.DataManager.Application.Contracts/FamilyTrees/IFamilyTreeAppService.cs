using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace YaSha.DataManager.FamilyTrees
{
    public interface IFamilyTreeAppService :
        ICrudAppService<
            FamilyTreeDto,
            Guid,
            PagedAndSortedResultRequestDto>
    {
        Task<PagedResultDto<FamilyTreeDto>> GetTreeListAsync();

    }
}
