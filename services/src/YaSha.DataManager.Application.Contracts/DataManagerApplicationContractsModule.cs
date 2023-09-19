using EasyAbp.Abp.Trees;
using EasyAbp.PrivateMessaging;

namespace YaSha.DataManager
{
    [DependsOn(
        typeof(DataManagerDomainSharedModule),
        typeof(AbpObjectExtendingModule),
        typeof(BasicManagementApplicationContractsModule),
        typeof(DataDictionaryManagementApplicationContractsModule),
        typeof(LanguageManagementApplicationContractsModule), 
        typeof(PrivateMessagingApplicationContractsModule),
        typeof(AbpTreesApplicationContractsModule)
    )]
    public class DataManagerApplicationContractsModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            DataManagerDtoExtensions.Configure();
        }
    }
}
