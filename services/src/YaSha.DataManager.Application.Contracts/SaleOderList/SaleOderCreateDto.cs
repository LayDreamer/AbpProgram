using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YaSha.DataManager.SaleOderList
{
    public  class SaleOderCreateDto
    {
        public string Name { get; set; }

        /// <summary>
        /// 文件存储地址
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 上传用户
        /// </summary>
        public string UploadUser { get; set; }




    }
}
