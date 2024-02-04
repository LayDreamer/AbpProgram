using Volo.Abp.Domain.Entities.Auditing;

namespace YaSha.DataManager.ArchitectureList.AggregateRoot;

public class ArchitectureListModuleFile : AuditedAggregateRoot<Guid>
{
    public Guid ModuleId { get; set; }
    public Guid FileId { get; set; }
    
    public string Data { get; set; }
    public ArchitectureListFile File { get; set; }
    
}