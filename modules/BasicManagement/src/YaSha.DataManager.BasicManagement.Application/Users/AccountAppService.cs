using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IdentityModel;
using YaSha.DataManager.BasicManagement.ConfigurationOptions;
using YaSha.DataManager.BasicManagement.Users.Dtos;
using Microsoft.IdentityModel.Tokens;
using Volo.Abp.Security.Claims;
using LdapForNet;
using LdapForNet.Native;
using Microsoft.AspNetCore.Identity;
using IdentityUser = Volo.Abp.Identity.IdentityUser;
using Volo.Abp.Identity;
using YaSha.DataManager.BasicManagement.OrganizationUnits;
using static Volo.Abp.Identity.Settings.IdentitySettingNames;
using OrganizationUnit = Volo.Abp.Identity.OrganizationUnit;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using Volo.Abp.Identity.AspNetCore;
using System.Transactions;
using Volo.Abp.Uow;
using static System.Formats.Asn1.AsnWriter;
using Volo.Abp.Users;

namespace YaSha.DataManager.BasicManagement.Users
{
    public class AccountAppService : BasicManagementAppService, IAccountAppService
    {
        const string passwordSuffix = "2wE*";

        const string errorNameOrPassword = "用户名或密码错误！";
        const string noUser = "用户名不存在！";
        const string userLockOut = "用户被锁定！ 请稍后重新登录！";
        const string userNotAllowed = "用户不允许登录！";
        const string userNoRole = "请联系付雪斌添加角色！";
        const string contactAdmin = "请联系付雪斌申请激活账户！";
        const string serverException = "域验证服务器异常！";





        private readonly IdentityUserManager _userManager;
        private readonly JwtOptions _jwtOptions;
        private readonly Microsoft.AspNetCore.Identity.SignInManager<IdentityUser> _signInManager;
        private readonly DomainUserAppService _domainUserAppService;
        private readonly IOrganizationUnitRepository _organizationUnitRepository;

        public AccountAppService(
           DomainUserAppService domainUserAppService,
            IdentityUserManager userManager,
            IOptionsSnapshot<JwtOptions> jwtOptions,
            Microsoft.AspNetCore.Identity.SignInManager<IdentityUser> signInManager,
            IOrganizationUnitRepository organizationUnitRepository
            )
        {
            _userManager = userManager;
            _jwtOptions = jwtOptions.Value;
            _signInManager = signInManager;
            _domainUserAppService = domainUserAppService;
            _organizationUnitRepository = organizationUnitRepository;
        }

        /// <summary>
        /// 截取8位，用户可能输入工号或名字，默认以名字为密码
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetPassword(string name)
        {
            string mdsPassword = GetMD5_32(name).Substring(7, 8);
            mdsPassword = string.IsNullOrEmpty(mdsPassword) ? "123qwE*" : mdsPassword;
            return mdsPassword + passwordSuffix;
        }



        /// <summary>
        /// 域用户登录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        public async Task<LoginOutput> DomainLoginAsync(LoginInput input)
        {
            //截取8位，用户可能输入工号或名字，默认以名字为密码
            string testPassword = GetPassword(input.Name);

            var result = await DomainAuthenticate(input, testPassword);

            //当用户输入工号时，先查找到用户名，重新赋值密码
            testPassword = GetPassword(input.Name);
            var user = await _userManager.FindByNameAsync(input.Name);

            if (!result.Item1 && !result.Item2)
            {
                return new LoginOutput() { ErrorMsg = serverException };
            }
            //域用户验证失败：用户名错误或密码错误
            if (!result.Item1)
            {
                //密码输入错误，该步骤只是为了记录登录错次数，锁定用户
                var login = await LoginAsync(input);
                if (!string.IsNullOrEmpty(login.ErrorMsg))
                    return login;
                return new LoginOutput() { ErrorMsg = errorNameOrPassword };//碰巧猜对了密码时
            }
            //通过了域验证后
            if (user != null)
            {
                input.Password = testPassword;
                return await LoginAsync(input);
            }
            else
            {
                return await NewAuthenticate(input, testPassword);
            }
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetMD5_32(string input)
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] data = md5.ComputeHash(System.Text.Encoding.Default.GetBytes(input));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 管理员加域再校验
        /// </summary>
        /// <param name="input"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public async Task<LoginOutput> NewAuthenticate(LoginInput input, string newPassword)
        {
            var domainUserList = await _domainUserAppService.GetListAsync(new Volo.Abp.Application.Dtos.PagedAndSortedResultRequestDto());
            var findUser = domainUserList.Items.Where(e => e.UserName.Equals(input.Name)).FirstOrDefault();
            //当无法在新建的数据表中找到该用户，就记录下用户名，等待使用者找到管理者申请使用该系统
            if (findUser == null)
            {
                await _domainUserAppService.CreateAsync(new DomainUserDto
                {
                    UserName = input.Name
                });
                return new LoginOutput() { ErrorMsg = contactAdmin };
            }
            else
            {
                if (findUser.IsDomain)
                {
                    var userId = GuidGenerator.Create();
                    var user = new IdentityUser(userId, input.Name, input.Name + "@chinayasha.com", CurrentTenant.Id);
                    user.Name = input.Name;
                    await _userManager.CreateAsync(user, newPassword);
                    return await DomainLoginAsync(input);
                }
                else
                {
                    return new LoginOutput() { ErrorMsg = contactAdmin };
                }
            }
        }
        /// <summary>
        /// 域用户验证
        /// </summary>
        /// <param name="input"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>

