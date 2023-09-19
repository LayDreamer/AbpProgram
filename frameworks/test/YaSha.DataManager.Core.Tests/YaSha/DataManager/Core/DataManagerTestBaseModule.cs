using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace YaSha.DataManager.Core
{
    [DependsOn(typeof(AbpTestBaseModule),
        typeof(AbpAutofacModule))]
    public class DataManagerTestBaseModule : AbpModule
    {
    }
}