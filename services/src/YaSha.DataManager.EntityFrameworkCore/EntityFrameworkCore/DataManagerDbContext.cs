using YaSha.DataManager.ArchitectureList.AggregateRoot;
using YaSha.DataManager.DataDictionaryManagement.DataDictionaries.Aggregates;
using YaSha.DataManager.Domain;
using YaSha.DataManager.FamilyLibs;
using YaSha.DataManager.LanguageManagement.EntityFrameworkCore;
using YaSha.DataManager.LanguageManagement.Languages.Aggregates;
using YaSha.DataManager.LanguageManagement.LanguageTexts.Aggregates;
using YaSha.DataManager.MaterialManage.AggregateRoot;
using YaSha.DataManager.MeasuringExcels;
using YaSha.DataManager.NewFamilyLibrary;
using YaSha.DataManager.NotificationManagement.Notifications.Aggregates;
using YaSha.DataManager.ProcessingLists;
using YaSha.DataManager.ProductInventory.AggregateRoot;
using YaSha.DataManager.ProductRetrieval;
using YaSha.DataManager.ProductRetrieval.AggregateRoot;
using YaSha.DataManager.SaleOderList;
using YaSha.DataManager.StandardAndPolicy.AggregateRoot;

namespace YaSha.DataManager.EntityFrameworkCore
{
    /* This is your actual DbContext used on runtime.
     * It includes only your entities.
     * It does not include entities of the used modules, because each module has already
     * its own DbContext class. If you want to share some database tables with the used modules,
     * just create a structure like done for AppUser.
     *
     * Don't use this DbContext for database migrations since it does not contain tables of the
     * used modules (as explained above). See DataManagerMigrationsDbContext for migrations.
     */
    [ConnectionStringName("Default")]
    public class DataManagerDbContext : AbpDbContext<DataManagerDbContext>, IDataManagerDbContext,
        IBasicManagementDbContext,
        INotificationManagementDbContext,
        IDataDictionaryManagementDbContext,
        ILanguageManagementDbContext
    {
        public DbSet<DomainUser> DomainUsers { get; set; }
        public DbSet<IdentityUser> Users { get; set; }
        public DbSet<IdentityRole> Roles { get; set; }
        public DbSet<IdentityClaimType> ClaimTypes { get; set; }
        public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
        public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
        public DbSet<IdentityLinkUser> LinkUsers { get; set; }
        public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
        public DbSet<FeatureGroupDefinitionRecord> FeatureGroups { get; set; }
        public DbSet<FeatureDefinitionRecord> Features { get; set; }
        public DbSet<FeatureValue> FeatureValues { get; set; }
        public DbSet<PermissionGroupDefinitionRecord> PermissionGroups { get; set; }
        public DbSet<PermissionDefinitionRecord> Permissions { get; set; }
        public DbSet<PermissionGrant> PermissionGrants { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }
        public DbSet<BackgroundJobRecord> BackgroundJobs { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<DataDictionary> DataDictionaries { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<LanguageText> LanguageTexts { get; set; }
        public DbSet<FamilyTree> Trees { get; set; }
        public DbSet<FamilyLib> FamilyLibs { get; set; }
        public DbSet<MaterialInventory> MaterialInventoryInfo { get; set; }
        public DbSet<ProjectInfo> ProjectInfos{ get; set; }
        public DbSet<ProductInventTree> ProductInventTrees { get; set; }
        public DbSet<ProductInventProduct> ProductInventProducts { get; set; }
        public DbSet<ProductInventModule> ProductInventModules { get; set; }
        public DbSet<ProductInventMaterial> ProductInventMaterials { get; set; }
        public DbSet<StandardAndPolicyTree> StandardAndPolicyTrees { get; set; }
        public DbSet<StandardAndPolicyLib> StandardAndPolicyLibs { get; set; }
        public DbSet<StandardAndPolicyTheme> StandardAndPolicyThemes { get; set; }
        public DbSet<StandardAndPolicyCollect> StandardAndPolicyCollects { get; set; }
        public DbSet<ListProcessing.ListProcessing> ListProcessings { get; set; }
        public DbSet<ProcessingList> ProcessingLists { get; set; }
        public DbSet<ProcessingData> ProcessingDatas { get; set; }
        public DbSet<ArchitectureListTree> ArchitectureListTrees { get; set; }
        public DbSet<ArchitectureListModule> ArchitectureListModules { get; set; }
        public DbSet<ArchitectureListMaterial> ArchitectureListMaterials { get; set; }
        public DbSet<ArchitectureListFile> ArchitectureListFiles { get; set; }
        public DbSet<ArchitectureListModuleFile> ArchitectureListModuleFiles { get; set; }
        public DbSet<MaterialManageInfo> MaterialManageInfos { get; set; }
        public DbSet<MeasuringExcel> MeasuringExcels { get; set; }
        public DbSet<SaleOder> SaleOderList { get; set; }
        public DbSet<NewFamilyTree> NewFamilyTrees { get; set; }
        public DbSet<NewFamilyLib> NewFamilyLibs { get; set; }

        public DataManagerDbContext(DbContextOptions<DataManagerDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            // 如何设置表前缀
            // Abp框架表前缀 Abp得不建议修改表前缀
            // AbpCommonDbProperties.DbTablePrefix = "xxx";

            // 数据字典表前缀
            //DataDictionaryManagementDbProperties=“xxx”
            // 通知模块
            //NotificationManagementDbProperties = "xxx"
            base.OnModelCreating(builder);


            builder.ConfigureDataManager();

            // 基础模块
            builder.ConfigureBasicManagement();

            // 数据字典
            builder.ConfigureDataDictionaryManagement();

            // 消息通知
            builder.ConfigureNotificationManagement();

            // 多语言
            builder.ConfigureLanguageManagement();

          
        }

    }
}