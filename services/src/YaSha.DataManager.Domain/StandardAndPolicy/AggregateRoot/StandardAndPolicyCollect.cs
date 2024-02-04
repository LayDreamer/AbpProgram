using Volo.Abp.Domain.Entities.Auditing;

namespace YaSha.DataManager.StandardAndPolicy.AggregateRoot;

public class StandardAndPolicyCollect : AuditedAggregateRoot<Guid>
{
    public Guid UserId { get; set; }

    public Guid LibId { get; set; }

    public bool Collect { get; set; }
}