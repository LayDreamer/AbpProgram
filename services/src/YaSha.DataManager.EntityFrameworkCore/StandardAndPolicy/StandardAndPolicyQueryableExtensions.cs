using YaSha.DataManager.StandardAndPolicy.AggregateRoot;

namespace YaSha.DataManager.StandardAndPolicy;

public static class StandardAndPolicyQueryableExtensions
{
    public static IQueryable<StandardAndPolicyTree> IncludeStandardAndPolicyTreeDetails(this IQueryable<StandardAndPolicyTree> queryable,
        bool include = true)
    {
        return !include ? queryable : queryable.Include(x => x.Children);
    }

    public static IQueryable<StandardAndPolicyTheme> IncludeStandardAndPolicyThemes(
        this IQueryable<StandardAndPolicyTheme> queryable, 
        bool includeLib = true,
        bool includeTree = true)
    {
        if (!includeLib && !includeTree)
        {
            return queryable;
        }

        if (includeLib && !includeTree)
        {
            return queryable.Include(x => x.Lib);
        }

        if (!includeLib && includeTree)
        {
            return queryable.Include(x => x.Tree);
        }

        if (includeLib && includeTree)
        {
            return queryable.Include(x => x.Tree).Include(x => x.Lib);
        }

        return queryable;
    }
}