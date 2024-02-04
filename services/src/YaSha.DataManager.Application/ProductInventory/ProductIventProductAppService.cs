using System.Text;
using Newtonsoft.Json;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Users;
using YaSha.DataManager.ProductInventory.Dto;
using YaSha.DataManager.ProductInventory.Manager;

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

    public async Task<List<ProductInventoryProductDto>> ImportProductFromExcel(string system, string series, string seriesRemark,
        List<ProductInventoryProductCreateDto> dtos)
    {
        Check.NotNullOrEmpty(system, "system");
        Check.NotNullOrEmpty(system, "series");
        return await _productService.ImportFromExcel(_currentUser.UserName, system, series, seriesRemark, dtos);
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
        return await _productService.UpdateProduct(_currentUser.UserName, dto);
    }

    public async Task<ImageResponseDto> UploadProductImage(ImageFileDto dto)
    {
        var path = "/ServerData/FileManagement/ProductList/Image";
        var imageServerPath = "";
        try
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var name = Guid.NewGuid() + Path.GetExtension(dto.File.FileName);
            var imageLocalPath = path + "\\" + name;
            using (FileStream fileStream = new FileStream(imageLocalPath, FileMode.Create, FileAccess.Write))
            {
                await dto.File.CopyToAsync(fileStream);
            }

            imageServerPath = "https://bds.chinayasha.com/bdsfileservice/ProductList/Image/" + name;

            await _productService.UpdateProductImage(_currentUser.UserName, dto.Id, imageServerPath);
        }
        catch (Exception e)
        {
            return new ImageResponseDto()
            {
                Code = -1,
                Error = e.Message,
            };
        }

        return new ImageResponseDto()
        {
            Code = 1,
            ServerPath = imageServerPath,
        };
    }

    public async Task<List<ProductInventoryProductDto>> PublishProducts(Dictionary<Guid, ProductInventoryPublishStatus> dto)
    {
        if (dto == null) throw new Exception("参数为null");
        return await _productService.PublishProducts(dto);
    }

    public async Task<PagedResultDto<ProductInventoryProductDto>> PageProducts(OrderNotificationSearchDto input)
    {
        return await _productService.FindProductBySearchDto(
            input.Key,
            input.SearchValue,
            input.SearchCode,
            input.Status,
            input.Sorting,
            input.SkipCount,
            input.MaxResultCount);
    }

    public async Task<ProductInventoryFullDto> GetProductDetails(Guid id)
    {
        return await _productService.GetProductDetails(id);
    }

    public async Task<string> GetErpTokenAsync()
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

    public async Task<string> GetErpTokenAsync(string clientId, string clientSecret, bool isTest = false)
    {
        string url = isTest ? "http://ysapitest.chinayasha.com:65501/imtech-oauth-server/publicapi/getAccessToken" : "https://ysapi.chinayasha.com/imtech-oauth-server/publicapi/getAccessToken";


        var parameters = new Dictionary<string, string>
        {
                        { "grant_type", "client_credentials"},
                        { "client_id", clientId},
                        { "client_secret", clientSecret},
                    };
        var postData = JsonConvert.SerializeObject(parameters);
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("charset", "utf-8");
        HttpContent content = new StringContent(postData, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(url, content);
        var responseText = await response.Content.ReadAsStringAsync();
        dynamic jsonResponse = JsonConvert.DeserializeObject(responseText);
        string accessToken = jsonResponse.data.access_token;
        return accessToken;
    }

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
            // if ((string)item["status"] != "1") continue;
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

            obj.Series = !string.IsNullOrEmpty(series)
                ? ProductInventoryConsts.ProductInventorySeriesDictionary.TryGetValue(series, out var value)
                    ? value
                    : series
                : "";
            var system = (string)item["classificationName"];
            obj.System = !string.IsNullOrEmpty(system)
                ? system
                : ProductInventoryConsts.ProductInventorySystemDictionary
                    .FirstOrDefault(x => obj.Code.Contains(x.Key)).Value ?? "";

            var material = (string)item["basisMaterial"];
            obj.MaterialQuality = !string.IsNullOrEmpty(material)
                ? material
                : (!string.IsNullOrEmpty((string)item["materialOne"]) ? (string)item["materialOne"] : "") +
                  (!string.IsNullOrEmpty((string)item["materialTwo"]) ? (string)item["materialTwo"] : "") +
                  (!string.IsNullOrEmpty((string)item["materialThree"]) ? (string)item["materialThree"] : "");

            var unit = (string)item["unitPurchase1"];
            obj.Unit = !string.IsNullOrEmpty(unit) ? unit : "";

            var unitBase = (string)item["unitBase"];
            obj.UnitBase = !string.IsNullOrEmpty(unitBase) ? unitBase : "";

            results.Add(obj);
        }

        return results;
    }
}