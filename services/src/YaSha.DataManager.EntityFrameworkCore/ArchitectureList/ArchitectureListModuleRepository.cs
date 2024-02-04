using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using YaSha.DataManager.ArchitectureList.AggregateRoot;
using YaSha.DataManager.ArchitectureList.Repository;
using YaSha.DataManager.EntityFrameworkCore;
using YaSha.DataManager.ProductInventory;

namespace YaSha.DataManager.ArchitectureList;

public class ArchitectureListModuleRepository : EfCoreRepository<DataManagerDbContext, ArchitectureListModule, Guid>,
    IArchitectureListModuleRepository
{
    public ArchitectureListModuleRepository(IDbContextProvider<DataManagerDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }


    public async Task<ArchitectureListModule> FindById(Guid id, bool include = true)
    {
        return await (await GetDbSetAsync())
            .IncludeModuleDetails(include)
            .Where(x => x.Id.Equals(id))
            .FirstOrDefaultAsync();
    }

    public async Task<List<ArchitectureListModule>> FindByIds(List<Guid> ids, bool include = true)
    {
        return await (await GetDbSetAsync())
            .IncludeModuleDetails(include)
            .Where(x => ids.Any(y => y.Equals(x.Id)))
            .ToListAsync();
    }

    public async Task<ArchitectureListModule> FindByNameAndProcess(string name, string process, bool include = true)
    {
        return await (await GetDbSetAsync())
            .IncludeModuleDetails(include)
            .Where(x => x.Name == name && x.ProcessingMode == process)
            .FirstOrDefaultAsync();
    }

    public async Task<ArchitectureListModule> FindByTypeAndProcess(string name, string process, bool include = false)
    {
        return await (await GetDbSetAsync())
          .IncludeModuleDetails(include)
          .Where(x => x.Model== name && x.ProcessingMode == process)
          .FirstOrDefaultAsync();
    }


    public async Task<ArchitectureListModule> FindByModel(string model, string system, bool include = true)
    {
        return await (await GetDbSetAsync())
            .IncludeModuleDetails(include)
            .FirstOrDefaultAsync(x => x.Model == model && x.System == system && !x.IsProcessing && x.ProcessingMode == "标准加工");
    }

    public async Task<ArchitectureListModule> Exists(Guid treeId, string name, string code, string processingMode, bool include)
    {
        return await (await GetDbSetAsync())
            .IncludeModuleDetails(include)
            .Where(x =>
                x.ParentId.Equals(treeId)
                && x.Name.Equals(name)
                && x.Model.Equals(code)
                && x.ProcessingMode.Equals(processingMode))
            .FirstOrDefaultAsync();
    }

    public async Task<Tuple<int, List<ArchitectureListModule>>> PageModule(
        List<Guid> ids,
        string value,
        string code,
        ArchitectureListPublishStatus status,
        string sorting,
        int skipCount,
        int maxCount,
        bool include = true)
    {
        var query = (await GetDbSetAsync())
            .IncludeModuleDetails(include)
            .Where(x => x.Status == status);

        if (ids is { Count: > 0 })
        {
            query = query.Where(x => ids.Any(y => y.Equals(x.ParentId)));
        }

        if (!string.IsNullOrEmpty(value))
        {
            query = query.Where(x => x.Name.Contains(value) || x.Materials.Any(y => y.Name.Contains(value)));
        }

        if (!string.IsNullOrEmpty(code))
        {
            query = query.Where(x => x.Model.Contains(code) || x.Materials.Any(y => y.Code.Contains(code)));
        }

        if (!string.IsNullOrEmpty(sorting))
        {
            query = query.OrderBy(sorting);
        }
        else
        {
            query = query.OrderBy<ArchitectureListModule, DateTime>(
                (Expression<Func<ArchitectureListModule, DateTime>>)(e => ((IHasCreationTime)e).CreationTime));
        }

        int total = await query.CountAsync();

        query = query.Skip((skipCount - 1) * maxCount).Take(maxCount);

        var result = await query.ToListAsync();

        return new Tuple<int, List<ArchitectureListModule>>(total, result);
    }


    public override async Task<IQueryable<ArchitectureListModule>> WithDetailsAsync(
        params Expression<Func<ArchitectureListModule, object>>[] propertySelectors)
    {
        return (await GetQueryableAsync()).IncludeModuleDetails();
    }
}