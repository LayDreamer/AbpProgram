namespace YaSha.DataManager.ProductRetrieval.Dto;

public class ProjectInfoCreateDto
{
    /// <summary>
    /// 项目名称
    /// </summary>
    public string ProjectName { get; set; }

    /// <summary>
    /// 项目编码
    /// </summary>
    public string ProjectCode { get; set; }

    /// <summary>
    /// 产品编码
    /// </summary>
    public string ProductCode { get; set; }

    /// <summary>
    /// 模块编码
    /// </summary>
    public string ModuleCode { get; set; }

    /// <summary>
    /// 物料编码
    /// </summary>
    public string MaterialCode { get; set; }
}

public class ProjectInfoDto : ProjectInfoCreateDto
{
    public Guid Id { get; set; }
    public DateTime CreationTime { get; set; }
}