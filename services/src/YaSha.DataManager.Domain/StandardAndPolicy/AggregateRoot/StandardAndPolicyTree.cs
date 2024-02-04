using Volo.Abp.Domain.Entities.Auditing;
using YaSha.DataManager.ProductInventory.AggregateRoot;

namespace YaSha.DataManager.StandardAndPolicy.AggregateRoot;
public class StandardAndPolicyTree : FullAuditedAggregateRoot<Guid>
{
    public StandardAndPolicyTree()
    {
        Children = new List<StandardAndPolicyTree>();
    }
    
    public StandardAndPolicyTree(Guid id, string name, StandardAndPolicyTree parent) : base(id)
    {
        Children = new List<StandardAndPolicyTree>();
        parent?.AddChild(this);
        ParentId = parent?.Id;
        Name = name;
    }

    public Guid? ParentId { get; set; }
    
    public int Code { get; set; }
    
    public bool Hide { get; set; }
    
    public StandardAndPolicyTree Parent { get; set; }

    public List<StandardAndPolicyTree> Children { get; set; }

    public string Name { get; set; }

    public void AddChild(StandardAndPolicyTree tree)
    {
        tree.Code = this.Children.Count() + 1;
        this.Children.Add(tree);
    }
}