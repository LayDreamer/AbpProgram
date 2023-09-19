namespace YaSha.DataManager.ElasticSearch;

[DependsOn(typeof(AbpAutofacModule))]
public class DataManagerElasticSearchModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.Configure<DataManagerElasticSearchOptions>(context.Services.GetConfiguration().GetSection("ElasticSearch"));
    }
}