using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Caching;
using YaSha.DataManager.ProductInventory.AggregateRoot;
using YaSha.DataManager.ProductInventory.Dto;
using YaSha.DataManager.ProductInventory.Repository;

namespace YaSha.DataManager.ProductInventory.Manager;

public class ProductInventProductManager : DataManagerDomainService
{
    private readonly IProductInvTreeRepository _treeRepository;
    private readonly IProductInvProductRepository _productRepository;
    private readonly IProductInvModuleRepository _moduleRepository;
    private readonly IProductInvMaterialRepository _materialRepository;
    private readonly IDistributedCache<ProductInventoryFullDto> _distributedCache;

    public ProductInventProductManager(IProductInvTreeRepository repository,
        IProductInvProductRepository productRepository,
        IProductInvModuleRepository moduleRepository,
        IProductInvMaterialRepository materialRepository,
        IDistributedCache<ProductInventoryFullDto> distributedCache)
    {
        _treeRepository = repository;
        _productRepository = productRepository;
        _moduleRepository = moduleRepository;
        _materialRepository = materialRepository;
        _distributedCache = distributedCache;
    }

    #region 增

    public async Task<ProductInventoryProductDto> Insert(string userName, Guid id, ProductInventoryProductCreateDto dto)
    {
        await CheckProductExist(id, dto.Name, dto.Code);
        var product = ObjectMapper.Map<ProductInventoryProductCreateDto, ProductInventProduct>(dto);
        product.CreateUser = userName;
        var insertProduct = await _productRepository.InsertProduct(id, product);
        var insertDto = ObjectMapper.Map<ProductInventProduct, ProductInventoryProductDto>(insertProduct);
        return insertDto;
    }

    public async Task<List<ProductInventoryProductDto>> ImportFromExcel(string userName, [NotNull] string system,
        [NotNull] string series, string remark, List<ProductInventoryProductCreateDto> dtos)
    {
        if (system == null) throw new ArgumentNullException(nameof(system));
        if (series == null) throw new ArgumentNullException(nameof(series));
        var results = new List<ProductInventoryProductDto>();
        if (dtos.Count == 0) return results;
        var roots = await _treeRepository.GetRootTree();
        var findSystem = roots.FirstOrDefault(x => x.Name == "产品")?.Children.FirstOrDefault(x => x.Name == system);
        if (findSystem == null)
        {
            throw new BusinessException(ProductInventoryErrorCode.ErrorSystem);
        }

        var findSeries = findSystem.Children.FirstOrDefault(x => x.Name == series);
        if (findSeries == null)
        {
            throw new BusinessException(ProductInventoryErrorCode.ErrorSeries);
        }

        findSeries.Remark = remark;
        await _treeRepository.UpdateAsync(findSeries, autoSave: true);

        foreach (var item in dtos)
        {
            results.Add(await Insert(userName, findSeries.Id, item));
        }

        return results;
    }

    public async Task<ProductInventoryProductDto> AddProduct(string createUser, ProductInventoryFullDto dto)
    {
        if (string.IsNullOrEmpty(dto.Data.Series) || string.IsNullOrEmpty(dto.Data.System))
        {
            throw new BusinessException(ProductInventoryErrorCode.MissedSystemOrSeries);
        }

        var roots = await _treeRepository.GetRootTree();
        var findSystem = roots.FirstOrDefault(x => x.Name == "产品")?.Children.FirstOrDefault(x => x.Name == dto.Data.System);
        if (findSystem == null)
        {
            throw new BusinessException(ProductInventoryErrorCode.ErrorSystem);
        }

        var findSeries = findSystem.Children.FirstOrDefault(x => x.Name == dto.Data.Series);
        if (findSeries == null)
        {
            throw new BusinessException(ProductInventoryErrorCode.ErrorSeries);
        }

        var insertProduct = ObjectMapper.Map<ProductInventoryEditDto, ProductInventProduct>(dto.Data);
        insertProduct.CreateUser = createUser;
        insertProduct.Status = ProductInventoryPublishStatus.DelayInMark;
        foreach (var child in dto.Children)
        {
            switch (child.Tag)
            {
                case ProductInventroyTag.Modules:
                {
                    var module = ObjectMapper.Map<ProductInventoryEditDto, ProductInventModule>(child.Data);
                    foreach (var material in child.Children.Select(item =>
                                 ObjectMapper.Map<ProductInventoryEditDto, ProductInventMaterial>(item.Data)))
                    {
                        module.Materials.Add(material);
                    }

                    insertProduct.Modules.Add(module);
                    break;
                }
                case ProductInventroyTag.Material:
                    insertProduct.Materials.Add(
                        ObjectMapper.Map<ProductInventoryEditDto, ProductInventMaterial>(child.Data));
                    break;
            }
        }

        await _productRepository.InsertProduct(findSeries.Id, insertProduct);
        return ObjectMapper.Map<ProductInventProduct, ProductInventoryProductDto>(insertProduct);
    }

