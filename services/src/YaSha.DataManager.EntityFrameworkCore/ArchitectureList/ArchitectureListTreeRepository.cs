using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using YaSha.DataManager.ArchitectureList.AggregateRoot;
using YaSha.DataManager.ArchitectureList.Repository;
using YaSha.DataManager.EntityFrameworkCore;

namespace YaSha.DataManager.ArchitectureList;

public class ArchitectureListTreeRepository : EfCoreRepository<DataManagerDbContext, ArchitectureListTree, Guid>,
    IArchitectureListTreeRepository
{
    public ArchitectureListTreeRepository(IDbContextProvider<DataManagerDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task InitTree(List<ArchitectureListTree> roots)
    {
        if (await base.GetCountAsync() == 0)
        {
            foreach (var trees in roots.Select(root => GetRootAllTrees(root)))
            {
                await base.InsertManyAsync(trees, autoSave: true);
            }
        }
    }
    
    public async Task<List<ArchitectureListTree>> GetRootTree(bool include = true)
    {
        var roots = await (await GetDbSetAsync())
            .IncludeTreeDetails(include)
            .Where(x => x.ParentId == null)
            .OrderBy(x=>x.Code)
            .ToListAsync();
        
        foreach (var item in roots)
        {
            await GetNodeFullLevel(item);
        }
        return roots;
    }

    async Task GetNodeFullLevel(ArchitectureListTree node)
    {
        if (node.Children.Count == 0)
        {
            node.Children= await (await GetDbSetAsync())
                .IncludeTreeDetails()
                .Where(x => x.ParentId == node.Id)
                .OrderBy(x=>x.Code)
                .ToListAsync();
        }
        else
        {
            node.Children = node.Children.OrderBy(x => x.Code).ToList();
        }
        
        foreach (var item in node.Children)
        {
            await GetNodeFullLevel(item);
        }
    }

    public async Task<List<ArchitectureListTree>> GetChildren(Guid id)
    {
        List<ArchitectureListTree> childrens = new List<ArchitectureListTree>();
        var tree = await FindAsync(x => x.Id == id, true);
        childrens.Add(tree);
        foreach (var item in tree.Children)
        {
            childrens.AddRange(await GetChildren(item.Id));
        }

        return childrens;
    }

    public async Task<ArchitectureListTree> GetTreeByName(string name, bool include = false)
    {
        return await (await GetDbSetAsync())
            .IncludeTreeDetails(include)
            .Where(x => x.Name == name)
            .FirstOrDefaultAsync();
    }

    public async Task<string> GetTreePath(Guid id)
    {
        var result = string.Empty;
        var findId = id;
        var list = await (await GetDbSetAsync()).ToListAsync();
        ArchitectureListTree find = null;
        do
        {
            find = list.FirstOrDefault(x => x.Id == findId);

            if (find != null)
            {
                result = result.Insert(0, "/" + find.Name);
                if (find.ParentId != null) findId = find.ParentId.Value;
            }
            
        } while (find != null && find.ParentId != null);

        return result;
    }

    public override async Task<IQueryable<ArchitectureListTree>> WithDetailsAsync()
    {
        return (await GetQueryableAsync()).IncludeTreeDetails();
    }

    private static IEnumerable<ArchitectureListTree> GetRootAllTrees(ArchitectureListTree root)
    {
        var allTrees = new List<ArchitectureListTree>() { root };
        foreach (var child in root.Children)
        {
            allTrees.AddRange(GetRootAllTrees(child));
        }

        return allTrees;
    }
}