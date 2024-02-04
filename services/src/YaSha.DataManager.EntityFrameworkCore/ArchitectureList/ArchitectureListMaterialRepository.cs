using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using YaSha.DataManager.ArchitectureList.AggregateRoot;
using YaSha.DataManager.ArchitectureList.Repository;
using YaSha.DataManager.EntityFrameworkCore;

namespace YaSha.DataManager.ArchitectureList;

public class ArchitectureListMaterialRepository : EfCoreRepository<DataManagerDbContext, ArchitectureListMaterial, Guid>,
    IArchitectureListMaterialRepository
{
    public ArchitectureListMaterialRepository(IDbContextProvider<DataManagerDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }
}