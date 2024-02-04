using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace YaSha.DataManager.ProductRetrieval.AggregateRoot
{
    public  class ProjectInfo : Entity<Guid>
    {
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }
        
        /// <summary>
        /// 项目编码
        /// </summary>
        public string ProjectCode { get; set; }
        
        /// <summary>
        /// 产品编码
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// 模块编码
        /// </summary>
        public string ModuleCode { get; set; }

        /// <summary>
        /// 物料编码
        /// </summary>
        public string MaterialCode { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }
        
        public void SetId()
        {
            this.Id = Guid.NewGuid();
            this.CreationTime = DateTime.Now;
        }
    }
}
