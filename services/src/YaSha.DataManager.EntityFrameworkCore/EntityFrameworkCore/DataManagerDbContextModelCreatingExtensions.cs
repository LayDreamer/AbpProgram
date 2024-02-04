using YaSha.DataManager.ArchitectureList.AggregateRoot;
using YaSha.DataManager.Domain;
using YaSha.DataManager.FamilyLibs;
using YaSha.DataManager.MaterialManage.AggregateRoot;
using YaSha.DataManager.MeasuringExcels;
using YaSha.DataManager.NewFamilyLibrary;
using YaSha.DataManager.ProcessingLists;
using YaSha.DataManager.ProductInventory.AggregateRoot;
using YaSha.DataManager.ProductRetrieval;
using YaSha.DataManager.ProductRetrieval.AggregateRoot;
using YaSha.DataManager.SaleOderList;
using YaSha.DataManager.StandardAndPolicy.AggregateRoot;

namespace YaSha.DataManager.EntityFrameworkCore
{
    public static class DataManagerDbContextModelCreatingExtensions
    {
        public static void ConfigureDataManager(this ModelBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            /* Configure your own tables/entities inside here */
            builder.Entity<FamilyTree>(b =>
            {
                b.ToTable(DataManagerConsts.DbTablePrefix + "FamilyTree",
                    DataManagerConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.DisplayName).IsRequired().HasMaxLength(128);
            });

            builder.Entity<FamilyLib>(b =>
            {
                b.ToTable(DataManagerConsts.DbTablePrefix + "FamilyLibs",
                    DataManagerConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.DisplayName).IsRequired().HasMaxLength(128);
            });

            builder.Entity<MaterialInventory>(b =>
            {
                b.ToTable(DataManagerConsts.DbTablePrefix + "MaterialInventoryInfo",
                    DataManagerConsts.DbSchema);
                b.ConfigureByConvention();
            });

            builder.Entity<ProductInventTree>(b =>
            {
                b.ToTable(DataManagerConsts.DbTablePrefix + nameof(ProductInventTree),
                    DataManagerConsts.DbSchema);
                b.ConfigureByConvention();
            });

            builder.Entity<ProductInventProduct>(b =>
            {
                b.ToTable(DataManagerConsts.DbTablePrefix + nameof(ProductInventProduct),
                    DataManagerConsts.DbSchema);
                b.ConfigureByConvention();
            });

            builder.Entity<ProductInventModule>(b =>
            {
                b.ToTable(DataManagerConsts.DbTablePrefix + nameof(ProductInventModule),
                    DataManagerConsts.DbSchema);
                b.ConfigureByConvention();
            });

            builder.Entity<ProductInventMaterial>(b =>
            {
                b.ToTable(DataManagerConsts.DbTablePrefix + nameof(ProductInventMaterial),
                    DataManagerConsts.DbSchema);
                b.ConfigureByConvention();
            });

            builder.Entity<ProjectInfo>(b =>
            {
                b.ToTable(DataManagerConsts.DbTablePrefix + nameof(ProjectInfo),
                    DataManagerConsts.DbSchema);
                b.ConfigureByConvention();
            });


            builder.Entity<DomainUser>(b =>
            {
                b.ToTable(AbpCommonDbProperties.DbTablePrefix + "DomainUsers",
                        AbpCommonDbProperties.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.UserName).IsRequired().HasMaxLength(128);
            });

            builder.Entity<StandardAndPolicyTree>(b =>
            {
                b.ToTable(DataManagerConsts.DbTablePrefix + nameof(StandardAndPolicyTree),
                    DataManagerConsts.DbSchema);
                b.ConfigureByConvention();
            });

            builder.Entity<StandardAndPolicyLib>(b =>
            {
                b.ToTable(DataManagerConsts.DbTablePrefix + nameof(StandardAndPolicyLib),
                    DataManagerConsts.DbSchema);
                b.ConfigureByConvention();
            });

            builder.Entity<StandardAndPolicyTheme>(b =>
            {
                b.ToTable(DataManagerConsts.DbTablePrefix + nameof(StandardAndPolicyTheme),
                    DataManagerConsts.DbSchema);
                b.ConfigureByConvention();
            });

            builder.Entity<StandardAndPolicyCollect>(b =>
            {
                b.ToTable(DataManagerConsts.DbTablePrefix + nameof(StandardAndPolicyCollect),
                    DataManagerConsts.DbSchema);
                b.ConfigureByConvention();
            });

            builder.Entity<ProcessingList>(b =>
            {
                b.ToTable(DataManagerConsts.DbTablePrefix + "ProcessingLists",
                    DataManagerConsts.DbSchema);
                b.ConfigureByConvention();
            });

            builder.Entity<ProcessingData>(b =>
            {
                b.ToTable(DataManagerConsts.DbTablePrefix + "ProcessingDatas",
                    DataManagerConsts.DbSchema);
                b.ConfigureByConvention();
            });


            builder.Entity<ListProcessing.ListProcessing>(b =>
            {
                b.ToTable(DataManagerConsts.DbTablePrefix + nameof(ListProcessing.ListProcessing),
                    DataManagerConsts.DbSchema);
                b.ConfigureByConvention();
            });

            builder.Entity<ArchitectureListTree>(b =>
            {
                b.ToTable(DataManagerConsts.DbTablePrefix + nameof(ArchitectureListTree),
                    DataManagerConsts.DbSchema);
                b.ConfigureByConvention();
            });

            builder.Entity<ArchitectureListModule>(b =>
            {
                b.ToTable(DataManagerConsts.DbTablePrefix + nameof(ArchitectureListModule),
                    DataManagerConsts.DbSchema);
                b.ConfigureByConvention();
            });

            builder.Entity<ArchitectureListMaterial>(b =>
            {
                b.ToTable(DataManagerConsts.DbTablePrefix + nameof(ArchitectureListMaterial),
                    DataManagerConsts.DbSchema);
                b.ConfigureByConvention();
            });
            
            builder.Entity<ArchitectureListFile>(b =>
            {
                b.ToTable(DataManagerConsts.DbTablePrefix + nameof(ArchitectureListFile),
                    DataManagerConsts.DbSchema);
                b.ConfigureByConvention();
            });
            
            builder.Entity<ArchitectureListModuleFile>(b =>
            {
                b.ToTable(DataManagerConsts.DbTablePrefix + nameof(ArchitectureListModuleFile),
                    DataManagerConsts.DbSchema);
                b.ConfigureByConvention();
            });

            builder.Entity<MeasuringExcel>(b =>
            {
                b.ToTable(DataManagerConsts.DbTablePrefix + "MeasuringExcels",
                    DataManagerConsts.DbSchema);
                b.ConfigureByConvention();
            });

            builder.Entity<SaleOder>(b =>
            {
                b.ToTable(DataManagerConsts.DbTablePrefix + nameof(SaleOder),
                    DataManagerConsts.DbSchema);
                b.ConfigureByConvention();
            });

            builder.Entity<NewFamilyTree>(b =>
            {
                b.ToTable(DataManagerConsts.DbTablePrefix + nameof(NewFamilyTree),
                    DataManagerConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.DisplayName).IsRequired().HasMaxLength(128);
            });

            builder.Entity<NewFamilyLib>(b =>
            {
                b.ToTable(DataManagerConsts.DbTablePrefix + nameof(NewFamilyLib),
                    DataManagerConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.DisplayName).IsRequired().HasMaxLength(128);
            });

            builder.Entity<MaterialManageInfo>(b =>
            {
                b.ToTable(DataManagerConsts.DbTablePrefix + nameof(MaterialManageInfo),
                    DataManagerConsts.DbSchema);
                b.ConfigureByConvention();
            });
        }
    }
}