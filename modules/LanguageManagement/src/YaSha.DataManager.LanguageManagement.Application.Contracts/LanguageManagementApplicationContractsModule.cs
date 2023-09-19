namespace YaSha.DataManager.LanguageManagement
{
    [DependsOn(
        typeof(LanguageManagementDomainSharedModule),
        typeof(AbpDddApplicationContractsModule),
        typeof(AbpAuthorizationModule)
        )]
    public class LanguageManagementApplicationContractsModule : AbpModule
    {

    }
}
