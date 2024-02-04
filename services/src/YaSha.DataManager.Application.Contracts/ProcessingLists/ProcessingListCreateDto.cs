using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YaSha.DataManager.ProcessingLists
{
    public class ProcessingListCreateDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// 项目编码
        /// </summary>
        public string ProjectCode { get; set; }


        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }


        /// <summary>
        /// 文件编码
        /// </summary>
        public string FileCode { get; set; }


        /// <summary>
        /// 文件地址
        /// </summary>
        public string FileAddress { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        public string Creator { get; set; }

      
    }
}
