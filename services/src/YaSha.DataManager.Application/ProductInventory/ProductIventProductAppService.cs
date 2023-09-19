using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Guids;
using Volo.Abp.Identity.Settings;
using Volo.Abp.Users;
using YaSha.DataManager.Common;
using YaSha.DataManager.ProductInventory.Dto;
using YaSha.DataManager.ProductInventory.Manager;
using YaSha.DataManager.ProductRetrieval.Dto;

namespace YaSha.DataManager.ProductInventory;

public class ProductIventProductAppService : DataManagerAppService, IProductIventProductAppService, ITransientDependency
{
    private readonly ProductInventProductManager _productService;

    private readonly ICurrentUser _currentUser;

    public ProductIventProductAppService(ICurrentUser currentUser, ProductInventProductManager productService)
    {
        _currentUser = currentUser;
        _productService = productService;
    }

    public async Task<ProductInventoryProductDto> Insert(Guid id, ProductInventoryProductCreateDto dto)
    {
        return await _productService.Insert(_currentUser.UserName, id, dto);
    }

    public async Task<List<ProductInventoryProductDto>> ImportProductFromExcel(string system, string series,
        List<ProductInventoryProductCreateDto> dtos)
    {
        Check.NotNullOrEmpty(system, "system");
        Check.NotNullOrEmpty(system, "series");
        return await _productService.ImportFromExcel(_currentUser.UserName, system, series, dtos);
    }

    public async Task<List<ProductInventoryProductDto>> AddProducts(List<ProductInventoryFullDto> dtos)
    {
        var results = new List<ProductInventoryProductDto>();

        foreach (var dto in dtos)
        {
            results.Add(await _productService.AddProduct(_currentUser.UserName, dto));
        }

        return results;
    }
    
    public async Task<List<ProductInventoryProductDto>> CopyProducts(List<Guid> ids)
    {
        return await _productService.CopyProduct(_currentUser.UserName, ids);
    }
    
    public async Task<string> DeleteProducts(List<Guid> ids)
    {
        var result = "success";
        try
        {
            foreach (var id in ids)
            {
                await _productService.DeleteProduct(id);
            }
        }
        catch (Exception e)
        {
            result = "Failed" + e.Message;
        }
        return result;
    }

    public async Task<string> DeleteProductEdits(List<ProductInventoryFullDto> dtos)
    {
        var result = "success";
        try
        {
            await _productService.DeleteProductEdits(dtos);
        }
        catch (Exception e)
        {
            result = "Failed" + e.Message;
        }
        return result; 
    }

    public async Task<ProductInventoryProductDto> UpdateProduct(ProductInventoryFullDto dto)
    {
        return await _productService.UpdateProduct(_currentUser.UserName,dto);
    }

    public async Task<List<ProductInventoryProductDto>> PublishProducts(Dictionary<Guid, ProductInventoryPublishStatus> dto)
    {
        if (dto == null) throw new Exception("参数为null");
        return await _productService.PublishProducts(dto);
    }

    public async Task<PagedResultDto<ProductInventoryProductDto>> PageProducts(OrderNotificationSearchDto input)
    {
        if (input.Key == null) return null;
        return await _productService.FindProductBySearchDto(input);
    }

    public async Task<ProductInventoryFullDto> GetProductDetails(Guid id)
    {
        return await _productService.GetProductDetails(id);
    }
}