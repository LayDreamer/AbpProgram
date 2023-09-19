namespace YaSha.DataManager.BasicManagement.EntityFrameworkCore;

[ConnectionStringName(BasicManagementDbProperties.ConnectionStringName)]
public interface IBasicManagementDbContext : 
    IEfCoreDbContext,     
    IFeatureManagementDbContext,
    IIdentityDbContext,
    IPermissionManagementDbContext,
    ISettingManagementDbContext,
    ITenantManagementDbContext,
    IBackgroundJobsDbContext,
    IAuditLoggingDbContext
{
    
}
