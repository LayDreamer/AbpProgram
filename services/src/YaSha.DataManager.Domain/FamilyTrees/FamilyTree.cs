using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using EasyAbp.Abp.Trees;

namespace YaSha.DataManager.FamilyTrees
{
    public class FamilyTree : AuditedAggregateRoot<Guid>, ITree<FamilyTree>
    {
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
        public FamilyTree Parent { get; set; }

        /// <summary>
        /// 子集
        /// </summary>
        public ICollection<FamilyTree> Children { get; set; }

        public string DisplayName { get; set; }

        //public string BlobName { get; set; }
    }
}
