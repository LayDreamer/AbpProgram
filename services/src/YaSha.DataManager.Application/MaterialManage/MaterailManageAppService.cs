using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using TencentCloud.Tiia.V20190529.Models;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Caching;
using Volo.Abp.Users;
using YaSha.DataManager.MaterailManage;
using YaSha.DataManager.MaterailManage.Dto;
using YaSha.DataManager.MaterialManage.Dto;
using YaSha.DataManager.MaterialManage.Manager;
using YaSha.DataManager.ProductInventory;
using YaSha.DataManager.StandardAndPolicy.Dto;

namespace YaSha.DataManager.MaterialManage;

public class MaterailManageAppService : DataManagerAppService, IMaterailManageAppService
{
    private readonly MaterialManageManager _service;
    private readonly ICurrentUser _currentUser;
    private readonly IProductIventProductAppService _appService;
    private readonly IBaiduApiAppService _apiAppService;
    private readonly IDistributedCache<List<MaterialManageErpCacheData>> _erpCache;
    private readonly IDistributedCache<Dictionary<Guid, float>> _tencentApiCache;

    public MaterailManageAppService(
        MaterialManageManager service,
        ICurrentUser currentUser,
        IDistributedCache<List<MaterialManageErpCacheData>> erpCache,
        IProductIventProductAppService appService,
        IBaiduApiAppService apiAppService,
        IDistributedCache<Dictionary<Guid, float>> tencentApiCache)
    {
        _service = service;
        _currentUser = currentUser;
        _erpCache = erpCache;
        _appService = appService;
        _apiAppService = apiAppService;
        _tencentApiCache = tencentApiCache;
        if (!Directory.Exists(MaterialManageConsts.MaterialImageTmpPath))
        {
            Directory.CreateDirectory(MaterialManageConsts.MaterialImageTmpPath);
        }

        if (!Directory.Exists(MaterialManageConsts.MaterialImageUserPath))
        {
            Directory.CreateDirectory(MaterialManageConsts.MaterialImageUserPath);
        }

        if (!Directory.Exists(MaterialManageConsts.MaterialImagePath))
        {
            Directory.CreateDirectory(MaterialManageConsts.MaterialImagePath);
        }

        if (!Directory.Exists(MaterialManageConsts.MaterialDownloadImagePath))
        {
            Directory.CreateDirectory(MaterialManageConsts.MaterialDownloadImagePath);
        }

        if (!Directory.Exists(MaterialManageConsts.MaterialSeriesImagePath))
        {
            Directory.CreateDirectory(MaterialManageConsts.MaterialSeriesImagePath);
        }
    }

