using YaSha.DataManager.Core;

namespace YaSha.DataManager.BasicManagement;

[DependsOn(
    typeof(AbpAuditLoggingDomainSharedModule),
    typeof(AbpBackgroundJobsDomainSharedModule),
    typeof(AbpFeatureManagementDomainSharedModule),
    typeof(AbpIdentityDomainSharedModule),
    typeof(AbpPermissionManagementDomainSharedModule),
    typeof(AbpSettingManagementDomainSharedModule),
    typeof(AbpTenantManagementDomainSharedModule),
    typeof(DataManagerCoreModule)
)]
public class BasicManagementDomainSharedModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<BasicManagementDomainSharedModule>(BasicManagementConsts.NameSpace);
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<BasicManagementResource>(BasicManagementConsts.DefaultCultureName)
                .AddVirtualJson("/Localization/BasicManagement")
                .AddBaseTypes(typeof(IdentityResource))
                .AddBaseTypes(typeof(AbpValidationResource))
                .AddBaseTypes(typeof(AbpLocalizationResource))
                .AddBaseTypes(typeof(AbpSettingManagementResource));

            options.DefaultResourceType = typeof(BasicManagementResource);
        });

        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace(BasicManagementConsts.NameSpace, typeof(BasicManagementResource));
        });
    }
}
