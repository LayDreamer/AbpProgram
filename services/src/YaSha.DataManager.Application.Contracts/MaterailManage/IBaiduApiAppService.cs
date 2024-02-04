using YaSha.DataManager.StandardAndPolicy.Dto;

namespace YaSha.DataManager.MaterailManage;

public interface IBaiduApiAppService : IApplicationService
{
    Task<ApiResultDto> CreatedImage(string filePath, string brief, string tag = "1");
    
    Task<ApiResultDto> DeleteImage(string sign);
    
    Task<ApiResultDto> UpdateImage(string sign, string brief, string tags);
    
    Task<ApiResultDto> SearchImage(string filePath, string tags = "1", string logic = "1");
}