namespace YaSha.DataManager.MaterialManage.Dto;

public class MaterialManageFullDto : MaterialManageDto
{
    /// <summary>
    /// 亚厦物料编码
    /// </summary>
    public string MaterialCode { get; set; }

    /// <summary>
    /// 生产批次
    /// </summary>
    public string ProductionBatch { get; set; }

    /// <summary>
    /// 入库时间
    /// </summary>
    public string StorageTime { get; set; }

    /// <summary>
    /// 批次库存
    /// </summary>
    public string BatchInventory { get; set; }
    
    /// <summary>
    /// 总库存
    /// </summary>
    public string TotalInventory { get; set; }
    
    /// <summary>
    /// 修改时间
    /// </summary>
    public DateTime? LastModificationTime { get; set; }
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreationTime { get; set; }
}