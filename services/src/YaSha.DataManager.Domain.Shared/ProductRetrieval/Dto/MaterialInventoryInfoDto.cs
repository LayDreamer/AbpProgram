﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Volo.Abp.Identity.Settings.IdentitySettingNames;

namespace YaSha.DataManager.ProductRetrieval.Dto
{
    public class MaterialInventoryCreateDto
    {
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// 仓库
        /// </summary>
        public string Warehouse { get; set; }

        /// <summary>
        /// 库位名称
        /// </summary>
        public string WarehouseLocationName { get; set; }

        /// <summary>
        /// 料单编码
        /// </summary>
        public string BillMaterialsCode { get; set; }

        /// <summary>
        /// 材料编码
        /// </summary>
        public string MaterialCode { get; set; }

        /// <summary>
        /// 材料名称
        /// </summary>
        public string MaterialName { get; set; }

        /// <summary>
        /// 生产批次
        /// </summary>
        public string ProductionBatch { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 库存数量
        /// </summary>
        public double InventoryQuantity { get; set; }

        /// <summary>
        /// 库存金额
        /// </summary>
        public double InventoryAmount { get; set; }

        /// <summary>
        /// 入库时间
        /// </summary>
        public string StorageTime { get; set; }
    }


    public class MaterialInventoryDto : MaterialInventoryCreateDto
    {
        public Guid Id { get; set; }
        public DateTime CreationTime { get; set; }
    }
}