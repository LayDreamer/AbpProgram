using YaSha.DataManager.StandardAndPolicy.Dto;

namespace YaSha.DataManager.MaterailManage;

public interface ITencentApiAppService : IApplicationService
{
    Task<ApiResultDto> CreatedLibrary();

    Task<ApiResultDto> CreatedImage(string entId, string filePath);

    Task<ApiResultDto> DeleteImage(string entId);

    Task<ApiResultDto> GetLibraryInfo();

    Task<ApiResultDto> GetImageInfo(string entId);

    Task<ApiResultDto> SearchImage(string filePath);
}