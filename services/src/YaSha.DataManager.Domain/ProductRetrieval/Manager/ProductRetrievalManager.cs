using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
//using Magicodes.ExporterAndImporter.Core;
//using Magicodes.ExporterAndImporter.Excel;
//using Magicodes.ExporterAndImporter.Excel.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Entities;
using YaSha.DataManager.ProductInventory;
using YaSha.DataManager.ProductInventory.AggregateRoot;
using YaSha.DataManager.ProductInventory.Dto;
using YaSha.DataManager.ProductInventory.Repository;
using YaSha.DataManager.ProductRetrieval.Dto;
using YaSha.DataManager.ProductRetrieval.Repository;

namespace YaSha.DataManager.ProductRetrieval.Manager;

public class ProductRetrievalManager : DataManagerDomainService
{
    private readonly IProductInvProductRepository _productRepository;
    private readonly IProductInvModuleRepository _moduleRepository;
    private readonly IProductInvMaterialRepository _materialRepository;
    private readonly IMaterialInventoryRepository _materialInventoryRepository;
    private readonly IDistributedCache<PagedResultDto<ProductRetrievalDto>> _cache;
    private readonly IDistributedCache<PagedResultDto<ProductRetrievalResultDto>> _cache1;

    public ProductRetrievalManager(
        IProductInvProductRepository productRepository,
        IProductInvModuleRepository moduleRepository,
        IProductInvMaterialRepository materialRepository,
        IDistributedCache<PagedResultDto<ProductRetrievalDto>> cache,
        IDistributedCache<PagedResultDto<ProductRetrievalResultDto>> cache1, IMaterialInventoryRepository materialInventoryRepository)
    {
        _productRepository = productRepository;
        _moduleRepository = moduleRepository;
        _materialRepository = materialRepository;
        _cache = cache;
        _cache1 = cache1;
        _materialInventoryRepository = materialInventoryRepository;
    }

