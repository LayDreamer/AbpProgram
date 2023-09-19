using Volo.Abp.Domain.Entities.Auditing;

namespace YaSha.DataManager.ProductInventory.AggregateRoot;

public class ProductInventMaterial : FullAuditedAggregateRoot<Guid>
{
    public Guid? ProductId { get; set; }

    public ProductInventProduct Product { get; set; }

    public Guid? ModuleId { get; set; }

    public ProductInventModule Module { get; set; }

    public string Name { get; set; }

    public string Code { get; set; }

    public string Length { get; set; }

    public string Width { get; set; }

    public string Height { get; set; }

    public string SupplyMode { get; set; }

    public string ModuleSpecification { get; set; }

    public string Property { get; set; }

    public string MaterialQuality { get; set; }

    public string Color { get; set; }

    public string Usage { get; set; }

    public string Unit { get; set; }

    public string IsProcess { get; set; }

    public string System { get; set; }

    public string Series { get; set; }
    
    public string Remark { get; set; }

    public string MaterialUsageFormula { get; set; }
    
    public void SetParentProductId(Guid id)
    {
        this.ProductId = id;
        if (this.Id == Guid.Empty)
            this.Id = Guid.NewGuid();
    }

    public void SetParentModuleId(Guid id)
    {
        this.ModuleId = id;
        if (this.Id == Guid.Empty)
            this.Id = Guid.NewGuid();
    }

    public void SetCopyName()
    {
        this.Code = "";
        // this.Name = "(复制)" + this.Name;
    }
}