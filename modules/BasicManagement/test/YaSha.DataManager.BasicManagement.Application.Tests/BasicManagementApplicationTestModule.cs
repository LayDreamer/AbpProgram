using Volo.Abp.Modularity;

namespace YaSha.DataManager.BasicManagement;

[DependsOn(
    typeof(BasicManagementApplicationModule),
    typeof(BasicManagementDomainTestModule)
    )]
public class BasicManagementApplicationTestModule : AbpModule
{

}
