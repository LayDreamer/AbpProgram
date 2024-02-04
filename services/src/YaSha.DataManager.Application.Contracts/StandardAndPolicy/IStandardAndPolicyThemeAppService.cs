using Microsoft.AspNetCore.Http;
using Volo.Abp.Application.Dtos;
using YaSha.DataManager.StandardAndPolicy.Dto;

namespace YaSha.DataManager.StandardAndPolicy;

public interface IStandardAndPolicyThemeAppService : IApplicationService
{
    Task<ApiResultDto> UploadFile(IFormFile file);
    Task<List<StandardAndPolicyThemeDto>> InsertStandardAndPolicyTheme(StandardAndPolicyCreateAndUpdateDto input);
    Task<List<StandardAndPolicyPageDto>> PageHome(int showCount = 4);
    Task<List<StandardAndPolicyPageDto>> PageSelectTree(Guid id, int showCount = 4);
    Task<PagedResultDto<StandardAndStandLibWithAllTreeDto>> PageSearchDetail(StandardAndPolicySearchDto input);
    Task<StandardAndPolicyCardDetailDto> GetCardDetail(Guid id);
    Task<StandardAndPolicyCollectDto> UpdateCollectStatus(Guid id, bool status);
    Task<StandardAndPolicyLibDto> UpdateLib(Guid id, StandardAndPolicyCreateAndUpdateDto input);
    Task<ApiResultDto> DeleteLibs(List<Guid> ids);
    Task<byte[]> ExportExcel();
}