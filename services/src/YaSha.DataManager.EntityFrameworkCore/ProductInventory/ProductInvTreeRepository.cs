using System.Linq.Expressions;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using YaSha.DataManager.EntityFrameworkCore;
using YaSha.DataManager.ProductInventory.AggregateRoot;
using YaSha.DataManager.ProductInventory.Repository;

namespace YaSha.DataManager.ProductInventory;

public class ProductInvTreeRepository : EfCoreRepository<DataManagerDbContext, ProductInventTree, Guid>,
    IProductInvTreeRepository
{
    public ProductInvTreeRepository(IDbContextProvider<DataManagerDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task InitProductInvTree(List<ProductInventTree> roots)
    {
        if (await base.GetCountAsync() == 0)
        {
            foreach (var trees in roots.Select(root => GetRootAllTrees(root)))
            {
                await base.InsertManyAsync(trees, autoSave: true);
            }
        }
    }

    public async Task<List<ProductInventTree>> GetRootTree(bool include = true)
    {
        return await (await GetDbSetAsync())
            .IncludeTreeDetails(include)
            .Where(x => x.ParentId == null)
            .ToListAsync();
    }

    public async Task<List<ProductInventTree>> GetChildren(Guid id)
    {
        List<ProductInventTree> childrens = new List<ProductInventTree>();
        var tree = await FindAsync(x => x.Id == id, true);
        childrens.Add(tree);
        foreach (var item in tree.Children)
        {
            childrens.AddRange(await GetChildren(item.Id));
        }

        return childrens;
    }

    public async Task<ProductInventTree> GetTreeByName(string name, bool include = false)
    {
        return await (await GetDbSetAsync())
            .IncludeTreeDetails(include)
            .Where(x => x.Name == name)
            .FirstOrDefaultAsync();
    }

    public override async Task<IQueryable<ProductInventTree>> WithDetailsAsync()
    {
        return (await GetQueryableAsync()).IncludeTreeDetails();
    }

    private static IEnumerable<ProductInventTree> GetRootAllTrees(ProductInventTree root)
    {
        var allTrees = new List<ProductInventTree>() { root };
        foreach (var child in root.Children)
        {
            allTrees.AddRange(GetRootAllTrees(child));
        }

        return allTrees;
    }
}