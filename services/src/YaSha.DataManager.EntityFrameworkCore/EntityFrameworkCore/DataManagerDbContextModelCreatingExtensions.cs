using YaSha.DataManager.FamilyLibs;
using YaSha.DataManager.FamilyTrees;
using YaSha.DataManager.ProductInventory.AggregateRoot;

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
        }
    }
}