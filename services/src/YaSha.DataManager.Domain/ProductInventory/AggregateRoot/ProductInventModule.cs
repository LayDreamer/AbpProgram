using Volo.Abp.Domain.Entities.Auditing;

namespace YaSha.DataManager.ProductInventory.AggregateRoot;

public class ProductInventModule : FullAuditedAggregateRoot<Guid>
{
    public Guid ParentId { get; set; }

    public ProductInventProduct Parent { get; set; }

    public string Level { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }

    public string Category { get; set; }

    public string Version { get; set; }

    public string Length { get; set; }

    public string Width { get; set; }

    public string Height { get; set; }

    public string SupplyMode { get; set; }

    public string System { get; set; }

    public string Series { get; set; }
    
    public string ModuleSpecification { get; set; }

    public string LimitInfos { get; set; }

    public List<ProductInventMaterial> Materials { get; set; }
    
    public ProductInventModule(Guid id) : base(id)
    {
        Materials = new List<ProductInventMaterial>();
    }
    
    public ProductInventModule()
    {
        Materials = new List<ProductInventMaterial>();
    }

    public void SetParentId(Guid id)
    {
        this.ParentId = id;
        if (this.Id == Guid.Empty)
            this.Id = Guid.NewGuid();
        foreach (var item in this.Materials)
        {
            item.SetParentModuleId(this.Id);
        }
    }

    public void SetCopyName()
    {
        // this.Name = "(复制)" + this.Name;
        this.Code = "";
        foreach (var material in this.Materials)
        {
            material.SetCopyName();
        }
    }
}