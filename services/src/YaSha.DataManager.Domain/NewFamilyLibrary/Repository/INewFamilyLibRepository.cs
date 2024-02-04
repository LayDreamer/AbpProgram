using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace YaSha.DataManager.NewFamilyLibrary.Repository
{
    public interface INewFamilyLibRepository : IBasicRepository<NewFamilyLib, Guid>
    {

        Task<List<NewFamilyLib>> GetListByTreeIdAsync(List<Guid> categoryIds, string SearchValue, string SearchCode, string Sorting, bool include = false);


        Task<List<NewFamilyLib>> GetById(Guid id, bool include = true);

    }
}
