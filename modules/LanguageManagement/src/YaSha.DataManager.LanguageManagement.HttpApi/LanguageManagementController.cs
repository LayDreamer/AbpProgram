using YaSha.DataManager.LanguageManagement.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace YaSha.DataManager.LanguageManagement
{
    public abstract class LanguageManagementController : AbpController
    {
        protected LanguageManagementController()
        {
            LocalizationResource = typeof(LanguageManagementResource);
        }
    }
}
