using EasyAbp.Abp.Trees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace YaSha.DataManager.FamilyLibs
{
    public class FamilyLib : AuditedAggregateRoot<Guid>, ITree<FamilyLib>
    {
        /// <summary>
        /// 状态 (停用/启用)
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 分类Id
        /// </summary>
        public Guid CategoryId { get; set; }

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
        public FamilyLib Parent { get; set; }

        /// <summary>
        /// 子集
        /// </summary>
        public ICollection<FamilyLib> Children { get; set; }


        public string DisplayName { get; set; }


        /// <summary>
        /// 编码
        /// </summary>
        public string Number { get; set; }

        public double Length { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 上传用户
        /// </summary>
        public string UploadUser { get; set; }

        /// <summary>
        /// 文件存储地址
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 图片存储地址
        /// </summary>
        public string ImagePath { get; set; }

        /// <summary>
        /// 数据描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 补充数据
        /// </summary>
        public string ExternalData { get; set; }

        /// <summary>
        /// 层级结构
        /// </summary>
        public string Hierarchy { get; set; }
    }
}
