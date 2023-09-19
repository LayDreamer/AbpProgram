using Volo.Abp;
using Volo.Abp.Modularity;

namespace YaSha.DataManager
{

    [DependsOn(typeof(DataManagerLocalizationModule))]
    [DependsOn(typeof(AbpTestBaseModule))]
    public class DataManagerLocalizationTestBaseModule : AbpModule
    {
    }
}
