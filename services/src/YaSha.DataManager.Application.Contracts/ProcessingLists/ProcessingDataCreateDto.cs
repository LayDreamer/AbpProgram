using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YaSha.DataManager.ProcessingLists
{
    public  class ProcessingDataCreateDto
    {
        
        /// <summary>
        /// 文件id
        /// </summary>
        public Guid? FileId { get; set; }


        /// <summary>
        /// 产品id
        /// </summary>
        public Guid? ProductId { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 产品编码
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// 产品长
        /// </summary>
        public string ProductLength { get; set; }

        /// <summary>
        /// 产品宽
        /// </summary>
        public string ProductWidth { get; set; }

        /// <summary>
        /// 产品高
        /// </summary>
        public string ProductHeight { get; set; }



        /// <summary>
        /// 模块名称
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// 模块编码
        /// </summary>
        public string ModuleCode { get; set; }

        /// <summary>
        /// 模块长
        /// </summary>
        public string ModuleLength { get; set; }

        /// <summary>
        /// 模块宽
        /// </summary>
        public string ModuleWidth { get; set; }

        /// <summary>
        /// 模块高
        /// </summary>
        public string ModuleHeight { get; set; }

        /// <summary>
        /// 物料名称
        /// </summary>
        public string MaterialName { get; set; }

        /// <summary>
        /// 物料编码
        /// </summary>
        public string MaterialCode { get; set; }

        /// <summary>
        /// 物料长
        /// </summary>
        public string MaterialLength { get; set; }

        /// <summary>
        /// 物料宽
        /// </summary>
        public string MaterialWidth { get; set; }

        /// <summary>
        /// 物料高
        /// </summary>
        public string MaterialHeight { get; set; }

        /// <summary>
        /// 物料单位
        /// </summary>
        public string MaterialUnit { get; set; }

        /// <summary>
        /// 额外的数据
        /// </summary>
        public string Data { get; set; }


        public ProcessingDataCreateDto(Guid? fileId, Guid? productId, string pName, string pCode, string pL, string pW, string pH,
       string mName, string mCode, string mL, string mW, string mH, string wName, string wCode, string wL, string wW, string wH, string wUnit)
        {
            FileId = fileId;
            ProductId = productId;
            ProductName = pName;
            ProductCode = pCode;
            ProductLength = pL;
            ProductWidth = pW;
            ProductHeight = pH;
            ModuleName = mName;
            ModuleCode = mCode;
            ModuleLength = mL;
            ModuleWidth = mW;
            ModuleHeight = mH;
            MaterialName = wName;
            MaterialCode = wCode;
            MaterialLength = wL;
            MaterialWidth = wW;
            MaterialHeight = wH;
            MaterialUnit = wUnit;
        }

    }
}
