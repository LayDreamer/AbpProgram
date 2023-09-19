using YaSha.DataManager.BasicManagement.Localization;

namespace YaSha.DataManager.BasicManagement;

public abstract class BasicManagementAppService : ApplicationService
{
    protected BasicManagementAppService()
    {
        LocalizationResource = typeof(BasicManagementResource);
        ObjectMapperContext = typeof(BasicManagementApplicationModule);
    }
}
