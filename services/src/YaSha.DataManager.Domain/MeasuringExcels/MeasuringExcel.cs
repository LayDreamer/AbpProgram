using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace YaSha.DataManager.MeasuringExcels
{
  
    public class MeasuringExcel : AuditedAggregateRoot<Guid>
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件地址
        /// </summary>
        public string FileAddress { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// 设计允许偏差值
        /// </summary>
        public string designDiffer { get; set; }

        /// <summary>
        /// 产品安装最小空腔值
        /// </summary>
        public string minCavityValue { get; set; }

    }
}
