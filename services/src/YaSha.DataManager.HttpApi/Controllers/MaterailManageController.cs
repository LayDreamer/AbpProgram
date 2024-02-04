using System.Diagnostics;
using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Volo.Abp.Application.Dtos;
using YaSha.DataManager.MaterailManage;
using YaSha.DataManager.MaterailManage.Dto;
using YaSha.DataManager.MaterialManage.Dto;
using YaSha.DataManager.ProductInventory;
using YaSha.DataManager.ProductRetrieval;
using YaSha.DataManager.StandardAndPolicy.Dto;

namespace YaSha.DataManager.Controllers;

/// <summary>
/// 材质库
/// </summary>
[Authorize]
[Route("MaterailManage")]
public class MaterailManageController : DataManagerController, IMaterailManageAppService
{
    private readonly IMaterailManageAppService _service;

    public MaterailManageController(IMaterailManageAppService service, IProductIventProductAppService productService, IMaterialInventoryAppService inventoryService)
    {
        _service = service;
    }

    [HttpPost("UploadImage")]
    public async Task<ApiResultDto> UploadImage(IFormFile image)
    {
        return await _service.UploadImage(image);
    }

    [HttpPost("InsertMaterialImage")]
    public async Task<ApiResultDto> InsertMaterialImages(List<IFormFile> images, List<IFormFile> downloads)
    {
        return await _service.InsertMaterialImages(images, downloads);
    }

    [HttpPost("InsertSeriesImage")]
    public async Task<ApiResultDto> InsertSeriesImages(List<IFormFile> images)
    {
        return await _service.InsertSeriesImages(images);
    }

    [HttpPost("ManageExtraInfo")]
    public async Task<MaterialManageManageExtraInfo> GetManageExtra()
    {
        return await _service.GetManageExtra();
    }

    [HttpPost("PageManage")]
    public async Task<PagedResultDto<MaterialManageFullDto>> PageManage(MaterialManageSearchDto search)
    {
        return await _service.PageManage(search);
    }

    [HttpPost("PageHome")]
    public async Task<MaterialManageHomeDto> PageHome(MaterialManageHomeSearchDto search)
    {
        return await _service.PageHome(search);
    }


    [HttpPost("GetDetail")]
    public async Task<List<MaterialManageFullDto>> GetDetail(List<Guid> id)
    {
        return await _service.GetDetail(id);
    }

    [HttpPost("CameraSearch")]
    public async Task<ApiResultDto> GetImageApiMap(IFormFile file, string key)
    {
        return await _service.GetImageApiMap(file, key);
    }

    [HttpPost("Save")]
    public async Task<ApiResultDto> Save(MaterialManageSaveDto input)
    {
        return await _service.Save(input);
    }
}