using System.Net;
using System.Text;
using Newtonsoft.Json;
using Volo.Abp.Application.Dtos;
using YaSha.DataManager.Common;
using YaSha.DataManager.ProductInventory;
using YaSha.DataManager.ProductInventory.Dto;
using YaSha.DataManager.ProductRetrieval.Dto;

namespace YaSha.DataManager.Controllers;

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
    public async Task<List<ProductInventoryProductDto>> ImportProductFromExcel(string system, string series,
        List<ProductInventoryProductCreateDto> dtos)
    {
        return await _service.ImportProductFromExcel(system, series, dtos);
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
    public async Task<string> GetErpTkenAsync()
    {
        const string url = "https://ysapi.chinayasha.com/imtech-oauth-server/publicapi/getAccessToken";
        const string postData = @"{
            ""grant_type"": ""client_credentials"",
            ""client_id"": ""17ec5b3e-fb90-11ed-8b51-c8e265121212"",
            ""client_secret"": ""17ec5b3e-fb90-11ed-8b51-c8e265121212""
        }";
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("charset", "utf-8");
        HttpContent content = new StringContent(postData, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(url, content);
        var responseText = await response.Content.ReadAsStringAsync();
        dynamic jsonResponse = JsonConvert.DeserializeObject(responseText);
        string accessToken = jsonResponse.data.access_token;
        return accessToken;
    }

    [HttpPost("ErpGetProduct")]
    public async Task<List<ProductInventoryEditDto>> GetErpData(string accessToken, string postData)
    {
        const string url = "https://erp.chinayasha.com/yasha-erp-web/sysopenapi/bimApi";
        const string clientId = "17ec5b3e-fb90-11ed-8b51-c8e265121212";
        using var client = new HttpClient();
        var fullUrl = $"{url}?access_token={accessToken}&client_id={clientId}";
        HttpContent content = new StringContent(postData, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(fullUrl, content);
        var responseText = await response.Content.ReadAsStringAsync();
        dynamic jsonResponse = JsonConvert.DeserializeObject(responseText);
        var results = new List<ProductInventoryEditDto>();
        foreach (var item in jsonResponse.data)
        {
            if ((string)item["status"] != "1") continue;
            var obj = new ProductInventoryEditDto();
            var productCode = (string)item["productNo"];
            var materialCode = (string)item["materialNo"];
            obj.Code = !string.IsNullOrEmpty(productCode) ? productCode : !string.IsNullOrEmpty(materialCode) ? materialCode : "";
            var productName = (string)item["productName"];
            var materialName = (string)item["materialName"];
            obj.Name = !string.IsNullOrEmpty(productName) ? productName : !string.IsNullOrEmpty(materialName) ? materialName : "";
            var productLength = (string)item["length"];
            var materialLength = (string)item["longth"];
            obj.Length = !string.IsNullOrEmpty(productLength) ? productLength : !string.IsNullOrEmpty(materialLength) ? materialLength : "";
            var width = (string)item["width"];
            obj.Width = !string.IsNullOrEmpty(width) ? width : "";
            var productHeight = (string)item["high"];
            var materialHeight = (string)item["height"];
            obj.Height = !string.IsNullOrEmpty(productHeight) ? productHeight : !string.IsNullOrEmpty(materialHeight) ? materialHeight : "";
            var color = (string)item["color"];
            obj.Color = !string.IsNullOrEmpty(color) ? color : "";
            var series = (string)item["series"];
            
            obj.Series = !string.IsNullOrEmpty(series) ? 
                DataManagerDomainSharedConsts.ProductInventorySeriesDictionary.TryGetValue(series, out var value) 
                    ? value : series : "";
            var system = (string)item["classificationName"];
            obj.System = !string.IsNullOrEmpty(system)
                ? system
                : DataManagerDomainSharedConsts.ProductInventorySystemDictionary
                    .FirstOrDefault(x => obj.Code.Contains(x.Key)).Value ?? "";

            var material = (string)item["basisMaterial"];
            obj.MaterialQuality = !string.IsNullOrEmpty(material) ? material : 
                (!string.IsNullOrEmpty((string)item["materialOne"])?(string)item["materialOne"]:"")+
                 (!string.IsNullOrEmpty((string)item["materialTwo"])?(string)item["materialTwo"]:"")+ 
                  (!string.IsNullOrEmpty((string)item["materialThree"])?(string)item["materialThree"]:"");
            results.Add(obj);
            var unit = (string)item["unitPurchase1"];
            obj.Unit = !string.IsNullOrEmpty(unit) ? unit : "";
        }

        return results;
    }
}