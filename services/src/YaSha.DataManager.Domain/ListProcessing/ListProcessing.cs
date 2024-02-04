using Volo.Abp.Domain.Entities.Auditing;

namespace YaSha.DataManager.ListProcessing;

public class ListProcessing : AuditedAggregateRoot<Guid>
{
    public string Name { get; set; }

    /// <summary>
    /// 文件存储地址
    /// </summary>
    public string FilePath { get; set; }
}