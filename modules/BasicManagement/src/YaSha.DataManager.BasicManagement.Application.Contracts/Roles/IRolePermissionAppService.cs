using YaSha.DataManager.BasicManagement.Roles.Dtos;

namespace YaSha.DataManager.BasicManagement.Roles
{
    public interface IRolePermissionAppService : IApplicationService
    {
        
        Task<PermissionOutput> GetPermissionAsync(GetPermissionInput input);

        Task UpdatePermissionAsync(UpdateRolePermissionsInput input);
    }
}