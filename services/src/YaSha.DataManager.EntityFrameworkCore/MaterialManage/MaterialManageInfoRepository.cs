using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using YaSha.DataManager.ArchitectureList.AggregateRoot;
using YaSha.DataManager.EntityFrameworkCore;
using YaSha.DataManager.MaterialManage.AggregateRoot;
using YaSha.DataManager.MaterialManage.Repository;

namespace YaSha.DataManager.MaterialManage;

public class MaterialManageInfoRepository : EfCoreRepository<DataManagerDbContext, MaterialManageInfo, Guid>,
    IMaterialManageInfoRepository
{
    public MaterialManageInfoRepository(IDbContextProvider<DataManagerDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<List<MaterialManageInfo>> FindByIds(List<Guid> ids)
    {
        return await (await GetDbSetAsync())
            .Where(x => ids.Any(y => y.Equals(x.Id)))
            .ToListAsync();
    }

    public async Task<MaterialManageInfo> FindBySequenceCodeAndSupplierCode(string sequenceCode, string supplierCode)
    {
        return await (await GetDbSetAsync())
            .Where(x =>
                !string.IsNullOrEmpty(sequenceCode) && x.SequenceCode.Equals(sequenceCode) &&
                !string.IsNullOrEmpty(supplierCode) && x.SupplierCode.Equals(supplierCode))
            .FirstOrDefaultAsync();
    }

    public async Task<List<MaterialManageInfo>> FindImageByMaterialName(string name)
    {
        return await (await GetDbSetAsync())
            .Where(x => x.SequenceCode.Equals(name) || x.SupplierCode.Equals(name))
            .ToListAsync();
    }


    public async Task<List<MaterialManageInfo>> FindImageBySeriesName(string name)
    {
        return await (await GetDbSetAsync())
            .Where(x => x.SeriesName.Equals(name))
            .ToListAsync();
    }

    public async Task<Tuple<List<string>, List<string>, List<string>>> GetManageExtraInfo()
    {
        var query = (await GetDbSetAsync())
            .AsQueryable();

        var suppliers = query
            .Where(x => !string.IsNullOrEmpty(x.Supplier))
            .OrderBy((Expression<Func<MaterialManageInfo, string>>)(e => e.Supplier))
            .Select(x => x.Supplier)
            .Distinct()
            .ToList();
        
        var supplierCodes = query
            .Where(x => !string.IsNullOrEmpty(x.SupplierCode))
            .OrderBy((Expression<Func<MaterialManageInfo, string>>)(e => e.SupplierCode))
            .Select(x => x.SupplierCode)
            .Distinct()
            .ToList();
        
        var seriesNames = query
            .Where(x => !string.IsNullOrEmpty(x.SeriesName))
            .OrderBy((Expression<Func<MaterialManageInfo, string>>)(e => e.SeriesName))
            .Select(x => x.SeriesName)
            .Distinct()
            .ToList();

        return new Tuple<List<string>, List<string>, List<string>>(suppliers, supplierCodes, seriesNames);
    }
    
    
    public async  Task<Tuple<int, List<MaterialManageInfo>>> PageManage(
        List<string> matchKeys,
        string search,
        List<string> status,
        List<string> materialType,
        List<string> supplier,
        List<string> supplierCode,
        List<string> seriesName,
        string sorting,
        int skipCount,
        int maxResultCount)
    {
        var query = (await GetDbSetAsync()).WhereIf(!string.IsNullOrEmpty(search), x =>
                x.SequenceCode.Equals(search) || x.SupplierCode.Equals(search) || (matchKeys.Count > 0 && matchKeys.Any(y => y.Equals(x.SupplierCode))))
            .WhereIf(status is { Count: > 0 }, x => status.Contains(x.Status))
            .WhereIf(materialType is { Count: > 0 }, x => materialType.Contains(x.MaterialType))
            .WhereIf(supplier is { Count: > 0 }, x => supplier.Contains(x.Supplier))
            .WhereIf(supplierCode is { Count: > 0 }, x => supplierCode.Contains(x.SupplierCode))
            .WhereIf(seriesName is { Count: > 0 }, x => seriesName.Contains(x.SeriesName));
            
        if (!string.IsNullOrEmpty(sorting))
        {
            query = query.OrderBy(sorting);
        }
        else
        {
            query = query.OrderByDescending<MaterialManageInfo, DateTime>(
                (Expression<Func<MaterialManageInfo, DateTime>>)(e => ((IHasCreationTime)e).CreationTime));
        }

        int total = await query.CountAsync();

        query = query.Skip((skipCount - 1) * maxResultCount).Take(maxResultCount);

        var result = await query.ToListAsync();

        return new Tuple<int, List<MaterialManageInfo>>(total, result);
    }

    public async Task<Tuple<int, List<MaterialManageInfo>, List<string>, List<string>, List<string>>> PageHome(
        Dictionary<Guid, float> maps,
        List<string> materialType,
        List<string> materialTexture,
        List<string> materialSurface,
        List<string> supplier,
        string search,
        string sorting,
        int skipCount,
        int maxResultCount)
    {
        //拍照返回id
        var ids = maps.Select(x => x.Key).ToList();
        var query = (await GetDbSetAsync())
            .Where(x => x.Status == "供应商在市")
            .WhereIf(ids.Count > 0, x => ids.Any(y => y.Equals(x.Id)))
            // .WhereIf(materialType is { Count: > 0 } && !materialType.Contains("全部"), x => materialType.Contains(x.MaterialType))
            // .WhereIf(materialTexture is { Count: > 0 } && !materialTexture.Contains("全部"), x => materialTexture.Contains(x.MaterialTexture))
            // .WhereIf(materialSurface is { Count: > 0 } && !materialSurface.Contains("全部"), x => materialSurface.Contains(x.MaterialSurface))
            .WhereIf(!string.IsNullOrEmpty(search), x =>
                x.SequenceCode.Equals(search) || x.SupplierCode.Equals(search) || (supplier.Count > 0 && supplier.Any(y => y.Equals(x.SupplierCode))))
            .AsQueryable();


        var types = query
            .WhereIf(materialTexture is { Count: > 0 } && !materialTexture.Contains("全部"), x => materialTexture.Contains(x.MaterialTexture))
            .WhereIf(materialSurface is { Count: > 0 } && !materialSurface.Contains("全部"), x => materialSurface.Contains(x.MaterialSurface))
            .Where(x => !string.IsNullOrEmpty(x.MaterialType)).Select(x => x.MaterialType).Distinct().ToList();

        var texture = query
            .WhereIf(materialType is { Count: > 0 } && !materialType.Contains("全部"), x => materialType.Contains(x.MaterialType))
            .WhereIf(materialSurface is { Count: > 0 } && !materialSurface.Contains("全部"), x => materialSurface.Contains(x.MaterialSurface))
            .Where(x => !string.IsNullOrEmpty(x.MaterialTexture)).Select(x => x.MaterialTexture).Distinct().ToList();

        var surface = query
            .WhereIf(materialType is { Count: > 0 } && !materialType.Contains("全部"), x => materialType.Contains(x.MaterialType))
            .WhereIf(materialTexture is { Count: > 0 } && !materialTexture.Contains("全部"), x => materialTexture.Contains(x.MaterialTexture))
            .Where(x => !string.IsNullOrEmpty(x.MaterialSurface)).Select(x => x.MaterialSurface).Distinct().ToList();


        query = query
            .WhereIf(materialType is { Count: > 0 } && !materialType.Contains("全部"), x => materialType.Contains(x.MaterialType))
            .WhereIf(materialTexture is { Count: > 0 } && !materialTexture.Contains("全部"), x => materialTexture.Contains(x.MaterialTexture))
            .WhereIf(materialSurface is { Count: > 0 } && !materialSurface.Contains("全部"), x => materialSurface.Contains(x.MaterialSurface))
            .AsQueryable();

        if (!string.IsNullOrEmpty(sorting))
        {
            query = query.OrderBy(sorting);
        }
        else
        {
            query = query.OrderByDescending(
                (Expression<Func<MaterialManageInfo, DateTime>>)(e => ((IHasCreationTime)e).CreationTime));
        }

        // var types = query.Where(x =>
        //     !string.IsNullOrEmpty(x.MaterialType)).Select(x => x.MaterialType).Distinct().ToList();
        // var texture = query.Where(x =>
        //     !string.IsNullOrEmpty(x.MaterialTexture)).Select(x => x.MaterialTexture).Distinct().ToList();
        // var surface = query.Where(x =>
        //     !string.IsNullOrEmpty(x.MaterialSurface)).Select(x => x.MaterialSurface).Distinct().ToList();

        var total = await query.CountAsync();
        List<MaterialManageInfo> result;
        if (maps.Count > 0)
        {
            result = (await query.ToListAsync())
                .OrderDescending(new CompareSource(maps))
                .Skip((skipCount - 1) * maxResultCount)
                .Take(maxResultCount).ToList();
        }
        else
        {
            query = query.Skip((skipCount - 1) * maxResultCount).Take(maxResultCount);
            result = await query.ToListAsync();
        }

        return new Tuple<int, List<MaterialManageInfo>, List<string>, List<string>, List<string>>(total, result, types, texture, surface);
    }
}

public class CompareSource : IComparer<MaterialManageInfo>
{
    private Dictionary<Guid, float> map;

    public int Compare(MaterialManageInfo x, MaterialManageInfo y)
    {
        try
        {
            var a = map[x.Id];
            var b = map[y.Id];
            if (a == b)
            {
                return 0;
            }

            if (a > b)
            {
                return 1;
            }

            return -1;
        }
        catch
        {
            return -1;
        }
    }

    public CompareSource(Dictionary<Guid, float> map)
    {
        this.map = map;
    }

    public CompareSource()
    {
    }
}