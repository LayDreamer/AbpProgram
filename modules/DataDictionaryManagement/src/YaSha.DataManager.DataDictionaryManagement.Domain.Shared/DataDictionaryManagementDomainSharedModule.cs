using YaSha.DataManager.Core;

namespace YaSha.DataManager.DataDictionaryManagement
{
    [DependsOn(
        typeof(AbpValidationModule),
        typeof(DataManagerCoreModule)
    )]
    public class DataDictionaryManagementDomainSharedModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<DataDictionaryManagementDomainSharedModule>();
            });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Add<DataDictionaryManagementResource>(DataDictionaryManagementConsts.DefaultCultureName)
                    .AddBaseTypes(typeof(AbpValidationResource))
                    .AddVirtualJson("/Localization/DataDictionaryManagement");
            });

            Configure<AbpExceptionLocalizationOptions>(options =>
            {
                options.MapCodeNamespace(DataDictionaryManagementConsts.NameSpace, typeof(DataDictionaryManagementResource));
            });
        }
    }
}
