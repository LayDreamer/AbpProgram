using System.Text;
using Newtonsoft.Json;
using Volo.Abp.Application.Dtos;
using YaSha.DataManager.ProductInventory;
using YaSha.DataManager.ProductInventory.Dto;

namespace YaSha.DataManager.Controllers;

/// <summary>
/// 架构清单
/// </summary>
[Route("ProductInventoryProduct")]
public class ProductInventoryProductController : DataManagerController, IProductIventProductAppService
{
    private readonly IProductIventProductAppService _service;


    public ProductInventoryProductController(IProductIventProductAppService service)
    {
        _service = service;
    }

    [HttpPost("Insert")]
    public async Task<ProductInventoryProductDto> Insert(Guid id, ProductInventoryProductCreateDto dto)
    {
        return await _service.Insert(id, dto);
    }

    [HttpPost("ImportFromExcel")]
    public async Task<List<ProductInventoryProductDto>> ImportProductFromExcel(string system, string series, string seriesRemark,
        List<ProductInventoryProductCreateDto> dtos)
    {
        return await _service.ImportProductFromExcel(system, series, seriesRemark, dtos);
    }

    [HttpPost("AddProduct")]
    public async Task<List<ProductInventoryProductDto>> AddProducts(List<ProductInventoryFullDto> dtos)
    {
        return await _service.AddProducts(dtos);
    }

    [HttpPost("CopyProduct")]
    public async Task<List<ProductInventoryProductDto>> CopyProducts(List<Guid> ids)
    {
        return await _service.CopyProducts(ids);
    }

    [HttpPost("DeleteProducts")]
    public async Task<string> DeleteProducts(List<Guid> ids)
    {
        return await _service.DeleteProducts(ids);
    }

    [HttpPost("DeleteEdits")]
    public async Task<string> DeleteProductEdits(List<ProductInventoryFullDto> dtos)
    {
        return await _service.DeleteProductEdits(dtos);
    }

    [HttpPost("UpdateProduct")]
    public async Task<ProductInventoryProductDto> UpdateProduct(ProductInventoryFullDto dto)
    {
        return await _service.UpdateProduct(dto);
    }

    [HttpPost("PublishProducts")]
    public async Task<List<ProductInventoryProductDto>> PublishProducts(Dictionary<Guid, ProductInventoryPublishStatus> dto)
    {
        return await _service.PublishProducts(dto);
    }


    [HttpPost("PageProducts")]
    public async Task<PagedResultDto<ProductInventoryProductDto>> PageProducts(OrderNotificationSearchDto input)
    {
        return await _service.PageProducts(input);
    }

    [HttpPost("GetProductDetails")]
    public async Task<ProductInventoryFullDto> GetProductDetails(Guid id)
    {
        return await _service.GetProductDetails(id);
    }

    [HttpPost("ErpGetToken")]
    public async Task<string> GetErpTokenAsync()
    {
        return await _service.GetErpTokenAsync();
    }

    [HttpPost("ErpGetProduct")]
    public async Task<List<ProductInventoryEditDto>> GetErpData(string accessToken, string postData)
    {
        return await _service.GetErpData(accessToken, postData);
    }

    [HttpPost("UploadProductImage")]
    public async Task<ImageResponseDto> UploadProductImage([FromForm] ImageFileDto dto)
    {
        return await _service.UploadProductImage(dto);
    }
}