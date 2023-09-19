using System.Linq.Expressions;
using YaSha.DataManager.ProductInventory.AggregateRoot;

namespace YaSha.DataManager.ProductInventory;

public static class ProductInventoryQueryableExtensions
{
    public static IQueryable<ProductInventTree> IncludeTreeDetails(this IQueryable<ProductInventTree> queryable,
        bool include = true)
    {
        return !include ? queryable : queryable.Include(x => x.Children).
            ThenInclude(x => x.Children).ThenInclude(x=>x.Children);
    }

    public static IQueryable<ProductInventProduct> IncludeProductDetails(
        this IQueryable<ProductInventProduct> queryable,
        bool include = true)
    {
        return !include
            ? queryable
            : queryable.Include(x => x.Modules).ThenInclude(x => x.Materials)
                .Include(x => x.Materials);
    }
    
    public static IQueryable<ProductInventModule> IncludeModuleDetails(
        this IQueryable<ProductInventModule> queryable,
        bool include = true)
    {
        return !include
            ? queryable
            : queryable.Include(x => x.Materials);
    }
}