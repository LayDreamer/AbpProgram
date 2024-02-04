using YaSha.DataManager.BasicManagement.Users.Dtos;

namespace YaSha.DataManager.BasicManagement.Users
{
    public interface IAccountAppService: IApplicationService
    {
        /// <summary>
        /// 用户名密码登录
        /// </summary>
        Task<LoginOutput> LoginAsync(LoginInput input);

        /// <summary>
        /// 域用户登录
        /// </summary>
        Task<LoginOutput> DomainLoginAsync(LoginInput input);

    }
}
