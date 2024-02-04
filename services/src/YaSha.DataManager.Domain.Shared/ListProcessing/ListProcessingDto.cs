using Volo.Abp.Application.Dtos;

namespace YaSha.DataManager.ListProcessing;

public class ListProcessingDto : AuditedEntityDto<Guid>
{
    public string Name { get; set; }

    /// <summary>
    /// 文件存储地址
    /// </summary>
    public string FilePath { get; set; } 
}