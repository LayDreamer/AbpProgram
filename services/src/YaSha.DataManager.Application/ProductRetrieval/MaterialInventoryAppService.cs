using EasyAbp.Abp.Trees;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Repositories;
using YaSha.DataManager.FamilyLibs;
using YaSha.DataManager.FamilyTrees;
using YaSha.DataManager.MaterialManage;
using YaSha.DataManager.ProductInventory;
using YaSha.DataManager.ProductRetrieval.AggregateRoot;
using YaSha.DataManager.ProductRetrieval.Dto;
using YaSha.DataManager.ProductRetrieval.Manager;
using YaSha.DataManager.StandardAndPolicy.Dto;
using static FreeSql.Internal.GlobalFilter;

namespace YaSha.DataManager.ProductRetrieval;

public class MaterialInventoryAppService : DataManagerAppService, IMaterialInventoryAppService
{
    private readonly MaterialInventoryManager _service;
    private readonly ProductIventProductAppService _appService;
    private readonly IDistributedCache<List<MaterialInventoryDto>> _erpCache;


    const string client_Id = "17ec5b3e-fb90-11ed-8b51-99e265121213";
    const string client_secret = "17ec5b3e-fb90-11ed-8b51-99e265121213";

    List<string> _warehouses = new() { "原材料", "半成品" }; /*"原材料立库", "半成品立库" , */
    List<string> _locationNames = new() { "合格", /*"不良"*/ }; //增加需求：只过滤出合格仓内容

    public MaterialInventoryAppService(MaterialInventoryManager service, IDistributedCache<List<MaterialInventoryDto>> erpCache, ProductIventProductAppService appService)
    {
        _service = service;
        _erpCache = erpCache;
        _appService = appService;
    }

    public async Task<ApiResultDto> UploadDataAsync(List<MaterialInventoryCreateDto> input)
    {
        try
        {
            await _service.Insert(input);
            return new ApiResultDto()
            {
                Success = true,
            };
        }
        catch (Exception e)
        {
            return new ApiResultDto()
            {
                Success = false,
                Error = e.Message
            };
        }
    }

    public async Task<List<MaterialInventoryDto>> ParsingListDataAsync(List<string> code)
    {
        return await _service.Find(code);
    }

    //public async Task<List<MaterialInventoryDto>> GetByErpAsync(List<string> codes)
    //{
    //    await _appService.GetErpTokenAsync(client_Id, client_secret);
    //    var map = new List<MaterialInventoryDto>();
    //    try
    //    {
    //        var token = await _appService.GetErpTokenAsync(client_Id, client_secret);
    //        //测试                
    //        //const string baseUrl = "http://erpdev.chinayasha.com:8040/yasha-erp-web/sysopenapi/GyhApiController/getStocklnfo";

    //        //正式
    //        const string baseUrl = "https://erp.chinayasha.com/yasha-erp-web/sysopenapi/GyhApiController/getStockInfo";

    //        using var client = new HttpClient();
    //        var url = $"{baseUrl}?access_token={token}&client_id={client_Id}";
    //        var httpClient = new HttpClient();
    //        string[] codeArray = codes.ToArray();            
    //        var parameters = new Dictionary<string, string[]>
    //                {
    //                    { "materialNoList", codeArray},
    //                };
    //        var postData = JsonConvert.SerializeObject(parameters);
    //        HttpContent content = new StringContent(postData, Encoding.UTF8, "application/json");
    //        var response = await httpClient.PostAsync(url, content);
    //        var responseText = await response.Content.ReadAsStringAsync();
    //        dynamic jsonResponse = JsonConvert.DeserializeObject(responseText);
    //        foreach (var data in jsonResponse.data)
    //        {
    //            var locationName = (string)data["nameStorehouse"];
    //            var warehouse = (string)data["nameWarehouse"];
    //            var materialCode= (string)data["materialNo"];
    //            var unit = (string)data["unitQuantity"];
    //            var inventoryQuantity = (double)data["quantityStock"];
    //            var inventoryAmount = (double)data["moneyStock"];
    //            map.Add(new MaterialInventoryDto()
    //            {
    //                Warehouse = warehouse,
    //                WarehouseLocationName = locationName,
    //                MaterialCode= materialCode,
    //                Unit= unit,
    //                InventoryQuantity= inventoryQuantity,
    //                InventoryAmount= inventoryAmount
    //            });
    //        }

    //    }
    //    catch (Exception e)
    //    {

    //    }

    //    return map;

    //}

    public async Task<ApiResultDto> GetByErpAsync(List<string> codes)
    {
        var resultDto = new ApiResultDto();
        await GetMaterialInventoriesAsync(codes, resultDto);
        return resultDto;
    }


    public async Task<List<MaterialInventoryDto>> GetMaterialInventoriesAsync(List<string> codes, ApiResultDto resultDto)
    {
        List<MaterialInventoryDto> map = new();
        try
        {
            var token = await _appService.GetErpTokenAsync(client_Id, client_secret);
            //const string baseUrl = "http://erpdev.chinayasha.com:8040/yasha-erp-web/sysopenapi/GyhApiController/getStockInfo";
            //正式
            const string baseUrl = "https://erp.chinayasha.com/yasha-erp-web/sysopenapi/GyhApiController/getStockInfo";
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("charset", "utf-8");
            var url = $"{baseUrl}?access_token={token}&client_id={client_Id}";
            //string[] codeArray = codes.ToArray();
            var parameters = new Dictionary<string, List<string>> { { "materialNoList", codes }, };
            var postData = JsonConvert.SerializeObject(parameters);
            HttpContent content = new StringContent(postData, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                resultDto.Success = false;
                resultDto.Error = $"请求错误：{response.StatusCode}";
                return map;
            }

            var responseText = await response.Content.ReadAsStringAsync();
            dynamic jsonResponse = JsonConvert.DeserializeObject(responseText);
            foreach (var data in jsonResponse.data)
            {
                var locationName = (string)data["nameStorehouse"];
                if (_locationNames.All(e => !locationName.Contains(e)))
                    continue;
                var warehouse = (string)data["nameWarehouse"];
                if (_warehouses.All(e => !warehouse.Contains(e)))
                    continue;
                var materialCode = (string)data["materialNo"];
                var unit = (string)data["unitQuantity"];
                double inventoryQuantity = (double)(data["quantityStock"]);
                double inventoryAmount = (double)(data["moneyStock"]);

                map.Add(new MaterialInventoryDto()
                {
                    //Id=Guid.NewGuid(),
                    //CreationTime= DateTime.Now,
                    Warehouse = warehouse,
                    WarehouseLocationName = locationName,
                    MaterialCode = materialCode,
                    Unit = unit,
                    InventoryQuantity = inventoryQuantity,
                    InventoryAmount = inventoryAmount
                });
            }
            resultDto.Success = true;
            resultDto.Data = map;
        }
        catch (Exception e)
        {
            resultDto.Success = false;
            resultDto.Error = e.Message;
        }
        return map;
    }
}