using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Caching;
using YaSha.DataManager.MaterialManage.AggregateRoot;
using YaSha.DataManager.MaterialManage.Dto;
using YaSha.DataManager.MaterialManage.Repository;
using YaSha.DataManager.ProductRetrieval.Repository;

namespace YaSha.DataManager.MaterialManage.Manager;

public class MaterialManageManager : DataManagerDomainService
{
    private readonly IMaterialManageInfoRepository _repository;
    private readonly IMaterialInventoryRepository _inventoryRepository;


    public MaterialManageManager(IMaterialManageInfoRepository repository, IMaterialInventoryRepository inventoryRepository)
    {
        _repository = repository;
        _inventoryRepository = inventoryRepository;
    }

    #region 增

    public async Task Insert(MaterialManageDto insert)
    {
        var materialManageInfo = ObjectMapper.Map<MaterialManageDto, MaterialManageInfo>(insert);
        await _repository.InsertAsync(materialManageInfo, true);
    }

    #endregion

    #region 删

    public async Task Delete(Guid id)
    {
        await _repository.DeleteAsync(id, true);
    }

    #endregion

    #region 改

    public async Task Update(MaterialManageDto dto)
    {
        var update = new List<MaterialManageInfo>();
        var dbEnt = await _repository.FindAsync(dto.Id);
        if (dbEnt != null)
        {
            Update(dbEnt, dto);
        }

        await _repository.UpdateAsync(dbEnt, true);
    }

    public async Task UpdateMaterialImage(MaterialManageDto dto)
    {
        var dbEnt = await _repository.FindAsync(dto.Id);
        if (dbEnt != null)
        {
            dbEnt.MaterialImageUrl = dto.MaterialImageUrl;
            dbEnt.MaterialImageDownLoadUrl = dto.MaterialImageDownLoadUrl;
            await _repository.UpdateAsync(dbEnt, true);
        }
    }

    public async Task UpdateSeriesImage(MaterialManageDto dto)
    {
        var dbEnt = await _repository.FindAsync(dto.Id);
        if (dbEnt != null)
        {
            dbEnt.SeriesImageUrl = dto.SeriesImageUrl;
            await _repository.UpdateAsync(dbEnt, true);
        }
    }

    void Update(MaterialManageInfo db, MaterialManageDto dto)
    {
        db.Status = dto.Status;
        db.MaterialImageUrl = dto.MaterialImageUrl;
        db.MaterialImageDownLoadUrl = dto.MaterialImageDownLoadUrl;
        db.MaterialType = dto.MaterialType;
        db.MaterialTexture = dto.MaterialTexture;
        db.MaterialSurface = dto.MaterialSurface;
        db.SequenceCode = dto.SequenceCode;
        db.StoragePlace = dto.StoragePlace;
        db.Width = dto.Width;
        db.Thickness = dto.Thickness;
        db.Weight = dto.Weight;
        db.Length = dto.Length;
        db.Supplier = dto.Supplier;
        db.SupplierOverview = dto.SupplierOverview;
        db.SupplierCode = dto.SupplierCode;
        db.Price = dto.Price;
        db.Quantity = dto.Quantity;
        db.DeliveryCycle = dto.DeliveryCycle;
        db.SeriesName = dto.SeriesName;
        db.SeriesImageUrl = dto.SeriesImageUrl;
        db.SeriesCode = dto.SeriesCode;
        db.OperatingUser = dto.OperatingUser;
    }

    #endregion

    #region 查

    public async Task CheckInsertInvalid(MaterialManageDto input)
    {
        var check = await _repository.FindBySequenceCodeAndSupplierCode(input.SequenceCode, input.SupplierCode);
        if (check != null)
        {
            throw new Exception($"同供应商编码{input.SupplierCode}下顺序码{input.SequenceCode}输入重复");
        }
    }


    public async Task<MaterialManageDto> FindById(Guid id)
    {
        var dbEnt = await _repository.FindAsync(id);
        return ObjectMapper.Map<MaterialManageInfo, MaterialManageDto>(dbEnt);
    }

    public async Task<List<MaterialManageDto>> FindByMaterialName(string fileName)
    {
        var dbEntities = await _repository.FindImageByMaterialName(fileName);
        return ObjectMapper.Map<List<MaterialManageInfo>, List<MaterialManageDto>>(dbEntities);
    }


    public async Task<List<MaterialManageDto>> FindBySeriesName(string fileName)
    {
        var dbEntities = await _repository.FindImageBySeriesName(fileName);
        return ObjectMapper.Map<List<MaterialManageInfo>, List<MaterialManageDto>>(dbEntities);
    }


    static bool AreWidthEquivalent(string str1, string str2)
    {
        if (string.IsNullOrEmpty(str1) || string.IsNullOrEmpty(str2))
        {
            return false;
        }

        var a = str1.Trim();
        var b = str2.Trim();
        if (decimal.TryParse(a, out var value1) && decimal.TryParse(b, out var value2))
        {
            return Math.Round(value1, 3) == Math.Round(value2, 3);
        }

        return false;
    }

    static bool AreHeightEquivalent(string str1, string str2)
    {
        if (string.IsNullOrEmpty(str1) || string.IsNullOrEmpty(str2))
        {
            return false;
        }

        var pattern = "[盎司ozOZsS]";
        var processedStr1 = Regex.Replace(str1.Trim(), pattern, "");
        var processedStr2 = Regex.Replace(str2.Trim(), pattern, "");
        return processedStr1.Equals(processedStr2);
    }

