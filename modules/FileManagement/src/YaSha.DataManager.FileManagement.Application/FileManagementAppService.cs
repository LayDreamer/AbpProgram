namespace YaSha.DataManager.FileManagement;

public abstract class FileManagementAppService : ApplicationService
{
    protected FileManagementAppService()
    {
        LocalizationResource = typeof(FileManagementResource);
        ObjectMapperContext = typeof(FileManagementApplicationModule);
    }
}