    public async Task<PagedResultDto<ProductRetrievalDto>> PageProductRetrieval(
        ProductIndexSearchDto input)
    {
        var cacheKey = input.CalculateCacheKey();

        return await _cache.GetOrAddAsync(cacheKey, async () =>
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

            switch (input.Level)
            {
                case "产品":
                    totalDtos.AddRange(rp);
                    break;
                case "模块":
                    totalDtos.AddRange(rm);
                    break;
                case "物料":
                    totalDtos.AddRange(rmat);
                    break;
                default:
                    totalDtos.AddRange(rp);
                    totalDtos.AddRange(rm);
                    totalDtos.AddRange(rmat);
                    break;
            }

            var endResults = new List<ProductRetrievalDto>();
            var groups = totalDtos.GroupBy(x => x.Code);
            foreach (var group in groups)
            {
                foreach (var item in group)
                {
                    if (!string.IsNullOrEmpty(item.Code))
                        endResults.Add(item);
                    break;
                }
            }

            totalCount = endResults.Count();
            var resultsDtos = endResults.Skip((input.SkipCount - 1) * input.MaxResultCount).Take(input.MaxResultCount)
                .ToList();

            return new PagedResultDto<ProductRetrievalDto>(
                totalCount,
                resultsDtos
            );
        }, () => new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        });
    }

    public async Task<List<ProductRetrievalDto>> NoPageProductRetrieval(ProductIndexSearchDto input)
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

        switch (input.Level)
        {
            case "产品":
                totalDtos.AddRange(rp);
                break;
            case "模块":
                totalDtos.AddRange(rm);
                break;
            case "物料":
                totalDtos.AddRange(rmat);
                break;
            default:
                totalDtos.AddRange(rp);
                totalDtos.AddRange(rm);
                totalDtos.AddRange(rmat);
                break;
        }

        var endResults = new List<ProductRetrievalDto>();
        var groups = totalDtos.GroupBy(x => x.Code);
        foreach (var group in groups)
        {
            foreach (var item in group)
            {
                if (!string.IsNullOrEmpty(item.Code))
                    endResults.Add(item);
                break;
            }
        }

        return endResults;
    }


    public async Task<PagedResultDto<ProductRetrievalResultDto>> FindProductRetrievalByInputs(ProductRetrievalSearchDto input)
    {
        //var cacheKey = input.CalculateCacheKey();

        //return await _cache1.GetOrAddAsync(cacheKey, async () =>
        //{
        var totalDtos = new List<ProductRetrievalResultDto>();
        //var searchInfo = input.SearchInfo.Reverse();
        foreach (var key in input.SearchInfo)
        {
            if (key.Tag == ProductInventroyTag.Product)
            {
                var product = await _productRepository.FindByCode(key.Code, include: true);
                if (product == null) continue;
                totalDtos.AddRange(GetProductResult(product));
            }
            else if (key.Tag == ProductInventroyTag.Modules)
            {
                var modules = await _moduleRepository.FindByCode(key.Code, include: true);
                if (modules == null) continue;
                foreach (var module in modules)
                {
                    var product = await _productRepository.FindById(module.ParentId);
                    totalDtos.AddRange(GetModuleResult(product, module));
                }
            }
            else if (key.Tag == ProductInventroyTag.Material)
            {
                var materials = await _materialRepository.FindByCode(key.Code);
                if (materials == null) continue;
                foreach (var material in materials)
                {
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
                        { Tag = ProductInventroyTag.Material });
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
                            { Tag = ProductInventroyTag.Material });
                        }
                    }
                }
            }
        }

        if (!string.IsNullOrEmpty(input.Sorting))
        {
            totalDtos = totalDtos.AsQueryable().OrderBy(input.Sorting).ToList();
        }

        var groups = totalDtos.GroupBy(x => new { x.ProductCode, x.ModuleCode, x.MaterialCode });

        var endResults = new List<ProductRetrievalResultDto>();

        foreach (var group in groups)
        {
            foreach (var item in group)
            {
                endResults.Add(item);
                break;
            }
        }

        var totalCount = endResults.Count;

        var resultDtos = endResults.Skip((input.SkipCount - 1) * input.MaxResultCount).Take(input.MaxResultCount)
            .ToList();

        //foreach (var item in resultDtos)
        //{
        //    var materialInfo = await _materialInventoryRepository.FindByMaterialCode(new List<string>() { item.MaterialCode });
        //    if (materialInfo.Count > 0)
        //    {
        //        item.MaterialCount = materialInfo[0].InventoryQuantity.ToString(CultureInfo.InvariantCulture);
        //    }
        //}
        return new PagedResultDto<ProductRetrievalResultDto>(totalCount, resultDtos);
        //}, () => new DistributedCacheEntryOptions()
        //{
        //    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        //});
    }


    public async Task<List<ProductRetrievalResultDto>> FindAllProductRetrievalByInputs(ProductRetrievalSearchDto input)
    {
        var totalDtos = new List<ProductRetrievalResultDto>();

        foreach (var key in input.SearchInfo)
        {
            if (key.Tag == ProductInventroyTag.Product)
            {
                var product = await _productRepository.FindByCode(key.Code, include: true);
                if (product == null) continue;
                totalDtos.AddRange(GetProductResult(product));
            }
            else if (key.Tag == ProductInventroyTag.Modules)
            {
                var modules = await _moduleRepository.FindByCode(key.Code, include: true);
                if (modules == null) continue;
                foreach (var module in modules)
                {
                    var product = await _productRepository.FindById(module.ParentId);
                    totalDtos.AddRange(GetModuleResult(product, module));
                }
            }
            else if (key.Tag == ProductInventroyTag.Material)
            {
                var materials = await _materialRepository.FindByCode(key.Code);
                if (materials == null) continue;
                foreach (var material in materials)
                {
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
                        { Tag = ProductInventroyTag.Material });
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
                            { Tag = ProductInventroyTag.Material });
                        }
                    }
                }
            }
        }

        if (!string.IsNullOrEmpty(input.Sorting))
        {
            totalDtos = totalDtos.AsQueryable().OrderBy(input.Sorting).ToList();
        }

        var groups = totalDtos.GroupBy(x => new { x.ProductCode, x.ModuleCode, x.MaterialCode });

        var endResults = new List<ProductRetrievalResultDto>();

        foreach (var group in groups)
        {
            foreach (var item in group)
            {
                endResults.Add(item);
                break;
            }
        }

        return endResults;
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
                { Tag = ProductInventroyTag.Product }));
        }

        results.AddRange(product.Materials.Select(material =>
            new ProductRetrievalResultDto(
                    product.Name,
                    product.Code,
                    null,
                    null,
                    material.Name,
                    material.Code)
            { Tag = ProductInventroyTag.Product }));
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
            { Tag = ProductInventroyTag.Modules }).ToList();
    }
}