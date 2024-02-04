using System.Text;
using Newtonsoft.Json;

namespace YaSha.DataManager.BasicManagement.Systems
{
    public class AccountController : BasicManagementController, IAccountAppService
    {
        private readonly IAccountAppService _accountAppService;

        public AccountController(IAccountAppService accountAppService)
        {
            _accountAppService = accountAppService;
        }

        [SwaggerOperation(summary: "域用户登录", Tags = new[] { "Account" })]
        public Task<LoginOutput> DomainLoginAsync(LoginInput input)
        {
            return _accountAppService.DomainLoginAsync(input);
        }

        [SwaggerOperation(summary: "账号密码登录", Tags = new[] { "Account" })]
        public Task<LoginOutput> LoginAsync(LoginInput input)
        {
            return _accountAppService.LoginAsync(input);
        }
    }
}