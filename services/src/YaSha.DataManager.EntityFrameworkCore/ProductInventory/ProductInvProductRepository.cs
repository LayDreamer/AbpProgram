using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using YaSha.DataManager.Common;
using YaSha.DataManager.EntityFrameworkCore;
using YaSha.DataManager.ProductInventory.AggregateRoot;
using YaSha.DataManager.Repository.ProductInventory;

namespace YaSha.DataManager.ProductInventory;

public class ProductInvProductRepository : EfCoreRepository<DataManagerDbContext, ProductInventProduct, Guid>,
    IProductInvProductRepository
{
    public ProductInvProductRepository(IDbContextProvider<DataManagerDbContext> dbContextProvider) : base(
        dbContextProvider)
    {
    }

    public async Task<ProductInventProduct> InsertProduct(Guid parentId, ProductInventProduct product)
    {
        product.SetParentId(parentId);
        return await base.InsertAsync(product, autoSave: true);
    }

    public async Task<ProductInventProduct> FindByTreeIdAndNameAndCode(Guid treeId, string name, string code,
        bool include = false)
    {
        return await (await GetDbSetAsync())
            .IncludeProductDetails(include)
            .Where(x => x.ParentId == treeId && x.Name == name && x.Code == code)
            .FirstOrDefaultAsync();
    }

    public async Task<List<ProductInventProduct>> FindProductIndex(string system, string series, string name, string code, string sorting, bool include = false)
    {
        var query = (await GetDbSetAsync()).IncludeProductDetails(include);
        query = query.Where(x => 
            (string.IsNullOrEmpty(system) || x.System.Contains(system))&&
            (string.IsNullOrEmpty(series) || x.Series.Contains(series))&&
            (string.IsNullOrEmpty(name) || x.Name.Contains(name))&&
            (string.IsNullOrEmpty(code) || x.Code.Contains(code))
            );
        if (!string.IsNullOrEmpty(sorting))
        {
            query = query.OrderBy(sorting);
        }
        else
        {
            query = query.OrderByDescending<ProductInventProduct, DateTime>(
                (Expression<Func<ProductInventProduct, DateTime>>)(e => ((IHasCreationTime)e).CreationTime));
        }
        return await AsyncExecuter.ToListAsync(query);
    }

    public async Task<ProductInventProduct> FindById(Guid id, bool include = false)
    {
        return await (await GetDbSetAsync())
            .IncludeProductDetails(include)
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<List<ProductInventProduct>> FindByIds(List<Guid> ids, bool include = false)
    {
        return await (await GetDbSetAsync())
            .IncludeProductDetails(include)
            .Where(x => ids.Any(y=>y.Equals(x.Id)))
            .ToListAsync();
    }

    public async Task<IQueryable<ProductInventProduct>> FindProductsByTreeIdsAndNameCode(IEnumerable<Guid> ids,
        string inputName, string inputCode)
    {
        IQueryable<ProductInventProduct> query;
        if (string.IsNullOrEmpty(inputName) && string.IsNullOrEmpty(inputCode))
        {
            query = (await GetDbSetAsync())
                .IncludeProductDetails(false)
                .Where(x => ids.Any(y => y.Equals(x.ParentId)));
        }
        else
        {
            query = (await GetDbSetAsync())
                .IncludeProductDetails(true)
                .Where(x => ids.Any(y => y.Equals(x.ParentId)));
            if (!string.IsNullOrEmpty(inputName))
            {
                query = query.Where(x =>
                    x.Name.Contains(inputName)
                    || x.Materials.Any(y => y.Name.Contains(inputName)
                    || x.Modules.Any(y => y.Name.Contains(inputName) || y.Materials.Any(z => z.Name.Contains(inputName)))));
            }

            if (!string.IsNullOrEmpty(inputCode))
            {
                query = query.Where(x =>
                    x.Code.Contains(inputCode)
                    || x.Materials.Any(y => y.Code.Contains(inputCode))
                    || x.Modules.Any(y => y.Code.Contains(inputCode) || y.Materials.Any(z => z.Code.Contains(inputCode))));
            }
        }

        return query;
    }

    public IQueryable<ProductInventProduct> SortAndPageProducts(IQueryable<ProductInventProduct> query,
        string sorting, int skip, int maxResult)
    {
        if (!string.IsNullOrEmpty(sorting))
        {
            query = query.OrderBy(sorting);
        }
        else
        {
            query = query.OrderByDescending<ProductInventProduct, DateTime>(
                (Expression<Func<ProductInventProduct, DateTime>>)(e => ((IHasCreationTime)e).CreationTime));
        }

        query = query.Skip((skip - 1) * maxResult).Take(maxResult);

        return query;
    }

    public async Task<ProductInventProduct> DeleteProduct(Guid id)
    {
        var product = await FindById(id, true);
        return await DeleteProduct(product);
    }

    public async Task<ProductInventProduct> DeleteProduct(ProductInventProduct product)
    {
        await base.DeleteAsync(product, autoSave: true);
        return product;
    }

    public override async Task<IQueryable<ProductInventProduct>> WithDetailsAsync(
        params Expression<Func<ProductInventProduct, object>>[] propertySelectors)
    {
        return (await GetQueryableAsync()).IncludeProductDetails();
    }
}