using Microsoft.AspNetCore.Http;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Users;
using YaSha.DataManager.StandardAndPolicy.Dto;

namespace YaSha.DataManager.ListProcessing;

public class ListProcessingAppService: DataManagerAppService, IListProcessingAppService
{
    private readonly ListProcessingManager _service;

    private readonly ICurrentUser _currentUser;

    public ListProcessingAppService(ListProcessingManager service, ICurrentUser currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    public  async Task<PagedResultDto<ListProcessingDto>> Page(ListProcessingSearchDto input)
    {
        if (_currentUser.Id == null)
        {
            throw new BusinessException("YaSha.DataManager:LoginOver" );
        }
        return await _service.Page(_currentUser.GetId(), input.Search, input.Sorting, input.SkipCount, input.MaxResultCount);
    }

    public async Task<List<string>> GetSelectData(ListProcessingSelectEnum type)
    {
        var results = new List<string>();
        var path = type switch
        {
            ListProcessingSelectEnum.Config => ListProcessingConsts.ListProcessingConfigPath,
            ListProcessingSelectEnum.Rule => ListProcessingConsts.ListProcessingRulePath,
            ListProcessingSelectEnum.Nest => ListProcessingConsts.ListProcessingNestPath,
            _ => string.Empty
        };

        if (!string.IsNullOrEmpty(path))
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            
            results  = await Task.Run(() => Directory.GetFiles(path).Select(Path.GetFileName).ToList());
        }
        return results;
    }
    
    public async Task<ApiResultDto> UpLoadListProcessingRules(IFormFile file, ListProcessingSelectEnum type)
    {
        var path = type switch
        {
            ListProcessingSelectEnum.Config => ListProcessingConsts.ListProcessingConfigPath,
            ListProcessingSelectEnum.Rule => ListProcessingConsts.ListProcessingRulePath,
            ListProcessingSelectEnum.Nest => ListProcessingConsts.ListProcessingNestPath,
            ListProcessingSelectEnum.Sheet => ListProcessingConsts.ListProcessingFilePath,
            _ => string.Empty
        };
        try
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var name = file.FileName;
            var localPath = path + "\\" + name;
            using (FileStream fileStream = new FileStream(localPath, FileMode.Create, FileAccess.Write))
            {
                await file.CopyToAsync(fileStream);
            }
            var serverPath = "https://bds.chinayasha.com/bdsfileservice/ListProcessing/" + Path.GetFileName(path) + "/" + name;
            return new ApiResultDto()
            {
                Data = serverPath,
                Success = true,
            };
        }
        catch (Exception e)
        {
            return new ApiResultDto()
            {
                Error = e.Message,
                Success = false,
            };
        }
    }

    public async Task<ApiResultDto> BuildSheets(ListProcessingBuildDto input)
    {
        var filePath = Path.Combine(ListProcessingConsts.ListProcessingFilePath, input.File);
        var configPath = Path.Combine(ListProcessingConsts.ListProcessingConfigPath, input.Config);
        var rulePath = Path.Combine(ListProcessingConsts.ListProcessingRulePath, input.Rule);
        var nestPath = Path.Combine(ListProcessingConsts.ListProcessingNestPath, input.Nest);
        var savePath = Path.Combine(ListProcessingConsts.ListProcessingResultPath, input.File);
        var factory = new BuildFactory(filePath, configPath, rulePath, nestPath, savePath, input.Type, input.Dt, input.Sheets);
        try
        {
            if (!Directory.Exists(ListProcessingConsts.ListProcessingResultPath))
            {
                Directory.CreateDirectory(ListProcessingConsts.ListProcessingResultPath);
            }
            factory.Build(input.Replace);
            var dto = await _service.CreateAndUpdate(input.File);
            return new ApiResultDto()
            {
                Success = true,
                Data = dto.FilePath
            };
        }
        catch(Exception e)
        {
            return new ApiResultDto()
            {
                Success = false,
                Error = e.Message,
            };
        }
    }

    public async Task<ApiResultDto> DeleteSheets(List<Guid> ids)
    {
        try
        {
            var delete = await _service.Delete(ids);
            foreach (var deletePath in delete.Select(dto => ListProcessingConsts.ListProcessingFilePath + "/" + Path.GetFileName(dto.FilePath)))
            {
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
    
}