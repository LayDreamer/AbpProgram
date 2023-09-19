namespace YaSha.DataManager.DataDictionaryManagement
{
    [DependsOn(
        typeof(DataDictionaryManagementDomainSharedModule),
        typeof(AbpDddApplicationContractsModule),
        typeof(AbpAuthorizationModule)
        )]
    public class DataDictionaryManagementApplicationContractsModule : AbpModule
    {

    }
}
