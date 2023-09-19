using EasyAbp.Abp.Trees;
using EasyAbp.PrivateMessaging;
using YaSha.DataManager.BasicManagement;
using YaSha.DataManager.BasicManagement.Localization;
using YaSha.DataManager.Core;
using YaSha.DataManager.LanguageManagement;

namespace YaSha.DataManager
{
    [DependsOn(
        typeof(DataManagerCoreModule),
        typeof(BasicManagementDomainSharedModule),
        typeof(DataDictionaryManagementDomainSharedModule),
        typeof(NotificationManagementDomainSharedModule),
        typeof(LanguageManagementDomainSharedModule),
        typeof(PrivateMessagingDomainSharedModule),
        typeof(AbpTreesDomainSharedModule)
    )]
    public class DataManagerDomainSharedModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            DataManagerGlobalFeatureConfigurator.Configure();
            DataManagerModuleExtensionConfigurator.Configure();
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpVirtualFileSystemOptions>(options => { options.FileSets.AddEmbedded<DataManagerDomainSharedModule>(DataManagerDomainSharedConsts.NameSpace); });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Add<DataManagerResource>(DataManagerDomainSharedConsts.DefaultCultureName)
                    .AddVirtualJson("/Localization/DataManager")
                    .AddBaseTypes(typeof(BasicManagementResource))
                    .AddBaseTypes(typeof(AbpTimingResource));

                options.DefaultResourceType = typeof(DataManagerResource);
            });

            Configure<AbpExceptionLocalizationOptions>(options => { options.MapCodeNamespace(DataManagerDomainSharedConsts.NameSpace, typeof(DataManagerResource)); });
        }
    }
}