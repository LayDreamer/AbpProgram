using Volo.Abp.Domain.Repositories;
using YaSha.DataManager.ProductInventory.AggregateRoot;

namespace YaSha.DataManager.ProductInventory.Repository;

public interface IProductInvProductRepository : IBasicRepository<ProductInventProduct,Guid>
{
    Task<ProductInventProduct> InsertProduct(Guid parentId, ProductInventProduct product);
    Task<ProductInventProduct> DeleteProduct(Guid id);
    Task<ProductInventProduct> DeleteProduct(ProductInventProduct product);
    
    Task<ProductInventProduct> FindByTreeIdAndNameAndCode(Guid treeId, string name, string code, bool include = false);
    Task<List<ProductInventProduct>> FindProductIndex(string system, string series, string name, string code, string sorting, bool include = false);
    Task<ProductInventProduct> FindById(Guid id, bool include = false);
    Task<ProductInventProduct> FindByCode(string code, bool include = false);
    Task<List<ProductInventProduct>> FindByIds(List<Guid> ids, bool include = false);
    Task<IQueryable<ProductInventProduct>> FindProductsByTreeIdsAndNameCode(IEnumerable<Guid> ids, string inputName,
        string inputCode,ProductInventoryPublishStatus inventoryPublishStatus);

    IQueryable<ProductInventProduct> SortAndPageProducts(IQueryable<ProductInventProduct> query,
        string sorting, int skip, int maxResult);

}