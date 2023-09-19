using Volo.Abp;
using Volo.Abp.Testing;

namespace YaSha.DataManager
{

    public abstract class DataManagerLocalizationTestBase : AbpIntegratedTest<DataManagerLocalizationTestBaseModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}
