using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using YaSha.DataManager.EntityFrameworkCore;
using YaSha.DataManager.ProductInventory.AggregateRoot;
using YaSha.DataManager.ProductInventory.Repository;

namespace YaSha.DataManager.ProductInventory;

public class ProductInvModuleRepository : EfCoreRepository<DataManagerDbContext, ProductInventModule, Guid>,
    IProductInvModuleRepository
{
    public ProductInvModuleRepository(IDbContextProvider<DataManagerDbContext> dbContextProvider) : base(
        dbContextProvider)
    {
    }

    public async Task DeleteModule(List<ProductInventModule> modules)
    {
        if (modules.Count == 0) return;
        await base.DeleteManyAsync(modules, autoSave: true);
    }

    public async Task DeleteModule(List<Guid> ids)
    {
        if (ids.Count == 0) return;
        await base.DeleteManyAsync(ids, autoSave: true);
    }

    public async Task<List<ProductInventModule>> FindByCode(string code, bool include = false)
    {
        return await (await GetDbSetAsync())
            .IncludeModuleDetails(include)
            .Where(x => x.Code == code)
            .ToListAsync();
    }

    public async Task<List<ProductInventModule>> FindModuleIndex(string system, string series, string name, string code,
        string sorting, bool include = false)
    {
        var query = (await GetDbSetAsync()).IncludeModuleDetails(include);
        query = query.Where(x =>
            (string.IsNullOrEmpty(system) || x.System.Contains(system)) &&
            (string.IsNullOrEmpty(series) || x.Series.Contains(series)) &&
            (string.IsNullOrEmpty(name) || x.Name.Contains(name)) &&
            (string.IsNullOrEmpty(code) || x.Code.Contains(code))
        );
        if (!string.IsNullOrEmpty(sorting))
        {
            query = query.OrderBy(sorting);
        }
        else
        {
            query = query.OrderByDescending<ProductInventModule, DateTime>(
                (Expression<Func<ProductInventModule, DateTime>>)(e => ((IHasCreationTime)e).CreationTime));
        }

        return await AsyncExecuter.ToListAsync(query);
    }
}