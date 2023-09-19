using YaSha.DataManager.BasicManagement.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace YaSha.DataManager.BasicManagement;

public abstract class BasicManagementController : AbpControllerBase
{
    protected BasicManagementController()
    {
        LocalizationResource = typeof(BasicManagementResource);
    }
}
