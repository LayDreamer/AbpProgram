using Volo.Abp.Domain.Entities.Auditing;

namespace YaSha.DataManager.ArchitectureList.AggregateRoot;

public class ArchitectureListTree: FullAuditedAggregateRoot<Guid>
{
    public Guid? ParentId { get; set; }
    
    public ArchitectureListTree Parent { get; set; }
    public List<ArchitectureListTree> Children { get; set; }
    public string Name { get; set; }
    
    public string Data { get; set; }
    public int Code { get; set; }
    
    public ArchitectureListTree()
    {
        Children = new List<ArchitectureListTree>();
    }
    
    public ArchitectureListTree(Guid id, string name, int code, ArchitectureListTree parent) : base(id)
    {
        Children = new List<ArchitectureListTree>();
        parent?.Children.Add(this);
        ParentId = parent?.Id;
        Name = name;
        Code = code;
    }
}