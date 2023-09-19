using Volo.Abp.Testing;

namespace YaSha.DataManager.ElasticSearch
{

    public abstract class DataManagerElasticSearchTestBase : AbpIntegratedTest<DataManagerElasticSearchTestBaseModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}
