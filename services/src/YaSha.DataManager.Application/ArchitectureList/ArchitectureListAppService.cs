using System.Diagnostics;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TencentCloud.Teo.V20220901.Models;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using YaSha.DataManager.ArchitectureList.Dto;
using YaSha.DataManager.ArchitectureList.Manager;
using YaSha.DataManager.NewFamilyLibrary;
using YaSha.DataManager.ProductInventory.Dto;
using YaSha.DataManager.StandardAndPolicy.Dto;

namespace YaSha.DataManager.ArchitectureList;

public class ArchitectureListAppService : DataManagerAppService, IArchitectureListAppService, ITransientDependency
{
    private readonly ArchitectureListManager _architectureListManager;
    private readonly ICurrentUser _currentUser;

    public ArchitectureListAppService(
        ICurrentUser currentUser, 
        ArchitectureListManager architectureListManager)
    {
        _currentUser = currentUser;
        _architectureListManager = architectureListManager;
    }

    public async Task<List<ArchitectureListTreeDto>> GetTreeRoot()
    {
        return await _architectureListManager.GetTreeRoot();
    }

    public async Task<ArchitectureListTreeDto> UpdateTreeNode(ArchitectureListTreeDto input)
    {
        return await _architectureListManager.UpdateTreeNode(_currentUser.UserName, input);
    }
    public async Task<List<ArchitectureListFileDto>> GetFileList(Guid treeId, ArchitectureListFileStatus status)
    {
        return await _architectureListManager.GetFileList(treeId, status);
    }

    public async Task<List<ArchitectureListModuleDto>> UpdateStatus(ArchitectureUpdateStatus input)
    {
        return await _architectureListManager.UpdateStatus(_currentUser.UserName, input.Id, input.Status);
    }

    public async Task<ArchitectureListModuleDto> InsertModule(ArchitectureListModuleDto input)
    {
        //var results = new List<ArchitectureListModuleDto>();
        //foreach (var dto in dtos)
        //{
        //    results.Add(await _architectureListManager.AddModules(_currentUser.UserName, input));
        //}
        var results = await _architectureListManager.InsertModule(_currentUser.UserName, input);
        return results;
    }

    public async Task<List<ArchitectureListModuleDto>> CopyModules(List<Guid> ids)
    {
        return await _architectureListManager.CopyModules(_currentUser.UserName, ids);
    }

    public async Task<ArchitectureListModuleDto> Update(ArchitectureListModuleDto input)
    {
        if (_currentUser.Id == null)
        {
            throw new BusinessException("YaSha.DataManager:LoginOver");
        }

        return await _architectureListManager.Update(_currentUser.UserName, input);
    }

    public async Task<ApiResultDto> Delete(List<Guid> id)
    {
        try
        {
            await _architectureListManager.Delete(id);
        }
        catch (Exception e)
        {
            return new ApiResultDto()
            {
                Success = false,
                Error = e.Message,
            };
        }

        return new ApiResultDto()
        {
            Success = true
        };
    }

    public async Task<ApiResultDto> DeleteFiles(List<Guid> id)
    {
        try
        {
            var dto = await _architectureListManager.DeleteFile(id);

            foreach (var item in dto)
            {
                var deletePath = ArchitectureListConst.ArchitectureListFileServerPath + "/" + Path.GetFileName(item.FilePath);
                File.Delete(deletePath);
                deletePath = ArchitectureListConst.ArchitectureListFileDownloadPath + "/" + Path.GetFileName(item.FilePath);
                File.Delete(deletePath);
            }
        }
        catch (Exception e)
        {
            return new ApiResultDto()
            {
                Success = false,
                Error = e.Message,
            };
        }

        return new ApiResultDto()
        {
            Success = true,
        };
    }

    public async Task<List<ArchitectureListModuleDto>> ImportFromExcel(string system, bool isProcess, List<ArchitectureListModuleCreateDto> input)
    {
        if (_currentUser.Id == null)
        {
            throw new BusinessException("YaSha.DataManager:LoginOver");
        }

        Check.NotNullOrEmpty(system, "system");
        return await _architectureListManager.ImportFromExcel(_currentUser.UserName, system, isProcess, input);
    }

    public async Task<ApiResultDto> InsertFile(Guid id, ArchitectureListFileStatus status, List<IFormFile> files)
    {
        string errMsg = string.Empty;
        var dtos = new List<ArchitectureListFileDto>();
        if (!Directory.Exists(ArchitectureListConst.ArchitectureListFileServerPath))
        {
            Directory.CreateDirectory(ArchitectureListConst.ArchitectureListFileServerPath);
        }

        foreach (var file in files)
        {
            try
            {
                var name = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(file.FileName);
                var localPath = ArchitectureListConst.ArchitectureListFileServerPath + "/" + name;
                using (FileStream fileStream = new FileStream(localPath, FileMode.Create, FileAccess.Write))
                {
                    await file.CopyToAsync(fileStream);
                }

                var serverPath = "https://bds.chinayasha.com/bdsfileservice/ProductList/File/Server/" + name;
                var encryptionServerPath = "https://bds.chinayasha.com/bdsfileservice/ProductList/File/Download/" + name;
                var data = await _architectureListManager.InsertFile(file.FileName, serverPath, encryptionServerPath, id,status);
                dtos.Add(data);
            }
            catch (Exception ex)
            {
                errMsg += ex.Message;
            }
        }

        if (string.IsNullOrEmpty(errMsg))
        {
            return new ApiResultDto
            {
                Success = true,
                Data = dtos,
            };
        }

        return new ApiResultDto
        {
            Success = false,
            Error = errMsg,
        };
    }

    public async Task<ApiResultDto> InsertModuleFiles(Guid moduleId, List<Guid> fileIds)
    {
        try
        {
            var data = await _architectureListManager.InsertModuleFiles(moduleId, fileIds);
            return new ApiResultDto
            {
                Success = true,
                Data = data,
            };
        }
        catch (Exception e)
        {
            return new ApiResultDto
            {
                Success = false,
                Error = e.Message,
            };
        }
    }

    public async Task<PagedResultDto<ArchitectureListModuleDto>> Page(ArchitectureSearchDto input)
    {
        return await _architectureListManager.Page(input.Key,
            input.SearchValue,
            input.SearchCode,
            input.Status,
            input.Sorting,
            input.SkipCount,
            input.MaxResultCount);
    }

    public async Task<object> RevitSearch(List<string> names)
    {
        return await _architectureListManager.RevitSearch(names);
    }

    public async Task<object> CadSearch(List<string> names)
    {
        return await _architectureListManager.CadSearch(names);
    }


}