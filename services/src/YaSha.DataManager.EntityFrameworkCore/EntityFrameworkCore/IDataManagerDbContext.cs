using YaSha.DataManager.ArchitectureList.AggregateRoot;
using YaSha.DataManager.Domain;
using YaSha.DataManager.FamilyLibs;
using YaSha.DataManager.MaterialManage.AggregateRoot;
using YaSha.DataManager.NewFamilyLibrary;
using YaSha.DataManager.ProcessingLists;
using YaSha.DataManager.ProductInventory.AggregateRoot;
using YaSha.DataManager.ProductRetrieval;
using YaSha.DataManager.ProductRetrieval.AggregateRoot;
using YaSha.DataManager.StandardAndPolicy.AggregateRoot;

namespace YaSha.DataManager.EntityFrameworkCore
{
    [ConnectionStringName("Default")]
    public interface IDataManagerDbContext : IEfCoreDbContext
    {
        DbSet<DomainUser> DomainUsers { get; set; }
        DbSet<FamilyTree> Trees { get; set; }
        DbSet<FamilyLib> FamilyLibs { get; set; }
        DbSet<MaterialInventory> MaterialInventoryInfo { get; set; }
        DbSet<ProjectInfo> ProjectInfos{ get; set; }
        DbSet<ProductInventTree> ProductInventTrees { get; set; }
        DbSet<ProductInventProduct> ProductInventProducts { get; set; }
        DbSet<ProductInventModule> ProductInventModules { get; set; }
        DbSet<ProductInventMaterial> ProductInventMaterials { get; set; }
        DbSet<StandardAndPolicyTree> StandardAndPolicyTrees { get; set; }
        DbSet<StandardAndPolicyLib> StandardAndPolicyLibs { get; set; }
        DbSet<StandardAndPolicyTheme> StandardAndPolicyThemes { get; set; }
        DbSet<StandardAndPolicyCollect> StandardAndPolicyCollects { get; set; }
        DbSet<ListProcessing.ListProcessing> ListProcessings { get; set; }
        DbSet<ProcessingList> ProcessingLists { get; set; }
        DbSet<ProcessingData> ProcessingDatas { get; set; }
        DbSet<ArchitectureListTree> ArchitectureListTrees { get; set; }
        DbSet<ArchitectureListModule> ArchitectureListModules { get; set; }
        DbSet<ArchitectureListMaterial> ArchitectureListMaterials { get; set; }
        DbSet<ArchitectureListFile> ArchitectureListFiles { get; set; }
        DbSet<ArchitectureListModuleFile> ArchitectureListModuleFiles { get; set; }

        DbSet<NewFamilyTree> NewFamilyTrees { get; set; }
        DbSet<NewFamilyLib> NewFamilyLibs { get; set; }
        DbSet<MaterialManageInfo> MaterialManageInfos { get; set; }
    }
}
