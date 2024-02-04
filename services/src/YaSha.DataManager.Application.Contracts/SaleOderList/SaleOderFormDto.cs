using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YaSha.DataManager.SaleOderList
{
    public  class SaleOderFormDto
    {
        /// <summary>
        /// 单位转化文件
        /// </summary>
        public string ConvertFilePath { get; set; }

        /// <summary>
        /// 拆单表
        /// </summary>
        public string SplitFilePath { get; set; }

        /// <summary>
        /// 含产成品编码深化清单
        /// </summary>
        public string ProductCodeFilePath { get; set; }


        /// <summary>
        /// 重命名文件
        /// </summary>
        public string FileRename { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Notes { get; set; }


        /// <summary>
        /// 要求到货日期
        /// </summary>
        public string RequireDate { get; set; }


        /// <summary>
        /// 订单行类型
        /// </summary>
        public string OrderType { get; set; }


        /// <summary>
        /// 送达客户编码
        /// </summary>
        public string CustomerCode { get; set; }


    }
}
