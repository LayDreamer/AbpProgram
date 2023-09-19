using System.Linq.Dynamic.Core;
using Volo.Abp.Application.Dtos;
using YaSha.DataManager.ProductInventory;
using YaSha.DataManager.ProductInventory.AggregateRoot;
using YaSha.DataManager.ProductInventory.Dto;
using YaSha.DataManager.ProductRetrieval.Dto;
using YaSha.DataManager.Repository.ProductInventory;

namespace YaSha.DataManager.ProductRetrieval.Manager;

public class ProductRetrievalManager : DataManagerDomainService
{
    private readonly IProductInvProductRepository _productRepository;
    private readonly IProductInvModuleRepository _moduleRepository;
    private readonly IProductInvMaterialRepository _materialRepository;

    public ProductRetrievalManager(
        IProductInvProductRepository productRepository,
        IProductInvModuleRepository moduleRepository,
        IProductInvMaterialRepository materialRepository)
    {
        _productRepository = productRepository;
        _moduleRepository = moduleRepository;
        _materialRepository = materialRepository;
    }

    public async Task<PagedResultDto<ProductRetrievalDto>> PageProductRetrieval(
        ProductIndexSearchDto input)
    {
        var totalDtos = new List<ProductRetrievalDto>();
        var totalCount = 0;
        var products = await _productRepository.FindProductIndex(input.System, input.Series,
            input.SearchValue, input.SearchCode, input.Sorting);

        var modules = await _moduleRepository.FindModuleIndex(input.System, input.Series,
            input.SearchValue, input.SearchCode, input.Sorting);

        var materials = await _materialRepository.FindMaterialIndex(input.System, input.Series,
            input.SearchValue, input.SearchCode, input.Sorting);

        var rp = ObjectMapper.Map<List<ProductInventProduct>, List<ProductRetrievalDto>>(products);
        rp.ForEach(x => x.Tag = ProductInventroyTag.Product);

        var rm = ObjectMapper.Map<List<ProductInventModule>, List<ProductRetrievalDto>>(modules);
        rm.ForEach(x => x.Tag = ProductInventroyTag.Modules);

        var rmat = ObjectMapper.Map<List<ProductInventMaterial>, List<ProductRetrievalDto>>(materials);
        rmat.ForEach(x => x.Tag = ProductInventroyTag.Material);

        totalDtos.AddRange(rp);
        totalDtos.AddRange(rm);
        totalDtos.AddRange(rmat);

        totalCount = totalDtos.Count();
        var resultsDtos = totalDtos.Skip((input.SkipCount - 1) * input.MaxResultCount).Take(input.MaxResultCount)
            .ToList();

        return new PagedResultDto<ProductRetrievalDto>(
            totalCount,
            resultsDtos
        );
    }


    public async Task<PagedResultDto<ProductRetrievalResultDto>> FindProductRetrievalByInputs(ProductRetrievalSearchDto input)
    {
        var totalDtos = new List<ProductRetrievalResultDto>();
        
        foreach (var key in input.SearchInfo)
        {
            if (key.Value == ProductInventroyTag.Product)
            {
                var product = await _productRepository.FindById(key.Key, include: true);
                totalDtos.AddRange(GetProductResult(product));
            }
            else if (key.Value == ProductInventroyTag.Modules)
            {
                var module = await _moduleRepository.FindAsync(key.Key, includeDetails: true);
                var product = await _productRepository.FindById(module.ParentId);
                totalDtos.AddRange(GetModuleResult(product, module));
            }
            else if (key.Value == ProductInventroyTag.Material)
            {
                var material = await _materialRepository.FindAsync(key.Key);
                if (material.ModuleId == null && material.ProductId != null)
                {
                    var product = await _productRepository.FindById(material.ProductId.Value);
                    totalDtos.Add(new ProductRetrievalResultDto(
                        product.Name,
                        product.Code,
                        null,
                        null,
                        material.Name,
                        material.Code)
                    { Tag = ProductInventroyTag.Material});
                }

                if (material.ModuleId != null && material.ProductId == null)
                {
                    var module = await _moduleRepository.FindAsync(material.ModuleId.Value, false);
                    if (module != null)
                    {
                        var product = await _productRepository.FindById(module.ParentId); 
                        totalDtos.Add(new ProductRetrievalResultDto(
                            product.Name,
                            product.Code,
                            module.Name,
                            module.Code,
                            material.Name,
                            material.Code)
                        { Tag = ProductInventroyTag.Material});
                    }
                }
            }
        }

        if (!string.IsNullOrEmpty(input.Sorting))
        {
            totalDtos = totalDtos.AsQueryable().OrderBy(input.Sorting).ToList();
        }
        
        var totalCount = totalDtos.Count;
        
        var resultDtos = totalDtos.Skip((input.SkipCount - 1) * input.MaxResultCount).Take(input.MaxResultCount)
            .ToList();

        return new PagedResultDto<ProductRetrievalResultDto>(totalCount, resultDtos);
    }

    private IEnumerable<ProductRetrievalResultDto> GetProductResult(ProductInventProduct product)
    {
        var results = new List<ProductRetrievalResultDto>();
        foreach (var module in product.Modules)
        {
            results.AddRange(module.Materials.Select(material =>
                new ProductRetrievalResultDto(
                    product.Name,
                    product.Code,
                    module.Name,
                    module.Code,
                    material.Name,
                    material.Code)
                { Tag = ProductInventroyTag.Product}));
        }

        results.AddRange(product.Materials.Select(material =>
            new ProductRetrievalResultDto(
                product.Name,
                product.Code,
                null,
                null,
                material.Name,
                material.Code)
            { Tag = ProductInventroyTag.Product}));
        return results;
    }

    private IEnumerable<ProductRetrievalResultDto> GetModuleResult(ProductInventProduct product, ProductInventModule module)
    {
        return module.Materials.Select(material =>
            new ProductRetrievalResultDto(
                product.Name,
                product.Code,
                module.Name,
                module.Code,
                material.Name,
                material.Code)
            { Tag = ProductInventroyTag.Modules}).ToList();
    }
}