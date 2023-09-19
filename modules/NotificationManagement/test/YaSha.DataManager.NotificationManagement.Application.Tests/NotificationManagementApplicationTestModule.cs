namespace YaSha.DataManager.NotificationManagement
{
    [DependsOn(
        typeof(NotificationManagementApplicationModule),
        typeof(NotificationManagementDomainTestModule)
        )]
    public class NotificationManagementApplicationTestModule : AbpModule
    {

    }
}
