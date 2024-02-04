using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using YaSha.DataManager.EntityFrameworkCore;
using YaSha.DataManager.ProductInventory.AggregateRoot;
using YaSha.DataManager.StandardAndPolicy.AggregateRoot;
using YaSha.DataManager.StandardAndPolicy.Repository;

namespace YaSha.DataManager.StandardAndPolicy;

public class StandardAndPolicyTreeRepository: EfCoreRepository<DataManagerDbContext, StandardAndPolicyTree, Guid>,
    IStandardAndPolicyTreeRepository
{
    public StandardAndPolicyTreeRepository(IDbContextProvider<DataManagerDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }


    public async Task InitStandardAndPolicyTree(List<StandardAndPolicyTree> roots)
    {
        if (await base.GetCountAsync() == 0)
        {
            foreach (var trees in roots.Select(root => GetRootAllTrees(root)))
            {
                await base.InsertManyAsync(trees, autoSave: true);
            }
        }
    }
    
    void BuildTree(StandardAndPolicyTree node,List<StandardAndPolicyTree> nodes)
    {
        var children = nodes.Where(x=>x.ParentId == node.Id).ToList();
        node.Children = children;
        nodes.RemoveAll(x => children.Contains(x));
        foreach (var child in children)
        {
            BuildTree(child, nodes);
        }
    }
    
    List<StandardAndPolicyTree> SeriesTrees(List<StandardAndPolicyTree> trees)
    {
        var parents = trees.Where(x => x.ParentId == null).ToList();
        trees.RemoveAll(x => parents.Contains(x));
        foreach (var tree in parents)
        {
            BuildTree(tree, trees);
        }
        return parents;
    }
    
    public async Task DeleteTree(Guid id)
    {
        var ids = await FindAllChildren(id);
        await base.DeleteManyAsync(ids, autoSave: true);
    }


    public async Task ExchangeTowNode(Guid id1, Guid id2)
    {
        var ent1 = await FindAsync(id1);
        var allChilds = await (await GetDbSetAsync())
            .Where(x => x.ParentId.Equals(ent1.ParentId))
            .OrderBy(x=>x.Code).ToListAsync();
        var index = allChilds.FirstOrDefault(x => x.Id.Equals(id1));
        var find = allChilds.FirstOrDefault(x => x.Id.Equals(id2));
        allChilds.Remove(index);
        if (find == null)
        {
            allChilds.Insert(0, index);
        }
        else
        {
            var t = allChilds.IndexOf(find);
            allChilds.Insert(t + 1,index);
        }
        for (int i = 0; i < allChilds.Count(); i++)
        {
            allChilds[i].Code = i;
        }

        await base.UpdateManyAsync(allChilds, true);
    }
    
    IEnumerable<StandardAndPolicyTree> GetRootAllTrees(StandardAndPolicyTree root)
    {
        var allTrees = new List<StandardAndPolicyTree>() { root };
        foreach (var child in root.Children)
        {
            allTrees.AddRange(GetRootAllTrees(child));
        }

        return allTrees;
    }

    public async Task<int> GetSameParentTotalCount(Guid id)
    {
        var find = await (await GetDbSetAsync())
            .IncludeStandardAndPolicyTreeDetails()
            .FirstOrDefaultAsync(x => x.Id.Equals(id));

        if (find == null)
        {
            return 999;
        }
        return find.Children.Max(x=>x.Code);
    }
    
    public async Task<List<StandardAndPolicyTree>> GetRootTree()
    {
        var roots= await (await GetDbSetAsync())
            .IncludeStandardAndPolicyTreeDetails()
            .Where(x => x.ParentId == null)
            .OrderBy(x=>x.Code)
            .ToListAsync();

        foreach (var root in roots)
        {
            root.Children = root.Children.OrderBy(x => x.Code).ToList();
        }
        return roots;
    }

    public async Task<string> GetParentName(Guid id)
    {
        var tree = await base.FindAsync(id);
        return tree.ParentId == null ? "" : (await base.FindAsync(tree.ParentId.Value)).Name;
    }

    public async Task<List<Guid>> GetRoot()
    {
        return await (await GetDbSetAsync()).Where(x => x.ParentId == null).Select(x=>x.Id).ToListAsync();
    }

    async Task<List<Guid>> FindChildrenById(Guid id)
    {
        return await (await GetDbSetAsync())
            .IncludeStandardAndPolicyTreeDetails(false)
            .Where(x => x.ParentId == id)
            .Select(x => x.Id).ToListAsync();
    }

    public async Task<List<Guid>> FindAllChildren(Guid id)
    {
        List<Guid> ids = new List<Guid>() { id };
        var children = await FindChildrenById(id);
        if (children.Count > 0)
        {
            foreach (var child in children)
            {
                ids.AddRange(await FindAllChildren(child));
            }
        }
        return ids;
    }

    public async Task<List<StandardAndPolicyTree>> HideTree(List<Guid> id, bool hide)
    {
        var trees = await (await GetDbSetAsync())
            .IncludeStandardAndPolicyTreeDetails(false)
            .Where(x => id.Any(y => y.Equals(x.Id)))
            .ToListAsync();

        trees.ForEach(x => x.Hide = hide);
        await UpdateManyAsync(trees, true);
        return trees;
    }

    public override async Task<IQueryable<StandardAndPolicyTree>> WithDetailsAsync()
    {
        return (await GetQueryableAsync()).IncludeStandardAndPolicyTreeDetails();
    }
}