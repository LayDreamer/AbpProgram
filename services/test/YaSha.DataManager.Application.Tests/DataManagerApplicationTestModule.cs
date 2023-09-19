using Volo.Abp.Modularity;

namespace YaSha.DataManager
{
    [DependsOn(
        typeof(DataManagerApplicationModule),
        typeof(DataManagerDomainTestModule)
        )]
    public class DataManagerApplicationTestModule : AbpModule
    {

    }
}