    public async Task<List<ProductInventoryProductDto>> CopyProduct(string createUser, List<Guid> ids)
    {
        var copys = new List<ProductInventProduct>();

        var products = await _productRepository.FindByIds(ids, true);

        foreach (var product in products)
        {
            var createDto = ObjectMapper.Map<ProductInventProduct, ProductInventoryProductCreateDto>(product);
            var createProduct = ObjectMapper.Map<ProductInventoryProductCreateDto, ProductInventProduct>(createDto);
            createProduct.SetParentId(product.ParentId);
            createProduct.SetCopyName();
            createProduct.ImagePath = product.ImagePath;
            createProduct.CreateUser = createUser;
            createProduct.Status = ProductInventoryPublishStatus.DelayInMark;
            copys.Add(createProduct);
        }

        await _productRepository.InsertManyAsync(copys, autoSave: true);

        return ObjectMapper.Map<List<ProductInventProduct>, List<ProductInventoryProductDto>>(copys);
    }

    #endregion


    #region 删

    private async Task CheckProductExist(Guid treeId, string name, string code)
    {
        var product = await _productRepository.FindByTreeIdAndNameAndCode(treeId, name, code, true);
        if (product != null)
        {
            await DeleteProduct(product);
        }
    }

    public async Task DeleteProduct(Guid id)
    {
        try
        {
            var deleteProduct = await _productRepository.FindById(id, true);
            await DeleteProduct(deleteProduct);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    private async Task DeleteProduct(ProductInventProduct product)
    {
        try
        {
            var modules = new List<ProductInventModule>();
            var materials = product.Materials.ToList();
            foreach (var module in product.Modules)
            {
                materials.AddRange(module.Materials);
                modules.Add(module);
            }

            await _materialRepository.DeleteMaterial(materials);
            await _moduleRepository.DeleteModule(modules);
            await _productRepository.DeleteProduct(product);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task DeleteProductEdits(List<ProductInventoryFullDto> dtos)
    {
        var modules = new List<Guid>();
        var materials = new List<Guid>();
        try
        {
            foreach (var dto in dtos)
            {
                switch (dto.Tag)
                {
                    case ProductInventroyTag.Material:
                        materials.Add(dto.Data.Id);
                        break;
                    case ProductInventroyTag.Modules:
                        modules.Add(dto.Data.Id);
                        materials.AddRange(dto.Children.Select(material => material.Data.Id));
                        break;
                }
            }

            await _materialRepository.DeleteMaterial(materials);
            await _moduleRepository.DeleteModule(modules);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    #endregion


    #region 改

    private static void UpdateProduct(ProductInventProduct product, ProductInventoryEditDto dto)
    {
        product.Code = dto.Code;
        product.Name = dto.Name;
        product.Length = dto.Length;
        product.Width = dto.Width;
        product.Height = dto.Height;
        product.Category = dto.Category;
        product.System = dto.System;
        product.Series = dto.Series;
        product.MaterialQuality = dto.MaterialQuality;
        product.Color = dto.Color;
        product.Version = dto.Version;
        product.ProcessNum = dto.ProcessNum;
        product.AssemblyDrawingNum = dto.AssemblyDrawingNum;
        product.DetailNum = dto.DetailNum;
        product.ProjectCode = dto.ProjectCode;
        product.ProjectName = dto.ProjectName;
        product.ProductSpecification = dto.ProductSpecification;
        product.LimitInfos = dto.LimitInfos;
        product.Remark = dto.Remark;
    }

    private static void UpdateModule(ProductInventModule module, ProductInventoryEditDto dto)
    {
        module.Code = dto.Code;
        module.Name = dto.Name;
        module.Length = dto.Length;
        module.Width = dto.Width;
        module.Height = dto.Height;
        module.Version = dto.Version;
        module.Level = dto.Level;
        module.Category = dto.Category;
        module.SupplyMode = dto.SupplyMode;
        module.ModuleSpecification = dto.ModuleSpecification;
        module.LimitInfos = dto.LimitInfos;
    }

    private static void UpdateMaterial(ProductInventMaterial material, ProductInventoryEditDto dto)
    {
        material.Code = dto.Code;
        material.Name = dto.Name;
        material.Level = dto.Level;
        material.Length = dto.Length;
        material.Width = dto.Width;
        material.Height = dto.Height;
        material.Unit = dto.Unit;
        material.MaterialQuality = dto.MaterialQuality;
        material.Color = dto.Color;
        material.Property = dto.Property;
        material.Usage = dto.Usage;
        material.IsProcess = dto.IsProcess;
        material.SupplyMode = dto.SupplyMode;
        material.Remark = dto.Remark;
        material.MaterialUsageFormula = dto.MaterialUsageFormula;
    }

    public async Task<ProductInventoryProductDto> UpdateProduct(string updateUser, ProductInventoryFullDto dto)
    {
        await UpdateProductEdit(updateUser, dto);
        var result = await _productRepository.FindById(dto.Data.Id, true);
        return ObjectMapper.Map<ProductInventProduct, ProductInventoryProductDto>(result);
    }


    private async Task UpdateProductEdit(string updateUser, ProductInventoryFullDto dto)
    {
        if (dto.Tag == ProductInventroyTag.Product)
        {
            if (dto.Status == ProductInventoryModifyStatus.Modify)
            {
                var product = await _productRepository.FindById(dto.Data.Id);
                if (product == null) return;
                product.ModifiyUser = updateUser;
                UpdateProduct(product, dto.Data);
                await _productRepository.UpdateAsync(product, autoSave: true);
            }
        }
        else if (dto.Tag == ProductInventroyTag.Modules)
        {
            if (dto.Status == ProductInventoryModifyStatus.Modify)
            {
                var module = await _moduleRepository.FindAsync(dto.Data.Id, includeDetails: false);
                if (module == null) return;
                UpdateModule(module, dto.Data);
                await _moduleRepository.UpdateAsync(module, autoSave: true);
            }
            else if (dto.Status == ProductInventoryModifyStatus.Insert)
            {
                var insertModule = ObjectMapper.Map<ProductInventoryEditDto, ProductInventModule>(dto.Data);
                if (dto.ParentId != null) insertModule.ParentId = dto.ParentId.Value;
                await _moduleRepository.InsertAsync(insertModule, autoSave: true);
            }
        }
        else if (dto.Tag == ProductInventroyTag.Material)
        {
            if (dto.Status == ProductInventoryModifyStatus.Modify)
            {
                var material = await _materialRepository.FindAsync(dto.Data.Id, includeDetails: false);
                if (material == null) return;
                UpdateMaterial(material, dto.Data);
                await _materialRepository.UpdateAsync(material, autoSave: true);
            }
            else if (dto.Status == ProductInventoryModifyStatus.Insert)
            {
                var insertMaterial = ObjectMapper.Map<ProductInventoryEditDto, ProductInventMaterial>(dto.Data);
                if (dto.ParentTag == ProductInventroyTag.Product)
                {
                    if (dto.ParentId != null) insertMaterial.SetParentProductId(dto.ParentId.Value);
                }
                else if (dto.ParentTag == ProductInventroyTag.Modules)
                {
                    if (dto.ParentId != null) insertMaterial.SetParentModuleId(dto.ParentId.Value);
                }

                await _materialRepository.InsertAsync(insertMaterial, autoSave: true);
            }
        }

        foreach (var child in dto.Children)
        {
            await UpdateProductEdit(updateUser, child);
        }
    }


    public async Task UpdateProductImage(string updateUser, Guid id, string imagePath)
    {
        var product = await _productRepository.FindById(id);

        if (product != null)
        {
            product.ImagePath = imagePath;
            product.ModifiyUser = updateUser;
            await _productRepository.UpdateAsync(product);
        }
    }

    public async Task<List<ProductInventoryProductDto>> PublishProducts(Dictionary<Guid, ProductInventoryPublishStatus> dto)
    {
        var results = new List<ProductInventoryProductDto>();
        var update = new List<ProductInventProduct>();
        foreach (var item in dto)
        {
            var product = await _productRepository.FindById(item.Key);
            if (product == null) continue;
            product.Status = item.Value;
            results.Add(ObjectMapper.Map<ProductInventProduct, ProductInventoryProductDto>(product));
            update.Add(product);
        }

        await _productRepository.UpdateManyAsync(update, autoSave: true);
        return results;
    }

    #endregion


    #region 查

    public async Task<PagedResultDto<ProductInventoryProductDto>> FindProductBySearchDto(
        string key,
        string searchValue,
        string searchCode,
        ProductInventoryPublishStatus status,
        string sorting,
        int skipCount,
        int maxResultCount)
    {
        if (string.IsNullOrEmpty(key))
        {
            return new PagedResultDto<ProductInventoryProductDto>()
            {
                TotalCount = 0,
                Items = new List<ProductInventoryProductDto>(),
            };
        }


        var trees = await _treeRepository.GetChildren(Guid.Parse(key));

        var query = await _productRepository.FindProductsByTreeIdsAndNameCode(trees.Select(x => x.Id),
            searchValue, searchCode, status);

        var totalCount = await AsyncExecuter.CountAsync(query);

        query = _productRepository.SortAndPageProducts(query, sorting, skipCount, maxResultCount);

        var entities = await AsyncExecuter.ToListAsync(query);

        var resultsDtos = ObjectMapper.Map<List<ProductInventProduct>, List<ProductInventoryProductDto>>(entities);

        return new PagedResultDto<ProductInventoryProductDto>(
            totalCount,
            resultsDtos
        );
    }

    public async Task<ProductInventoryFullDto> GetProductDetails(Guid id)
    {
        var root = new ProductInventoryFullDto()
        {
            Tag = ProductInventroyTag.Product,
        };
        var product = await _productRepository.FindById(id, true);
        root.Data = ObjectMapper.Map<ProductInventProduct, ProductInventoryEditDto>(product);
        foreach (var module in product.Modules)
        {
            var dto = ObjectMapper.Map<ProductInventModule, ProductInventoryEditDto>(module);
            var fullModule = root.AddModule(dto);
            foreach (var materialDto in module.Materials.Select(material =>
                         ObjectMapper.Map<ProductInventMaterial, ProductInventoryEditDto>(material)))
            {
                materialDto.InModuleMaterial = true;
                fullModule.AddMaterial(materialDto);
            }

            fullModule.Children = fullModule.Children.OrderBy(x =>
            {
                if (!string.IsNullOrEmpty(x.Data.Level))
                {
                    if (x.Data.Level.Contains('.'))
                    {
                        return int.Parse(x.Data.Level.Split('.')[1]);
                    }

                    double.TryParse(x.Data.Level, out double d);
                    return d;
                }

                return 1e3;
            }).ToList();
        }

        foreach (var dto in product.Materials.Select(material =>
                     ObjectMapper.Map<ProductInventMaterial, ProductInventoryEditDto>(material)))
        {
            root.AddMaterial(dto);
        }

        root.Children = root.Children.OrderBy(x =>
        {
            if (!string.IsNullOrEmpty(x.Data.Level))
            {
                if (x.Data.Level.Contains('.'))
                {
                    return int.Parse(x.Data.Level.Split('.')[1]);
                }

                double.TryParse(x.Data.Level, out double d);
                return d;
            }

            return 1e3;
        }).ToList();
        return root;
    }

    #endregion
}