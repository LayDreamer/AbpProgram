using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using YaSha.DataManager.EntityFrameworkCore;
using YaSha.DataManager.ProductInventory.AggregateRoot;
using YaSha.DataManager.ProductInventory.Repository;

namespace YaSha.DataManager.ProductInventory;

public class ProductInvMaterialRepository : EfCoreRepository<DataManagerDbContext, ProductInventMaterial, Guid>,
    IProductInvMaterialRepository
{
    public ProductInvMaterialRepository(IDbContextProvider<DataManagerDbContext> dbContextProvider) : base(
        dbContextProvider)
    {
    }

    public async Task DeleteMaterial(List<ProductInventMaterial> materials)
    {
        if (materials.Count == 0) return;
        await base.DeleteManyAsync(materials, autoSave: true);
    }

    public async Task DeleteMaterial(List<Guid> ids)
    {
        if (ids.Count == 0) return;
        await base.DeleteManyAsync(ids, autoSave: true);
    }

    public async Task<List<ProductInventMaterial>> FindByCode(string code)
    {
        return await (await GetDbSetAsync())
            .Where(x => x.Code == code)
            .ToListAsync();
    }

    public async Task<List<ProductInventMaterial>> FindMaterialIndex(string system, string series, string name,
        string code, string sorting)
    {
        var query = (await GetDbSetAsync()).AsQueryable();
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
            query = query.OrderByDescending<ProductInventMaterial, DateTime>(
                (Expression<Func<ProductInventMaterial, DateTime>>)(e => ((IHasCreationTime)e).CreationTime));
        }

        return await AsyncExecuter.ToListAsync(query);
    }
}