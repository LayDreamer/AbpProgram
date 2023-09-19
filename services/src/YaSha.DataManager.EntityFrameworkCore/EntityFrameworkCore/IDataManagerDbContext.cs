using YaSha.DataManager.ProductInventory.AggregateRoot;

namespace YaSha.DataManager.EntityFrameworkCore
{
    [ConnectionStringName("Default")]
    public interface IDataManagerDbContext : IEfCoreDbContext
    {
        DbSet<ProductInventTree> ProductInventTrees { get; set; }
        DbSet<ProductInventProduct> ProductInventProducts { get; set; }
        DbSet<ProductInventModule> ProductInventModules { get; set; }
        DbSet<ProductInventMaterial> ProductInventMaterials { get; set; }
    }
}
