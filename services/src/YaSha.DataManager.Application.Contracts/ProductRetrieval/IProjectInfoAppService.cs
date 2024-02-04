using Autofac.Core;
using Microsoft.AspNetCore.Http;
using Volo.Abp.Application.Dtos;
using YaSha.DataManager.FamilyLibs;
using YaSha.DataManager.ProductRetrieval.Dto;
using YaSha.DataManager.StandardAndPolicy.Dto;

namespace YaSha.DataManager.ProductRetrieval;

public interface IProjectInfoAppService :IApplicationService
{
    Task<ApiResultDto> ImportFromExcel(IFormFile file);
    Task<List<ProjectInfoCodeResultDto>> Parsing(List<ProjectInfoSearchCodeDto> input);

    //Task<List<ProjectInfoCodeResultDto>> GetByErp(List<ProjectInfoSearchCodeDto> input);
    /// <summary>
    /// 项目信息查询
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<ApiResultDto> GetByErp(List<ProjectInfoSearchCodeDto> input);
}