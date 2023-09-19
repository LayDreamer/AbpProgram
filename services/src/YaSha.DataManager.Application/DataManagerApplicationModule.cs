using EasyAbp.Abp.Trees;
using EasyAbp.PrivateMessaging;

namespace YaSha.DataManager
{
    [DependsOn(
        typeof(DataManagerDomainModule),
        typeof(DataManagerApplicationContractsModule),
        typeof(BasicManagementApplicationModule),
        typeof(DataDictionaryManagementApplicationModule),
        typeof(NotificationManagementApplicationModule),
        typeof(LanguageManagementApplicationModule),
        typeof(DataManagerFreeSqlModule),
        typeof(PrivateMessagingApplicationModule),
        typeof(AbpTreesApplicationModule)
    )]
    public class DataManagerApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(options => 
            { 
                options.AddMaps<DataManagerApplicationModule>(); 
            });
        }
    }
}