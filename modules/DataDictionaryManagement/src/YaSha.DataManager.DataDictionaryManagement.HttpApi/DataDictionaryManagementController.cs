using YaSha.DataManager.DataDictionaryManagement.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace YaSha.DataManager.DataDictionaryManagement
{
    public abstract class DataDictionaryManagementController : AbpController
    {
        protected DataDictionaryManagementController()
        {
            LocalizationResource = typeof(DataDictionaryManagementResource);
        }
    }
}