        public async Task<(bool, bool)> DomainAuthenticate(LoginInput input, string newPassword)
        {
            bool result = false;
            bool connect = false;
            //四个服务器: 10.10.12.63   10.10.12.11    10.10.12.64  10.18.12.1
            //四个服务器 至少写2个, 写单台 域服务区有问题的时候,验证不了
            var server = "10.10.12.64";
            var serverPort = 389;
            var baseDc = "OU=亚厦工业化数据管理系统,OU=服务器集群区域,DC=chinayasha,DC=com";
            var newBaseDc = "OU=YASHA,DC=chinayasha,DC=com";
            var AdministratorName = "YSBDS";
            var AdministratorPassword = "ysgyhsjgl@20230817";

            using (var ldapConnection = new LdapConnection())
            {
                string dn = "";
                try
                {
                    ldapConnection.Connect(server, serverPort, Native.LdapSchema.LDAP, Native.LdapVersion.LDAP_VERSION3);
                    connect = true;
                }
                catch { }
                if (!connect)
                {
                    ldapConnection.Connect("10.10.12.11", serverPort, Native.LdapSchema.LDAP, Native.LdapVersion.LDAP_VERSION3);
                }
                try
                {
                    await ldapConnection.BindAsync(Native.LdapAuthType.Simple, new LdapCredential
                    {
                        UserName = $"CN={AdministratorName},{baseDc}",
                        Password = AdministratorPassword
                    });
                    connect = true;
                    var userEntry = new LdapEntry();
                    var searchResults = await ldapConnection.SearchAsync(newBaseDc, $"(&(sAMAccountName={input.Name}))");
                    if (searchResults.Count == 0)
                    {
                        searchResults = await ldapConnection.SearchAsync(newBaseDc, $"(&(employeeID={input.Name}))");
                        userEntry = searchResults.First();
                        input.Name = userEntry.Attributes["sAMAccountName"].FirstOrDefault();
                        newPassword = GetPassword(input.Name);
                    }
                    else
                    {
                        userEntry = searchResults.First();
                    }
                    await ldapConnection.BindAsync(Native.LdapAuthType.Simple, new LdapCredential
                    {
                        UserName = userEntry.Dn,
                        Password = input.Password
                    });
                    result = true;
                    dn = userEntry.Dn;
                }
                catch { }
                if (result)
                {
                    try
                    {
                        var findUser = await _userManager.FindByNameAsync(input.Name);
                        if (dn.Contains("OU=工业化,OU=亚厦装饰") || dn.Contains("OU=营销中心,OU=亚厦装饰"))
                        {
                            if (findUser == null)
                            {
                                var userId = GuidGenerator.Create();
                                var user = new IdentityUser(userId, input.Name, input.Name + "@chinayasha.com", CurrentTenant.Id);
                                await _userManager.CreateAsync(user, newPassword);
                            }
                        }
                        findUser = await _userManager.FindByNameAsync(input.Name);
                        if (findUser == null) return (result, connect);
                        #region 获取用户所在的组织
                        OrganizationUnit ou = null;
                        var ss = dn.Split(',').ToList();
                        ss = ss.Where(e => e.Contains("OU")).ToList();
                        ss.Reverse();
                        Guid parentId = Guid.Empty;
                        for (int i = 0; i < ss.Count; i++)
                        {
                            string name = ss[i].Replace("OU=", "");
                            if (parentId == Guid.Empty)
                            {
                                ou = await _organizationUnitRepository.GetAsync(name);
                                if (ou == null) continue;
                            }
                            else
                            {
                                var ous = await _organizationUnitRepository.GetChildrenAsync(parentId);
                                if (ous == null)
                                {
                                    ou = null;
                                    break;
                                }
                                ou = ous.Where(e => e.DisplayName.Equals(name)).FirstOrDefault();
                            }
                            if (ou == null) break;
                            parentId = ou.Id;
                        }
                        #endregion
                        if (ou != null)
                        {
                            #region  如果用户换了部门，就将其从之前的部门移除，并删除该用户所有角色
                            var userOus = await _userManager.GetOrganizationUnitsAsync(findUser);
                            if (userOus != null)
                            {
                                userOus = userOus.Where(e => e.Id != ou.Id).ToList();
                                if (userOus != null && userOus.Count > 0)
                                {
                                    foreach (var currOu in userOus)
                                    {
                                        await _userManager.RemoveFromOrganizationUnitAsync(findUser, currOu);
                                    }
                                    var oldRoles = await _userManager.GetRolesAsync(findUser);
                                    await _userManager.RemoveFromRolesAsync(findUser, oldRoles);
                                }
                            }
                            #endregion
                            //添加组织及角色
                            bool b = await _userManager.IsInOrganizationUnitAsync(findUser, ou);
                            if (!b)
                                await _userManager.AddToOrganizationUnitAsync(findUser, ou);
                            var roles = await _organizationUnitRepository.GetRolesAsync(ou);
                            foreach (var role in roles)
                            {
                                bool inRole = await _userManager.IsInRoleAsync(findUser, role.Name);
                                if (!inRole)
                                {
                                    await _userManager.AddToRoleAsync(findUser, role.Name);
                                }
                            }
                        }
                        else
                        {
                            var roles = await _userManager.GetRolesAsync(findUser);
                            if (roles.Count == 0)
                            {
                                await _userManager.AddToRoleAsync(findUser, "login-test");
                            }
                        }
                    }
                    catch { }
                }
            }
            return (result, connect);
        }
        public async Task<LoginOutput> LoginAsync(LoginInput input)
        {
            var user = await _userManager.FindByNameAsync(input.Name);
            if (user != null)
            {
                await _userManager.SetLockoutEnabledAsync(user, true);
                var result = await _signInManager.CheckPasswordSignInAsync(user, input.Password, true);

                if (result.IsLockedOut)
                {
                    return new LoginOutput() { ErrorMsg = userLockOut };
                }
                if (result.IsNotAllowed)
                {
                    return new LoginOutput() { ErrorMsg = userNotAllowed };
                }
                if (!result.Succeeded)
                {
                    return new LoginOutput() { ErrorMsg = errorNameOrPassword };
                }
            }
            else
            {
                return new LoginOutput() { ErrorMsg = noUser };
            }
            return await BuildResult(user);
        }



