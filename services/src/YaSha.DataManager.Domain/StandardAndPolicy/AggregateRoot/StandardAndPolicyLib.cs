using Volo.Abp.Domain.Entities.Auditing;
using YaSha.DataManager.ProductInventory.AggregateRoot;

namespace YaSha.DataManager.StandardAndPolicy.AggregateRoot;

public class StandardAndPolicyLib : AuditedAggregateRoot<Guid>
{
    /// <summary>
    /// 编号
    /// </summary>
    public string Number { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 行业
    /// </summary>
    public string Industry { get; set; }

    /// <summary>
    /// 发布单位
    /// </summary>
    public string PublishingUnit { get; set; }

    /// <summary>
    /// 发布日期
    /// </summary>
    public DateTime PublishingDate { get; set; }

    /// <summary>
    /// 实施日期
    /// </summary>
    public DateTime ImplementationDate { get; set; }

    /// <summary>
    /// 图片地址
    /// </summary>
    public string ImagePath { get; set; }

    /// <summary>
    /// PDF地址
    /// </summary>
    public string PdfPath { get; set; }

    /// <summary>
    /// 原文链接
    /// </summary>
    public string LinkPath { get; set; }


    /// <summary>
    /// 标准/政策
    /// </summary>
    public string Type { get; set; }


    /// <summary>
    /// 状态
    /// </summary>
    public StandardAndPolicyStatus Status { get; set; }


    /// <summary>
    /// 发文字号
    /// </summary>
    public string DispatchFont { get; set; }

    
    /// <summary>
    /// 失效日期
    /// </summary>
    public DateTime LoseDate { get; set; }
    
    
    /// <summary>
    /// 标准
    /// </summary>
    public StandardAndPolicyCategory StandardCategory { get; set; }
}