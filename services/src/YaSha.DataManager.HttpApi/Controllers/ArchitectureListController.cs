using Autodesk.Forge.Client;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Nodes;
using Volo.Abp.Application.Dtos;
using YaSha.DataManager.ArchitectureList;
using YaSha.DataManager.ArchitectureList.Dto;
using YaSha.DataManager.Models;
using YaSha.DataManager.StandardAndPolicy.Dto;

namespace YaSha.DataManager.Controllers;

/// <summary>
/// 新产品架构清单
/// </summary>
[Route("ArchitectureList")]
public class ArchitectureListController : DataManagerController, IArchitectureListAppService
{
    private readonly IArchitectureListAppService _service;
    private readonly APS _aps;

    public record BucketObject(string name, string urn);

    public record AccessToken(string access_token, long expires_in);

    public ArchitectureListController(IArchitectureListAppService service, APS aps)
    {
        _service = service;
        _aps = aps;
    }

    [HttpPost("GetTree")]
    public async Task<List<ArchitectureListTreeDto>> GetTreeRoot()
    {
        return await _service.GetTreeRoot();
    }

    [HttpPost("UpdateTreeNode")]
    public async Task<ArchitectureListTreeDto> UpdateTreeNode(ArchitectureListTreeDto input)
    {
        return await _service.UpdateTreeNode(input);
    }
    

    [HttpPost("GetFileList")]
    public async Task<List<ArchitectureListFileDto>> GetFileList(Guid treeId, ArchitectureListFileStatus status)
    {
        return await _service.GetFileList(treeId, status);
    }

    [HttpPost("UpdateStatus")]
    public async Task<List<ArchitectureListModuleDto>> UpdateStatus(ArchitectureUpdateStatus input)
    {
        return await _service.UpdateStatus(input);
    }

    [HttpPost("InsertModule")]
    public async Task<ArchitectureListModuleDto> InsertModule(ArchitectureListModuleDto input)
    {
        return await _service.InsertModule(input);
    }

    [HttpPost("CopyModules")]
    public async Task<List<ArchitectureListModuleDto>> CopyModules(List<Guid> ids)
    {
        return await _service.CopyModules(ids);
    }

    [HttpPost("Update")]
    public async Task<ArchitectureListModuleDto> Update(ArchitectureListModuleDto input)
    {
        return await _service.Update(input);
    }

    [HttpPost("Delete")]
    public async Task<ApiResultDto> Delete(List<Guid> id)
    {
        return await _service.Delete(id);
    }

    [HttpPost("DeleteFile")]
    public async Task<ApiResultDto> DeleteFiles(List<Guid> id)
    {
        return await _service.DeleteFiles(id);
    }

    [HttpPost("ImportFromExcel")]
    public async Task<List<ArchitectureListModuleDto>> ImportFromExcel(string system, bool isProcess, List<ArchitectureListModuleCreateDto> input)
    {
        return await _service.ImportFromExcel(system, isProcess, input);
    }

    [HttpPost("InsertFile")]
    public async Task<ApiResultDto> InsertFile(Guid id, ArchitectureListFileStatus status, [FromForm(Name = "file")] List<IFormFile> files)
    {
        return await _service.InsertFile(id, status, files);
    }

    [HttpPost("InsertModuleFiles")]
    public async Task<ApiResultDto> InsertModuleFiles(Guid moduleId, List<Guid> fileIds)
    {
        return await _service.InsertModuleFiles(moduleId, fileIds);
    }


    [HttpPost("Page")]
    public async Task<PagedResultDto<ArchitectureListModuleDto>> Page(ArchitectureSearchDto input)
    {
        return await _service.Page(input);
    }

    [HttpPost("RevitSearch")]
    public async Task<object> RevitSearch(List<string> names)
    {
        return await _service.RevitSearch(names);
    }

    [HttpPost("CadSearch")]
    public async Task<object> CadSearch(List<string> names)
    {
        return await _service.CadSearch(names);
    }


    #region 模型显示

    [HttpPost("GetToken")]
    public async Task<AccessToken> GetAccessToken()
    {
        var token = await _aps.GetPublicToken();
        return new AccessToken(
            token.AccessToken,
            (long)Math.Round((token.ExpiresAt - DateTime.UtcNow).TotalSeconds)
        );
    }

    [HttpPost("GetModelList")]
    public async Task<IEnumerable<BucketObject>> GetModels()
    {
        var objects = await _aps.GetObjects();
        return from o in objects
            select new BucketObject(o.ObjectKey, APS.Base64Encode(o.ObjectId));
    }

    [HttpPost("GetStatus")]
    public async Task<TranslationStatus> GetModelStatus(string urn)
    {
        try
        {
            var status = await _aps.GetTranslationStatus(urn);
            return status;
        }
        catch (ApiException ex)
        {
            if (ex.ErrorCode == 404)
                return new TranslationStatus("n/a", "", new List<string>());
            else
                throw ex;
        }
    }

    [HttpPost("RemoveUploadList")]
    public async Task RemoveUploadList()
    {
        await _aps.RemoveAllObjects();
    }

    [HttpPost("UpLoadModel")]
    public async Task<BucketObject> UploadAndTranslateModel(IFormFile file)
    {
        var list = await GetModels();
        var current = list.FirstOrDefault(x => x.name == file.FileName);
        if (current != null)
        {
            var status = await GetModelStatus(current.urn);
            if (status.Status == "success")
            {
                return current;
            }
        }

        using (var stream = new MemoryStream())
        {
            await file.CopyToAsync(stream);
            stream.Position = 0;
            var obj = await _aps.UploadModel(file.FileName, stream);
            var job = await _aps.TranslateModel(obj.ObjectId, null);
            return new BucketObject(obj.ObjectKey, job.Urn);
        }
    }

    #endregion
}