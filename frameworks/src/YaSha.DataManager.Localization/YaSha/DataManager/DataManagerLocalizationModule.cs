namespace YaSha.DataManager;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpLocalizationModule)
)]
public class DataManagerLocalizationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options => { options.FileSets.AddEmbedded<DataManagerLocalizationModule>(DataManagerLocalizationConsts.NameSpace); });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<DataManagerLocalizationResource>(DataManagerLocalizationConsts.DefaultCultureName)
                .AddVirtualJson(DataManagerLocalizationConsts.DefaultLocalizationResourceVirtualPath);

            options.DefaultResourceType = typeof(DataManagerLocalizationResource);
        });

        Configure<AbpExceptionLocalizationOptions>(options => { options.MapCodeNamespace(DataManagerLocalizationConsts.NameSpace, typeof(DataManagerLocalizationResource)); });
    }
}