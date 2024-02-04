using EFCore.BulkExtensions;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using YaSha.DataManager.EntityFrameworkCore;
using YaSha.DataManager.MaterialManage.Dto;
using YaSha.DataManager.ProductRetrieval.AggregateRoot;
using YaSha.DataManager.ProductRetrieval.Repository;

namespace YaSha.DataManager.ProductRetrieval;

public class ProjectInfoRepository: EfCoreRepository<DataManagerDbContext, ProjectInfo, Guid>,
    IProjectInfoRepository
{
    public ProjectInfoRepository(IDbContextProvider<DataManagerDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task InsertBulk(List<ProjectInfo> input)
    {
        var context = await GetDbContextAsync();
        await context.BulkInsertAsync(input);
        await context.SaveChangesAsync();
    }

    public async Task DeleteBulk(List<ProjectInfo> input)
    {
        var context = await GetDbContextAsync();
        await context.BulkDeleteAsync(input);
        await context.SaveChangesAsync();
    }

    public async Task<List<ProjectInfo>> FindByCodeAndType(string code, ProjectInfoInputType type)
    {
        return await (await GetDbSetAsync())
            .WhereIf(type == ProjectInfoInputType.Producut, x => 
                !string.IsNullOrEmpty(x.ProductCode) && x.ProductCode.Equals(code))
            .WhereIf(type == ProjectInfoInputType.Module, x => 
                !string.IsNullOrEmpty(x.ModuleCode) && x.ModuleCode.Equals(code))
            .WhereIf(type == ProjectInfoInputType.Material, x => 
                !string.IsNullOrEmpty(x.MaterialCode) && x.MaterialCode.Equals(code))
            .ToListAsync();
    }
}