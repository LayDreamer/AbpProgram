using YaSha.DataManager.ArchitectureList.AggregateRoot;

namespace YaSha.DataManager.ArchitectureList;

public static class ArchitectureListQueryableExtensions
{
    public static IQueryable<ArchitectureListTree> IncludeTreeDetails(this IQueryable<ArchitectureListTree> queryable,
        bool include = true)
    {
        return !include
            ? queryable
            : queryable
                .Include(x => x.Children);
    }
    
    public static IQueryable<ArchitectureListModule> IncludeModuleDetails(
        this IQueryable<ArchitectureListModule> queryable,
        bool include = true)
    {
        return !include
            ? queryable
            : queryable.Include(x => x.Materials);
    }
    
    public static IQueryable<ArchitectureListModuleFile> IncludeModuleFileDetails(
        this IQueryable<ArchitectureListModuleFile> queryable,
        bool include = true)
    {
        return !include
            ? queryable
            : queryable.Include(x => x.File);
    }
}