using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using YaSha.DataManager.EntityFrameworkCore;
using YaSha.DataManager.NewFamilyLibrary.Repository;
using YaSha.DataManager.ProductInventory.AggregateRoot;
using System.Linq.Dynamic.Core;

namespace YaSha.DataManager.NewFamilyLibrary
{
    public class NewFamilyLibRepository : EfCoreRepository<DataManagerDbContext, NewFamilyLib, Guid>,
    INewFamilyLibRepository
    {
        public NewFamilyLibRepository(IDbContextProvider<DataManagerDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }
        public async Task<List<NewFamilyLib>> GetListByTreeIdAsync(List<Guid> categoryIds, string searchValue, string searchCode,
            string sorting, bool include = false)
        {
            var query = (await GetDbSetAsync()).IncludeLibDetails(include);
            query = query.Where(x =>
                x.ParentId == null &&
                categoryIds.Any(y => y.Equals(x.CategoryId)) &&
                (string.IsNullOrEmpty(searchValue) || (x.DisplayName.Trim().Contains(searchValue.Trim()) || x.Children.Any(y => y.DisplayName.Trim().Contains(searchValue.Trim())))) &&
                (string.IsNullOrEmpty(searchCode) || (x.Type.Trim().Equals(searchCode.Trim()) || x.Children.Any(y => y.Number.Trim().Equals(searchCode.Trim()))))
            );

            if (!string.IsNullOrEmpty(sorting))
            {
                query = query.OrderBy(sorting);
            }
            else
            {
                query = query.OrderByDescending<NewFamilyLib, DateTime>(
                    (Expression<Func<NewFamilyLib, DateTime>>)(e => ((IHasCreationTime)e).CreationTime));
            }
            return await AsyncExecuter.ToListAsync(query);
        }

        public async Task<List<NewFamilyLib>> GetById(Guid id, bool include = true)
        {
            return await (await GetDbSetAsync())
                .IncludeLibDetails(include)
               .Where(x => x.Id == id)
                 .ToListAsync();
        }
    }

}
