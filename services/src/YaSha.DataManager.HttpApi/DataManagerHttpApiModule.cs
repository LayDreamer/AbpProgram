using EasyAbp.Abp.Trees;
using Microsoft.Extensions.DependencyInjection;
using YaSha.DataManager.BasicManagement;
using YaSha.DataManager.LanguageManagement;

namespace YaSha.DataManager
{
    [DependsOn(
        typeof(DataManagerApplicationContractsModule),
        typeof(BasicManagementHttpApiModule),
        typeof(DataDictionaryManagementHttpApiModule),
        typeof(NotificationManagementHttpApiModule),
        typeof(LanguageManagementHttpApiModule),
        typeof(AbpTreesHttpApiModule)
        )]
    public class DataManagerHttpApiModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            ConfigureLocalization();
        }

        private void ConfigureLocalization()
        {
            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Get<DataManagerResource>()
                    .AddBaseTypes(
                        typeof(AbpUiResource)
                    );
            });
        }
    }
}
