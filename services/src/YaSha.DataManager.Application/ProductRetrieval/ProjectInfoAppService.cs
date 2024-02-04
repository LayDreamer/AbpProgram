using System.Diagnostics;
using System.Text;
using Castle.Components.DictionaryAdapter.Xml;
using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using OfficeOpenXml;
using TencentCloud.Bi.V20220105.Models;
using TencentCloud.Iot.V20180123.Models;
using TencentCloud.Rum.V20210622.Models;
using TencentCloud.Scf.V20180416.Models;
using Volo.Abp.Caching;
using YaSha.DataManager.MaterailManage;
using YaSha.DataManager.MaterialManage;
using YaSha.DataManager.MaterialManage.Dto;
using YaSha.DataManager.ProductInventory;
using YaSha.DataManager.ProductRetrieval.AggregateRoot;
using YaSha.DataManager.ProductRetrieval.Dto;
using YaSha.DataManager.ProductRetrieval.Manager;
using YaSha.DataManager.StandardAndPolicy.Dto;
using static IdentityModel.OidcConstants;
using static YaSha.DataManager.Permissions.DataManagerPermissions;

namespace YaSha.DataManager.ProductRetrieval;

public class ProjectInfoAppService : DataManagerAppService, IProjectInfoAppService, ITransientDependency
{
    private readonly ProjectInfoManager _service;
    private readonly ProductIventProductAppService _appService;
    private readonly IDistributedCache<ApiResultDto> _cache;


    const string client_Id = "17ec5b3e-fb90-11ed-8b51-99e265121213";
    const string client_secret = "17ec5b3e-fb90-11ed-8b51-99e265121213";

    public ProjectInfoAppService(ProjectInfoManager service, IDistributedCache<ApiResultDto> cache, ProductIventProductAppService appService)
    {
        _service = service;
        _cache = cache;
        _appService = appService;
    }

    List<ProjectInfoCreateDto> GetExcelInfo(string path)
    {
        var result = new List<ProjectInfoCreateDto>();
        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
        var package = new ExcelPackage(new FileInfo(path));
        var worksheet = package.Workbook.Worksheets[0];
        int rows = worksheet.Dimension.End.Row;
        int cols = worksheet.Dimension.End.Column;
        var colNames = new List<string>() { "项目名称", "项目编码", "产品编码", "模块编码", "物料编码" };
        Dictionary<string, int> map = new Dictionary<string, int>();
        for (int cell = 1; cell < cols; cell++)
        {
            var text = worksheet.Cells[1, cell].Text;
            if (!colNames.Contains(text)) continue;
            map.TryAdd(text, cell);
        }

        for (var row = 2; row < rows; row++)
        {
            var index = worksheet.Cells[row, 1].Text;
            if (string.IsNullOrEmpty(index)) break;
            var ent = new ProjectInfoCreateDto()
            {
                ProjectName = worksheet.Cells[row, map["项目名称"]].Text,
                ProjectCode = worksheet.Cells[row, map["项目编码"]].Text.Trim(),
                ProductCode = worksheet.Cells[row, map["产品编码"]].Text.Trim(),
                ModuleCode = worksheet.Cells[row, map["模块编码"]].Text.Trim(),
                MaterialCode = worksheet.Cells[row, map["物料编码"]].Text.Trim(),
            };
            result.Add(ent);
        }

        return result;
    }