    public async Task<ApiResultDto> UploadImage(IFormFile image)
    {
        if (_currentUser.Id == null)
        {
            throw new BusinessException("YaSha.DataManager:LoginOver");
        }

        try
        {
            var name = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(image.FileName);
            var localPath = MaterialManageConsts.MaterialImageTmpPath + "/" + name;
            using (FileStream fileStream = new FileStream(localPath, FileMode.Create, FileAccess.Write))
            {
                await image.CopyToAsync(fileStream);
            }

            var serverPath = "https://bds.chinayasha.com/bdsfileservice/MaterialManage/Tmp/" + name;
            return new ApiResultDto()
            {
                Success = true,
                Data = serverPath,
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

    public async Task<ApiResultDto> InsertMaterialImages(List<IFormFile> images, List<IFormFile> downloads)
    {
        if (_currentUser.Id == null)
        {
            throw new BusinessException("YaSha.DataManager:LoginOver");
        }

        if (images.Count != downloads.Count)
        {
            return new ApiResultDto()
            {
                Success = false,
                Error = "传入参数有误",
            };
        }

        var errorMsg = string.Empty;
        for (int i = 0; i < images.Count; i++)
        {
            try
            {
                var fileName = Path.GetFileNameWithoutExtension(images[i].FileName);
                fileName = fileName.Replace("／", "/");
                var dto = await _service.FindByMaterialName(fileName);
                if (dto == null || dto.Count == 0) continue;
                var imageName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(images[i].FileName);
                var downloadName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(downloads[i].FileName);
                var targetImagePath = MaterialManageConsts.MaterialImagePath + "/" + imageName;
                var targetDownloadImagePath = MaterialManageConsts.MaterialDownloadImagePath + "/" + downloadName;
                using (FileStream fileStream = new FileStream(targetImagePath, FileMode.Create, FileAccess.Write))
                {
                    await images[i].CopyToAsync(fileStream);
                }

                using (FileStream fileStream = new FileStream(targetDownloadImagePath, FileMode.Create, FileAccess.Write))
                {
                    await downloads[i].CopyToAsync(fileStream);
                }

                foreach (var item in dto)
                {
                    if (!string.IsNullOrEmpty(item.MaterialImageUrl))
                    {
                        var path = MaterialManageConsts.MaterialImagePath + "/" + Path.GetFileName(item.MaterialImageUrl);
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }

                        await _apiAppService.DeleteImage(item.BaiduSign);
                        var apiResultDto = await _apiAppService.CreatedImage(targetImagePath, item.Id.ToString());
                        if (apiResultDto.Success)
                        {
                            item.BaiduSign = (string)apiResultDto.Data;
                        }
                    }
                    else
                    {
                        var apiResultDto = await _apiAppService.CreatedImage(targetImagePath, item.Id.ToString());
                        if (apiResultDto.Success)
                        {
                            item.BaiduSign = (string)apiResultDto.Data;
                        }
                    }

                    item.MaterialImageUrl = "https://bds.chinayasha.com/bdsfileservice/MaterialManage/Image/" + imageName;
                    //删除旧的下载数据
                    if (!string.IsNullOrEmpty(item.MaterialImageDownLoadUrl))
                    {
                        var path = MaterialManageConsts.MaterialDownloadImagePath + "/" + Path.GetFileName(item.MaterialImageDownLoadUrl);
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                    }

                    //重新赋值
                    item.MaterialImageDownLoadUrl = "https://bds.chinayasha.com/bdsfileservice/MaterialManage/DownloadImage/" + downloadName;
                    await _service.UpdateMaterialImage(item);
                }
            }
            catch (Exception ex)
            {
                errorMsg += ex.Message;
            }
        }

        if (!string.IsNullOrEmpty(errorMsg))
        {
            return new ApiResultDto
            {
                Success = false,
                Error = errorMsg,
            };
        }

        return new ApiResultDto
        {
            Success = true,
        };
    }

    public async Task<ApiResultDto> InsertSeriesImages(List<IFormFile> images)
    {
        if (_currentUser.Id == null)
        {
            throw new BusinessException("YaSha.DataManager:LoginOver");
        }

        var errorMsg = string.Empty;
        foreach (var item in images)
        {
            try
            {
                var fileName = Path.GetFileNameWithoutExtension(item.FileName);
                fileName = fileName.Replace("／", "/");
                var dtos = await _service.FindBySeriesName(fileName);
                if (dtos == null || dtos.Count == 0) continue;
                var saveName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(item.FileName);
                var savePath = MaterialManageConsts.MaterialSeriesImagePath + "/" + saveName;
                using (FileStream fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write))
                {
                    await item.CopyToAsync(fileStream);
                }

                foreach (var dto in dtos)
                {
                    if (!string.IsNullOrEmpty(dto.SeriesImageUrl))
                    {
                        var path = MaterialManageConsts.MaterialSeriesImagePath + "/" + Path.GetFileName(dto.SeriesImageUrl);
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                    }

                    dto.SeriesImageUrl = "https://bds.chinayasha.com/bdsfileservice/MaterialManage/SeriesImage/" + saveName;
                    await _service.UpdateSeriesImage(dto);
                }
            }
            catch (Exception ex)
            {
                errorMsg += ex.Message;
            }
        }

        if (!string.IsNullOrEmpty(errorMsg))
        {
            return new ApiResultDto
            {
                Success = false,
                Error = errorMsg,
            };
        }

        return new ApiResultDto
        {
            Success = true,
        };
    }

    public async Task<MaterialManageManageExtraInfo> GetManageExtra()
    {
        return await _service.GetManageExtra();
    }

    public async Task<PagedResultDto<MaterialManageFullDto>> PageManage(MaterialManageSearchDto search)
    {
        if (_currentUser.Id == null)
        {
            throw new BusinessException("YaSha.DataManager:LoginOver");
        }

        var map = await GetYaShaErpCodeMap();
        return await _service.PageManage(
            map,
            search.Search,
            search.Status,
            search.MaterialType,
            search.Supplier,
            search.SupplierCode,
            search.SeriesName,
            search.Sorting,
            search.SkipCount,
            search.MaxResultCount);
    }

    public async Task<ApiResultDto> GetImageApiMap(IFormFile file, string key)
    {
        if (file == null)
        {
            return new ApiResultDto()
            {
                Success = false,
                Error = "参数file为空",
            };
        }

        try
        {
            var name = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(file.FileName);
            var localPath = MaterialManageConsts.MaterialImageUserPath + "/" + name;
            using (FileStream fileStream = new FileStream(localPath, FileMode.Create, FileAccess.Write))
            {
                await file.CopyToAsync(fileStream);
            }

            //识别结果加入redis
            await _tencentApiCache.GetOrAddAsync(key, async () =>
                {
                    var apiDto = await _apiAppService.SearchImage(localPath);
                    var map = new Dictionary<Guid, float>();
                    if (apiDto.Success)
                    {
                        var infos = (List<MaterialBaiduSearchDto>)apiDto.Data;
                        if (infos != null)
                        {
                            foreach (var item in infos)
                            {
                                map.Add(Guid.Parse(item.Brief), item.Score);
                            }
                        }
                    }

                    return map;
                }, () => new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) }
            );
            return new ApiResultDto()
            {
                Success = true,
                Data = "https://bds.chinayasha.com/bdsfileservice/MaterialManage/User/" + name,
            };
        }
        catch (Exception e)
        {
            return new ApiResultDto()
            {
                Success = false,
                Error = e.Message,
            };
        }
    }

    public async Task<MaterialManageHomeDto> PageHome(MaterialManageHomeSearchDto search)
    {
        var map = await GetYaShaErpCodeMap();
        var apiMap = new Dictionary<Guid, float>();
        if (!string.IsNullOrEmpty(search.CameraSearchKey))
        {
            apiMap = await _tencentApiCache.GetAsync(search.CameraSearchKey);
        }

        return await _service.PageHome(
            map,
            apiMap,
            search.MaterialType,
            search.MaterialTexture,
            search.MaterialSurface,
            search.Search,
            search.Sorting,
            search.SkipCount,
            search.MaxResultCount);
    }

    public async Task<List<MaterialManageFullDto>> GetDetail(List<Guid> id)
    {
        var map = await GetYaShaErpCodeMap();
        return await _service.GetDetail(id, map);
    }

    private async Task<List<MaterialManageErpCacheData>> GetYaShaErpCodeMap()
    {
        return await _erpCache.GetOrAddAsync(MaterialManageConsts.YaShaErpCacheKey, async () =>
        {
            var map = new List<MaterialManageErpCacheData>();
            try
            {
                var token = await _appService.GetErpTokenAsync();
                const string url = "https://erp.chinayasha.com/yasha-erp-web/sysopenapi/bimApi";
                const string clientId = "17ec5b3e-fb90-11ed-8b51-c8e265121212";
                using var client = new HttpClient();
                var fullUrl = $"{url}?access_token={token}&client_id={clientId}";
                string jsonData = "{\"data\":[{\"name\":\"膜\"}]}";
                HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(fullUrl, content);
                var responseText = await response.Content.ReadAsStringAsync();
                dynamic jsonResponse = JsonConvert.DeserializeObject(responseText);

                foreach (var item in jsonResponse.data)
                {
                    if ((string)item["status"] != "1") continue;
                    var sCode = (string)item["colorNo"];
                    if (string.IsNullOrEmpty(sCode)) continue;
                    var mCode = (string)item["materialNo"];
                    if (string.IsNullOrEmpty(mCode)) continue;
                    var w = (string)item["width"];
                    var h = (string)item["height"];
                    map.Add(new MaterialManageErpCacheData()
                    {
                        SupplierCode = sCode,
                        MaterialCode = mCode,
                        Width = w,
                        Height = h,
                    });
                }
            }
            catch (Exception e)
            {
            }

            return map;
        }, () => new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(4)
        });
    }

    public async Task<ApiResultDto> Save(MaterialManageSaveDto input)
    {
        if (_currentUser.Id == null)
        {
            throw new BusinessException("YaSha.DataManager:LoginOver");
        }

        var errorMsg = string.Empty;
        try
        {
            //新增
            if (input.Insert is { Count: > 0 })
            {
                foreach (var item in input.Insert)
                {
                    item.OperatingUser = _currentUser.UserName;
                    item.Id = Guid.NewGuid();
                    try
                    {
                        await _service.CheckInsertInvalid(item);
                        if (!string.IsNullOrEmpty(item.MaterialImageUrl))
                        {
                            var name = Path.GetFileName(item.MaterialImageUrl);
                            var sourcePath = MaterialManageConsts.MaterialImageTmpPath + "/" + name;
                            var targetPath = MaterialManageConsts.MaterialImagePath + "/" + name;
                            if (File.Exists(sourcePath))
                            {
                                File.Copy(sourcePath, targetPath);
                                item.MaterialImageUrl = "https://bds.chinayasha.com/bdsfileservice/MaterialManage/Image/" + name;
                                var apiResultDto = await _apiAppService.CreatedImage(targetPath, item.Id.ToString());
                                if (apiResultDto.Success)
                                {
                                    item.BaiduSign = (string)apiResultDto.Data;
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(item.MaterialImageDownLoadUrl))
                        {
                            var name = Path.GetFileName(item.MaterialImageDownLoadUrl);
                            var sourcePath = MaterialManageConsts.MaterialImageTmpPath + "/" + name;
                            var targetPath = MaterialManageConsts.MaterialDownloadImagePath + "/" + name;
                            if (File.Exists(sourcePath))
                            {
                                File.Copy(sourcePath, targetPath);
                                item.MaterialImageDownLoadUrl = "https://bds.chinayasha.com/bdsfileservice/MaterialManage/DownloadImage/" + name;
                            }
                        }

                        if (!string.IsNullOrEmpty(item.SeriesImageUrl))
                        {
                            var name = Path.GetFileName(item.SeriesImageUrl);
                            var sourcePath = MaterialManageConsts.MaterialImageTmpPath + "/" + name;
                            var targetPath = MaterialManageConsts.MaterialSeriesImagePath + "/" + name;
                            if (File.Exists(sourcePath))
                            {
                                File.Copy(sourcePath, targetPath);
                                item.SeriesImageUrl = "https://bds.chinayasha.com/bdsfileservice/MaterialManage/SeriesImage/" + name;
                            }
                        }

                        await _service.Insert(item);
                    }
                    catch (Exception e)
                    {
                        errorMsg += e.Message + "\n";
                    }
                }
            }

            //删除
            if (input.Delete is { Count: > 0 })
            {
                foreach (var item in input.Delete)
                {
                    try
                    {
                        await _service.Delete(item.Id);
                        if (!string.IsNullOrEmpty(item.MaterialImageUrl))
                        {
                            var name = Path.GetFileName(item.MaterialImageUrl);
                            var localPath = MaterialManageConsts.MaterialImagePath + "/" + name;
                            if (File.Exists(localPath))
                            {
                                File.Delete(localPath);
                            }
                        }

                        if (!string.IsNullOrEmpty(item.MaterialImageDownLoadUrl))
                        {
                            var name = Path.GetFileName(item.MaterialImageDownLoadUrl);
                            var localPath = MaterialManageConsts.MaterialDownloadImagePath + "/" + name;
                            if (File.Exists(localPath))
                            {
                                File.Delete(localPath);
                            }
                        }

                        if (string.IsNullOrEmpty(item.SeriesImageUrl)) continue;
                        {
                            var name = Path.GetFileName(item.SeriesImageUrl);
                            var localPath = MaterialManageConsts.MaterialSeriesImagePath + "/" + name;
                            if (File.Exists(localPath))
                            {
                                File.Delete(localPath);
                            }
                        }
                        await _apiAppService.DeleteImage(item.BaiduSign);
                    }
                    catch (Exception e)
                    {
                        errorMsg += e.Message + "\n";
                    }
                }
            }

            //修改
            if (input.Modify != null && input.Modify.Count > 0)
            {
                foreach (var item in input.Modify)
                {
                    try
                    {
                        var dto = await _service.FindById(item.Id);
                        item.OperatingUser = _currentUser.UserName;
                        dynamic obj = await UpdateMaterialImage(dto, item.MaterialImageUrl);
                        item.MaterialImageUrl = obj.Url;
                        item.BaiduSign = obj.Sign;
                        item.MaterialImageDownLoadUrl = await UpdateMaterialDownloadImage(dto, item.MaterialImageDownLoadUrl);
                        item.SeriesImageUrl = await UpdateMaterialSeriesImage(dto, item.SeriesImageUrl);
                        await _service.Update(item);
                    }
                    catch (Exception e)
                    {
                        errorMsg += e.Message + "\n";
                    }
                }
            }

            if (string.IsNullOrEmpty(errorMsg))
            {
                return new ApiResultDto
                {
                    Success = true,
                };
            }

            return new ApiResultDto
            {
                Success = false,
                Error = errorMsg,
            };
        }
        catch (Exception ex)
        {
            return new ApiResultDto
            {
                Success = false,
                Error = ex.Message
            };
        }
    }

    async Task<dynamic> UpdateMaterialImage(MaterialManageDto dto, string url)
    {
        var sign = "";
        var resulturl = "";
        if (string.IsNullOrEmpty(dto.MaterialImageUrl))
        {
            //修改 新增图片
            if (!string.IsNullOrEmpty(url))
            {
                var name = Path.GetFileName(url);
                var sourcePath = MaterialManageConsts.MaterialImageTmpPath + "/" + name;
                var targetPath = MaterialManageConsts.MaterialImagePath + "/" + name;
                if (File.Exists(sourcePath))
                {
                    File.Copy(sourcePath, targetPath);
                    var apiDto = await _apiAppService.CreatedImage(targetPath, dto.Id.ToString());
                    if (apiDto.Success)
                    {
                        sign = (string)apiDto.Data;
                    }

                    resulturl = "https://bds.chinayasha.com/bdsfileservice/MaterialManage/Image/" + name;
                }
            }

            return new { Sign = sign, Url = resulturl };
        }

        //修改 删除图片
        if (string.IsNullOrEmpty(url))
        {
            var name = Path.GetFileName(url);
            var localPath = MaterialManageConsts.MaterialImagePath + "/" + name;
            if (File.Exists(localPath))
            {
                File.Delete(localPath);
                await _apiAppService.DeleteImage(dto.BaiduSign);
            }

            sign = dto.BaiduSign;
            return new { Sign = sign, Url = resulturl };
        }

        //修改 替换图片
        {
            var name = Path.GetFileName(url);
            var imgUrl = "https://bds.chinayasha.com/bdsfileservice/MaterialManage/Image/" + name;
            sign = dto.BaiduSign;
            resulturl = dto.MaterialImageUrl;
            if (dto.MaterialImageUrl != imgUrl)
            {
                var localPath = MaterialManageConsts.MaterialImagePath + "/" + Path.GetFileName(dto.MaterialImageUrl);
                if (File.Exists(localPath))
                {
                    File.Delete(localPath);
                }

                var sourcePath = MaterialManageConsts.MaterialImageTmpPath + "/" + name;
                var targetPath = MaterialManageConsts.MaterialImagePath + "/" + name;
                if (File.Exists(sourcePath))
                {
                    File.Copy(sourcePath, targetPath);
                    await _apiAppService.DeleteImage(dto.BaiduSign);
                    var apiDto = await _apiAppService.CreatedImage(targetPath, dto.Id.ToString());
                    if (apiDto.Success)
                    {
                        sign = (string)apiDto.Data;
                    }

                    resulturl = "https://bds.chinayasha.com/bdsfileservice/MaterialManage/Image/" + name;
                }
            }

            return new { Sign = sign, Url = resulturl };
        }
    }

    async Task<string> UpdateMaterialDownloadImage(MaterialManageDto dto, string url)
    {
        if (string.IsNullOrEmpty(dto.MaterialImageDownLoadUrl))
        {
            //修改 新增图片
            if (!string.IsNullOrEmpty(url))
            {
                var name = Path.GetFileName(url);
                var sourcePath = MaterialManageConsts.MaterialImageTmpPath + "/" + name;
                var targetPath = MaterialManageConsts.MaterialDownloadImagePath + "/" + name;
                if (File.Exists(sourcePath))
                {
                    File.Copy(sourcePath, targetPath);
                    return "https://bds.chinayasha.com/bdsfileservice/MaterialManage/DownloadImage/" + name;
                }
            }

            return string.Empty;
        }

        //修改 删除图片
        if (string.IsNullOrEmpty(url))
        {
            var name = Path.GetFileName(url);
            var localPath = MaterialManageConsts.MaterialDownloadImagePath + "/" + name;
            if (File.Exists(localPath))
            {
                File.Delete(localPath);
            }

            return string.Empty;
        }

        //修改 替换图片
        {
            var name = Path.GetFileName(url);
            var imgUrl = "https://bds.chinayasha.com/bdsfileservice/MaterialManage/DownloadImage/" + name;

            if (dto.MaterialImageDownLoadUrl != imgUrl)
            {
                var localPath = MaterialManageConsts.MaterialDownloadImagePath + "/" + Path.GetFileName(dto.MaterialImageDownLoadUrl);
                if (File.Exists(localPath))
                {
                    File.Delete(localPath);
                }

                var sourcePath = MaterialManageConsts.MaterialImageTmpPath + "/" + name;
                var targetPath = MaterialManageConsts.MaterialDownloadImagePath + "/" + name;
                if (File.Exists(sourcePath))
                {
                    File.Copy(sourcePath, targetPath);
                    return "https://bds.chinayasha.com/bdsfileservice/MaterialManage/DownloadImage/" + name;
                }
            }

            return imgUrl;
        }
    }

    async Task<string> UpdateMaterialSeriesImage(MaterialManageDto dto, string url)
    {
        if (string.IsNullOrEmpty(dto.SeriesImageUrl))
        {
            //修改 新增图片
            if (!string.IsNullOrEmpty(url))
            {
                var name = Path.GetFileName(url);
                var sourcePath = MaterialManageConsts.MaterialImageTmpPath + "/" + name;
                var targetPath = MaterialManageConsts.MaterialSeriesImagePath + "/" + name;
                if (File.Exists(sourcePath))
                {
                    File.Copy(sourcePath, targetPath);
                    return "https://bds.chinayasha.com/bdsfileservice/MaterialManage/SeriesImage/" + name;
                }
            }

            return string.Empty;
        }

        //修改 删除图片
        if (string.IsNullOrEmpty(url))
        {
            var name = Path.GetFileName(url);
            var localPath = MaterialManageConsts.MaterialSeriesImagePath + "/" + name;
            if (File.Exists(localPath))
            {
                File.Delete(localPath);
            }

            return string.Empty;
        }

        //修改 替换图片
        {
            var name = Path.GetFileName(url);
            var imgUrl = "https://bds.chinayasha.com/bdsfileservice/MaterialManage/SeriesImage/" + name;

            if (dto.SeriesImageUrl != imgUrl)
            {
                var localPath = MaterialManageConsts.MaterialSeriesImagePath + "/" + Path.GetFileName(dto.SeriesImageUrl);
                if (File.Exists(localPath))
                {
                    File.Delete(localPath);
                }

                var sourcePath = MaterialManageConsts.MaterialImageTmpPath + "/" + name;
                var targetPath = MaterialManageConsts.MaterialSeriesImagePath + "/" + name;
                if (File.Exists(sourcePath))
                {
                    File.Copy(sourcePath, targetPath);
                    return "https://bds.chinayasha.com/bdsfileservice/MaterialManage/SeriesImage/" + name;
                }
            }

            return imgUrl;
        }
    }
}