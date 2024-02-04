using Volo.Abp.Domain.Entities.Auditing;

namespace YaSha.DataManager.MaterialManage.AggregateRoot;

public class MaterialManageInfo : FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// 材料状态
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    /// 材料图片
    /// </summary>
    public string MaterialImageUrl { get; set; }

    /// <summary>
    /// 材料图片下载地址
    /// </summary>
    public string MaterialImageDownLoadUrl { get; set; }
    
    /// <summary>
    /// 百度图库Sign
    /// </summary>
    public string BaiduSign { get; set; }
    
    /// <summary>
    /// 材质类型
    /// </summary>
    public string MaterialType { get; set; }

    /// <summary>
    /// 材质纹理
    /// </summary>
    public string MaterialTexture { get; set; }

    /// <summary>
    /// 表面质感
    /// </summary>
    public string MaterialSurface { get; set; }

    /// <summary>
    /// 顺序码
    /// </summary>
    public string SequenceCode { get; set; }

    /// <summary>
    /// 封样样品存放处
    /// </summary>
    public string StoragePlace { get; set; }

    /// <summary>
    /// 宽幅
    /// </summary>
    public string Width { get; set; }

    /// <summary>
    /// 厚度
    /// </summary>
    public string Thickness { get; set; }

    /// <summary>
    /// 重量
    /// </summary>
    public string Weight { get; set; }

    /// <summary>
    /// 每卷长度
    /// </summary>
    public string Length { get; set; }

    /// <summary>
    /// 供应商
    /// </summary>
    public string Supplier { get; set; }
    
    /// <summary>
    /// 供应商简称 
    /// </summary>
    public string SupplierOverview { get; set; }
    
    /// <summary>
    /// 供应商编码
    /// </summary>
    public string SupplierCode { get; set; }

    /// <summary>
    /// 价格区间
    /// </summary>
    public string Price { get; set; }

    /// <summary>
    /// 起订量
    /// </summary>
    public string Quantity { get; set; }

    /// <summary>
    /// 供货周期
    /// </summary>
    public string DeliveryCycle { get; set; }

    /// <summary>
    /// 样册系列名称
    /// </summary>
    public string SeriesName { get; set; }

    /// <summary>
    /// 样册系列照片
    /// </summary>
    public string SeriesImageUrl { get; set; }

    /// <summary>
    /// 样册系列编码
    /// </summary>
    public string SeriesCode { get; set; }
    
    /// <summary>
    /// 操作用户
    /// </summary>
    public string OperatingUser { get; set; }
}