    public async Task<ApiResultDto> ImportFromExcel(IFormFile file)
    {
        try
        {
            var localSavePath = "/ServerData/FileManagement/Tmp";
            if (!Directory.Exists(localSavePath))
            {
                Directory.CreateDirectory(localSavePath);
            }

            var name = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(file.FileName);
            localSavePath = localSavePath + "/" + name;
            using (FileStream fileStream = new FileStream(localSavePath, FileMode.Create, FileAccess.Write))
            {
                await file.CopyToAsync(fileStream);
            }

            var create = GetExcelInfo(localSavePath);
            await _service.Insert(create);
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

    public async Task<List<ProjectInfoCodeResultDto>> Parsing(List<ProjectInfoSearchCodeDto> input)
    {
        var resultDtos = new List<ProjectInfoCodeResultDto>();

        foreach (var item in input)
        {
            if (string.IsNullOrEmpty(item.Code)) continue;
            var dto = await _service.FindByCodeAndType(item.Code, item.Type);
            resultDtos.Add(new ProjectInfoCodeResultDto()
            {
                Code = item.Code,
                Projects = dto.DistinctBy(x => x.ProjectCode).ToList(),
            });
        }

        return resultDtos;
    }

    public async Task<ApiResultDto> GetByErp(List<ProjectInfoSearchCodeDto> input)
    {
        //var cacheKey = input.FirstOrDefault().CalculateCacheKey();
        //return await _cache.GetOrAddAsync(cacheKey, async () =>
        //{
        ApiResultDto resultDto = new();
        var map = new List<ProjectInfoCodeResultDto>();
        try
        {
            var token = await _appService.GetErpTokenAsync(client_Id, client_secret);
            //测试                
            //const string baseUrl = "http://erpdev.chinayasha.com:8040/yasha-erp-web/sysopenapi/GyhApiController/getDeepDesignInfo";

            //正式
            const string baseUrl = "https://erp.chinayasha.com/yasha-erp-web/sysopenapi/GyhApiController/getDeepDesignInfo";

            var url = $"{baseUrl}?access_token={token}&client_id={client_Id}";
            var httpClient = new HttpClient();


            foreach (var item in input)
            {
                string paramNo = "";
                List<ProjectInfoDto> projectList = new();
                switch (item.Type)
                {
                    case ProjectInfoInputType.Producut:
                        paramNo = "productNoList";
                        break;
                    case ProjectInfoInputType.Module:
                        paramNo = "moduleNoList";
                        break;
                    case ProjectInfoInputType.Material:
                        paramNo = "materialNoList";
                        break;
                    case ProjectInfoInputType.Undefined:
                        break;
                    default:
                        break;
                }
                var parameters = new Dictionary<string, string>
                    {
                        { paramNo, item.Code},
                    };
                var postData = JsonConvert.SerializeObject(parameters);
                HttpContent content = new StringContent(postData, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(url, content);
                var responseText = await response.Content.ReadAsStringAsync();
                dynamic jsonResponse = JsonConvert.DeserializeObject(responseText);
                foreach (var data in jsonResponse.data)
                {
                    var projectName = (string)data["projectName"];
                    if (string.IsNullOrEmpty(projectName)) continue;
                    var projectCode = (string)data["projectNo"];
                    if (string.IsNullOrEmpty(projectCode)) continue;
                    projectList.Add(new ProjectInfoDto
                    {
                        ProjectName = projectName,
                        ProjectCode = projectCode
                    });
                }
                map.Add(new ProjectInfoCodeResultDto()
                {
                    Code = item.Code,
                    Projects = projectList.DistinctBy(x => x.ProjectCode).ToList(),
                });

                resultDto.Success = true;
                resultDto.Data = map;
            }

            //var parameters = new Dictionary<string, List<string>>();
            //foreach (var item in input)
            //{
            //    string paramNo = "";
            //    List<ProjectInfoDto> projectList = new();
            //    switch (item.Type)
            //    {
            //        case ProjectInfoInputType.Producut:
            //            paramNo = "productNoList";
            //            break;
            //        case ProjectInfoInputType.Module:
            //            paramNo = "moduleNoList";
            //            break;
            //        case ProjectInfoInputType.Material:
            //            paramNo = "materialNoList";
            //            break;
            //        case ProjectInfoInputType.Undefined:
            //            break;
            //        default:
            //            break;
            //    }
            //    if (parameters.ContainsKey(paramNo))
            //    {
            //        parameters[paramNo].Add(item.Code);
            //    }
            //    else
            //    {
            //        parameters.Add(paramNo, new List<string> { item.Code });
            //    }
            //}

            //var postData = JsonConvert.SerializeObject(parameters);
            //HttpContent content = new StringContent(postData, Encoding.UTF8, "application/json");
            //var response = await httpClient.PostAsync(url, content);
            //var responseText = await response.Content.ReadAsStringAsync();
            //dynamic jsonResponse = JsonConvert.DeserializeObject(responseText);
            //foreach (var data in jsonResponse.data)
            //{
            //    var projectName = (string)data["projectName"];
            //    if (string.IsNullOrEmpty(projectName)) continue;
            //    var projectCode = (string)data["projectNo"];
            //    if (string.IsNullOrEmpty(projectCode)) continue;
            //    projectList.Add(new ProjectInfoDto
            //    {
            //        ProjectName = projectName,
            //        ProjectCode = projectCode
            //    });
            //}
            //map.Add(new ProjectInfoCodeResultDto()
            //{
            //    Code = item.Code,
            //    Projects = projectList.DistinctBy(x => x.ProjectCode).ToList(),
            //});

            //resultDto.Success = true;
            //resultDto.Data = map;
        }
        catch (Exception e)
        {
            resultDto.Success = false;
            resultDto.Error = e.Message;
        }
        return resultDto;

        //}, () => new DistributedCacheEntryOptions()
        //{
        //    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2)
        //});
    }
}