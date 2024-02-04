using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YaSha.DataManager.ProductRetrieval.Dto
{
    public class ExportProductOutput
    {
        //[ExporterHeader(DisplayName = "产品名称")]
        public string ProductName { get; set; }

        //[ExporterHeader(DisplayName = "产品编码")]
        public string ProductCode { get; set; }

        //[ExporterHeader(DisplayName = "模块名称")]
        public string ModuleName { get; set; }

        //[ExporterHeader(DisplayName = "模块编码")]
        public string ModuleCode { get; set; }

        //[ExporterHeader(DisplayName = "物料名称")]
        public string MaterialName { get; set; }

        //[ExporterHeader(DisplayName = "物料编码")]
        public string MaterialCode { get; set; }

        //[ExporterHeader(DisplayName = "下市物料\r\n库存数量")]
        public string InventoryCount { get; set; }

        //[ExporterHeader(DisplayName = "下市物料\r\n库存金额")]
        public string InventoryMoney { get; set; }

        //[ExporterHeader(DisplayName = "序号")]
        public string Number { get; set; }
        

        public ExportProductOutput(string number,string productName, string productCode, string moduleName, string moduleCode, string materialName, string materialCode,
            string inventoryCount, string inventoryMoney)
        {
            Number= number;
            ProductName = productName;
            ProductCode = productCode;
            ModuleName = moduleName;
            ModuleCode = moduleCode;
            MaterialName = materialName;
            MaterialCode = materialCode;
            InventoryCount= inventoryCount;
            InventoryMoney= inventoryMoney;
        }

    }

    public class ProductInfo
    {
        public List<ExportProductOutput> ExportProductOutputs { get; set; }
        public ProductInfo(List<ExportProductOutput> exportProductOutputs)
        {
            ExportProductOutputs=exportProductOutputs;
        }
    }



}
