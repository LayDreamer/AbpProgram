using EFCore.BulkExtensions;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using YaSha.DataManager.EntityFrameworkCore;
using YaSha.DataManager.ProductRetrieval.AggregateRoot;
using YaSha.DataManager.ProductRetrieval.Repository;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using NetTopologySuite.Index.HPRtree;

namespace YaSha.DataManager.ProductRetrieval;

public class MaterialInventoryRepository : EfCoreRepository<DataManagerDbContext, MaterialInventory, Guid>,
    IMaterialInventoryRepository
{
    public MaterialInventoryRepository(IDbContextProvider<DataManagerDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task InsertBulk(List<MaterialInventory> input)
    {
        var context = await GetDbContextAsync();
        await context.BulkInsertAsync(input);
        await context.SaveChangesAsync();
    }

    public async Task DeleteBulk(List<MaterialInventory> input)
    {
        var context = await GetDbContextAsync();
        await context.BulkDeleteAsync(input);
        await context.SaveChangesAsync();
    }

    public async Task<List<MaterialInventory>> FindByMaterialCode(List<string> code)
    {
        var warehouses = new List<string>() { "原材料", "半成品" }; /*"原材料立库", "半成品立库" , */
        var locationNames = new List<string>() { "合格", /*"不良"*/ }; //增加需求：只过滤出合格仓内容
        Expression<Func<MaterialInventory, bool>> predicate1 = (Item) => false;
        Expression<Func<MaterialInventory, bool>> predicate2 = (Item) => false;
        foreach(var item in warehouses)
            predicate1 = predicate1.Or(x => !string.IsNullOrEmpty(x.Warehouse) && x.Warehouse.Contains(item));
        foreach (var item in locationNames)
        {
            predicate2 = predicate2.Or(x => !string.IsNullOrEmpty(x.WarehouseLocationName) && x.WarehouseLocationName.Contains(item));
        }


        return await (await GetDbSetAsync())
            .Where(predicate2.And(predicate1))
            .Where(e => e.InventoryQuantity > 0 && code.Any(x => e.MaterialCode.Equals(x)))
            .ToListAsync();
    }
}