using Volo.Abp.Domain.Entities.Auditing;

namespace YaSha.DataManager.ArchitectureList.AggregateRoot;

public class ArchitectureListFile : FullAuditedAggregateRoot<Guid>
{
    public Guid TreeId { get; set; }
    
    public ArchitectureListTree Tree { get; set; }
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public string FileEncryptionPath { get; set; }
    
    public string Data { get; set; }
    public ArchitectureListFileStatus Type { get; set; }

}