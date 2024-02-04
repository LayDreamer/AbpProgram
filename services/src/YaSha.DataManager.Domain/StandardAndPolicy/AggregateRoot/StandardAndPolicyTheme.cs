using Volo.Abp.Domain.Entities.Auditing;

namespace YaSha.DataManager.StandardAndPolicy.AggregateRoot;

public class StandardAndPolicyTheme : AuditedAggregateRoot<Guid>
{
    public Guid LibId { get; set; } 
    
    public StandardAndPolicyLib Lib { get; set; }
    
    public Guid TreeId { get; set; }

    public StandardAndPolicyTree Tree { get; set; }
}