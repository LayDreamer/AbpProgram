using EasyAbp.Abp.Trees;
using EasyAbp.PrivateMessaging;

namespace YaSha.DataManager
{
    [DependsOn(
        typeof(DataManagerDomainSharedModule),
        typeof(AbpEmailingModule),
        typeof(BasicManagementDomainModule),
        typeof(DataDictionaryManagementDomainModule),
        typeof(NotificationManagementDomainModule),
        typeof(LanguageManagementDomainModule),
         typeof(PrivateMessagingDomainModule),
        typeof(AbpTreesDomainModule)
    )]
    public class DataManagerDomainModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpMultiTenancyOptions>(options => { options.IsEnabled = MultiTenancyConsts.IsEnabled;});
            Configure<AbpAutoMapperOptions>(options => { options.AddMaps<DataManagerDomainModule>(); });
        }
    }
}