        #region 私有方法

        private async Task<LoginOutput> BuildResult(IdentityUser user)
        {
            if (!user.IsActive) return new LoginOutput() { ErrorMsg = userLockOut };
            var roles = await _userManager.GetRolesAsync(user);
            if (roles == null || roles.Count == 0)
                return new LoginOutput() { ErrorMsg = userNoRole };
            var token = GenerateJwt(user.Id, user.UserName, user.UserName, user.Email,
            user.TenantId.ToString(), roles.ToList());
            var loginOutput = ObjectMapper.Map<IdentityUser, LoginOutput>(user);
            loginOutput.Token = token;
            loginOutput.Roles = roles.ToList();
            return loginOutput;
        }

        /// <summary>
        /// 生成jwt token
        /// </summary>
        /// <returns></returns>
        private string GenerateJwt(Guid userId, string userName, string name, string email,
            string tenantId, List<string> roles)
        {
            var dateNow = Clock.Now;
            var expirationTime = dateNow.AddHours(_jwtOptions.ExpirationTime);
            var key = Encoding.ASCII.GetBytes(_jwtOptions.SecurityKey);

            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Audience, _jwtOptions.Audience),
                new Claim(JwtClaimTypes.Issuer, _jwtOptions.Issuer),
                new Claim(AbpClaimTypes.UserId, userId.ToString()),
                new Claim(AbpClaimTypes.Name, name),
                new Claim(AbpClaimTypes.UserName, userName),
                new Claim(AbpClaimTypes.Email, email),
                new Claim(AbpClaimTypes.TenantId, tenantId)
            };

            foreach (var item in roles)
            {
                claims.Add(new Claim(JwtClaimTypes.Role, item));
            }

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expirationTime, // token 过期时间
                NotBefore = dateNow,    // token 签发时间
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);
            return handler.WriteToken(token);
        }

        #endregion
    }
}