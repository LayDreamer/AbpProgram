using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using YaSha.DataManager.MaterailManage.Dto;
using YaSha.DataManager.MaterialManage.Dto;
using YaSha.DataManager.StandardAndPolicy.Dto;

namespace YaSha.DataManager.MaterailManage;

public interface IMaterailManageAppService : IApplicationService
{
    Task<ApiResultDto> UploadImage(IFormFile image);

    Task<ApiResultDto> InsertMaterialImages(List<IFormFile> images, List<IFormFile> downloads);

    Task<ApiResultDto> InsertSeriesImages(List<IFormFile> images);

    Task<MaterialManageManageExtraInfo> GetManageExtra();
    
    Task<PagedResultDto<MaterialManageFullDto>> PageManage(MaterialManageSearchDto search);

    Task<MaterialManageHomeDto> PageHome([FromForm] MaterialManageHomeSearchDto search);

    Task<List<MaterialManageFullDto>> GetDetail(List<Guid> id);

    Task<ApiResultDto> GetImageApiMap(IFormFile file, string key);
    
    Task<ApiResultDto> Save(MaterialManageSaveDto input);
}