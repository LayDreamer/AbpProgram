using Volo.Abp.Domain.Entities.Auditing;

namespace YaSha.DataManager.Domain
{
    public class DomainUser : AuditedAggregateRoot<Guid>
    {
      
        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 管理者是否将其添加域
        /// </summary>
        public bool IsDomain { get; set; }

    }
}
