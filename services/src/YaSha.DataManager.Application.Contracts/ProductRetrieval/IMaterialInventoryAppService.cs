using Autofac.Core;
using Volo.Abp.Application.Dtos;
using YaSha.DataManager.FamilyLibs;
using YaSha.DataManager.ProductRetrieval.Dto;
using YaSha.DataManager.StandardAndPolicy.Dto;

namespace YaSha.DataManager.ProductRetrieval;

public interface IMaterialInventoryAppService : IApplicationService
{
    Task<ApiResultDto> UploadDataAsync(List<MaterialInventoryCreateDto> input);

    Task<List<MaterialInventoryDto>> ParsingListDataAsync(List<string> code);

    /// <summary>
    /// 物料库存信息查询
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    Task<ApiResultDto> GetByErpAsync(List<string> code);


    
}