    string FindMaterialCode(IEnumerable<MaterialManageErpCacheData> map, string supplierCode, string w, string h)
    {
        var result = string.Empty;

        var list = map.Where(x =>
            x.SupplierCode.Equals(supplierCode) &&
            AreWidthEquivalent(x.Width, w) &&
            AreHeightEquivalent(x.Height, h)).ToList();

        if (list.Any())
        {
            result = list.First().MaterialCode;
        }

        return result;
    }

    public async Task<MaterialManageManageExtraInfo> GetManageExtra()
    {
        var info = await _repository.GetManageExtraInfo();
        return new MaterialManageManageExtraInfo()
        {
            Supplier = info.Item1,
            SupplierCode = info.Item2,
            SeriesName = info.Item3,
        };
    }

    public async Task<PagedResultDto<MaterialManageFullDto>> PageManage(
        List<MaterialManageErpCacheData> map,
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
        //找到物料编码是search的 
        var matchingKeys = map.Where(pair => pair.MaterialCode == search).Select(pair => pair.SupplierCode).ToList();
        var result = await _repository.PageManage(matchingKeys, search, status,
            materialType, supplier, supplierCode, seriesName, sorting, skipCount, maxResultCount);
        var dto = ObjectMapper.Map<List<MaterialManageInfo>, List<MaterialManageFullDto>>(result.Item2);
        foreach (var item in dto)
        {
            if (string.IsNullOrEmpty(item.SupplierCode)) continue;
            var materialCode = FindMaterialCode(map, item.SupplierCode, item.Width, item.Weight);
            if (!string.IsNullOrEmpty(materialCode))
            {
                item.MaterialCode = materialCode;
            }

            if (string.IsNullOrEmpty(item.MaterialCode)) continue;
            var find = await _inventoryRepository.FindByMaterialCode(new List<string> { item.MaterialCode });
            if (find.Count <= 0) continue;
            item.ProductionBatch = string.Join("、", find.Select(x => x.ProductionBatch).ToList());
            item.StorageTime = string.Join("、", find.Select(x =>
                DateTime.ParseExact(x.StorageTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
                    .ToString("yyyy-MM-dd")));
            item.BatchInventory = string.Join("、", find.Select(x =>
                x.InventoryQuantity + x.Unit));
            item.TotalInventory = find.Sum(x => x.InventoryQuantity) + find[0].Unit;
        }

        return new PagedResultDto<MaterialManageFullDto>(
            result.Item1, dto
        );
    }

    public async Task<MaterialManageHomeDto> PageHome(
        List<MaterialManageErpCacheData> map,
        Dictionary<Guid, float> apiMap,
        List<string> materialType,
        List<string> materialTexture,
        List<string> materialSurface,
        string search,
        string sorting = "",
        int skipCount = 1,
        int maxResultCount = 10)
    {
        var matchingKeys = map.Where(pair => pair.MaterialCode == search).Select(pair => pair.SupplierCode).ToList();
        var homeData = await _repository.PageHome(
            apiMap,
            materialType,
            materialTexture,
            materialSurface,
            matchingKeys,
            search,
            sorting,
            skipCount,
            maxResultCount);
        var result = new MaterialManageHomeDto()
        {
            TotalCount = homeData.Item1,
            MaterialTypes = homeData.Item3,
            MaterialTextures = homeData.Item4,
            MaterialSurfaces = homeData.Item5,
        };

        var dto = ObjectMapper.Map<List<MaterialManageInfo>, List<MaterialManageDto>>(homeData.Item2);
        var homeItems = dto.Select(item => new MaterialManageHomeItem
            {
                Id = item.Id,
                MaterialImageUrl = item.MaterialImageUrl,
                SequenceCode = item.SequenceCode,
                MaterialType = item.MaterialType,
                MaterialTexture = item.MaterialTexture,
                MaterialSurface = item.MaterialSurface,
                Similarity = apiMap.Count == 0 ? null : (apiMap[item.Id] * 100).ToString("0.0"),
            })
            .ToList();

        result.Items = homeItems;
        return result;
    }

    public async Task<List<MaterialManageFullDto>> GetDetail(List<Guid> id, List<MaterialManageErpCacheData> map)
    {
        var results = new List<MaterialManageFullDto>();
        var materialInfo = await _repository.FindByIds(id);
        var dtos = ObjectMapper.Map<List<MaterialManageInfo>, List<MaterialManageFullDto>>(materialInfo);
        foreach (var dto in dtos)
        {
            if (string.IsNullOrEmpty(dto.SupplierCode))
            {
                results.Add(dto);
                continue;
            }
            var materialCode = FindMaterialCode(map, dto.SupplierCode, dto.Width, dto.Weight);
            if (!string.IsNullOrEmpty(materialCode))
            {
                dto.MaterialCode = materialCode;
            }

            if (string.IsNullOrEmpty(dto.MaterialCode))
            {
                results.Add(dto);
                continue;
            }
            var find = await _inventoryRepository.FindByMaterialCode(new List<string> { dto.MaterialCode });
            if (find.Count <= 0)
            {
                results.Add(dto);
                continue;
            }
            dto.ProductionBatch = string.Join("、", find.Select(x => x.ProductionBatch).ToList());
            dto.StorageTime = string.Join("、", find.Select(x =>
                DateTime.ParseExact(x.StorageTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
                    .ToString("yyyy-MM-dd")));
            dto.BatchInventory = string.Join("、", find.Select(x =>
                x.InventoryQuantity + x.Unit));
            dto.TotalInventory = find.Sum(x => x.InventoryQuantity) + find[0].Unit;
            results.Add(dto);
        }

        return results;
    }

    #endregion
}