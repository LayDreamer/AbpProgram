using YaSha.DataManager.BasicManagement;
using YaSha.DataManager.LanguageManagement;
using YaSha.DataManager.NotificationManagement;

namespace YaSha.DataManager
{
    [DependsOn(
        typeof(DataManagerApplicationContractsModule),
        typeof(BasicManagementHttpApiClientModule),
        typeof(DataDictionaryManagementHttpApiClientModule),
        typeof(NotificationManagementHttpApiClientModule),
        typeof(LanguageManagementHttpApiClientModule)
    )]
    public class DataManagerHttpApiClientModule : AbpModule
    {
        public const string RemoteServiceName = "Default";

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddHttpClientProxies(
                typeof(DataManagerApplicationContractsModule).Assembly,
                RemoteServiceName
            );
        }
    }
}
