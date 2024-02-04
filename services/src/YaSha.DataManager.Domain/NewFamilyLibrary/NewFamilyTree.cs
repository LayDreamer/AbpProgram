using EasyAbp.Abp.Trees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace YaSha.DataManager.NewFamilyLibrary
{
    public class NewFamilyTree : AuditedAggregateRoot<Guid>
    {
        public NewFamilyTree()
        {
            Children = new List<NewFamilyTree>();
        }

        /// <summary>
        /// 父级Id
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 层级编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 层级
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 父级
        /// </summary>
        public NewFamilyTree Parent { get; set; }

        /// <summary>
        /// 子集
        /// </summary>
        public List<NewFamilyTree> Children { get; set; }

        
        public string DisplayName { get; set; }

    }
}
