using System.Runtime.CompilerServices;
using Volo.Abp.Domain.Entities.Auditing;

namespace YaSha.DataManager.ProductInventory.AggregateRoot;

public class ProductInventTree : FullAuditedAggregateRoot<Guid>
{
    public ProductInventTree()
    {
        Children = new List<ProductInventTree>();
        Products = new List<ProductInventProduct>();
    }
    
    public ProductInventTree(Guid id, string name, ProductInventTree parent) : base(id)
    {
        Children = new List<ProductInventTree>();
        parent?.Children.Add(this);
        ParentId = parent?.Id;
        Name = name;
    }

    public Guid? ParentId { get; set; }


    public ProductInventTree Parent { get; set; }

    public List<ProductInventTree> Children { get; set; }

    public string Name { get; set; }

    public List<ProductInventProduct> Products { get; set; }

    public string Remark { get; set; }
}