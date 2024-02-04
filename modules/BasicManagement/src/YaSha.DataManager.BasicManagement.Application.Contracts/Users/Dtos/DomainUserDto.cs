using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YaSha.DataManager.BasicManagement.Users.Dtos
{
    public class DomainUserDto : AuditedEntityDto<Guid>
    {
        /// <summary>
        ///id
        /// </summary>
        public Guid id { get; set; }
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
