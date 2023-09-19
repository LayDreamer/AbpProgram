using Volo.Abp.Domain.Entities.Auditing;

namespace YaSha.DataManager.ProductInventory.AggregateRoot;

public class ProductInventProduct : FullAuditedAggregateRoot<Guid>
{
    public Guid ParentId { get; set; }

    public ProductInventTree Parent { get; set; }

    public string Name { get; set; }

    public string Code { get; set; }

    public string Category { get; set; }

    public string Version { get; set; }

    public string Length { get; set; }

    public string Width { get; set; }

    public string Height { get; set; }

    public string MaterialQuality { get; set; }

    public string Color { get; set; }

    public string ProductSpecification { get; set; }

    public string ProcessNum { get; set; }

    public string AssemblyDrawingNum { get; set; }

    public string DetailNum { get; set; }

    public string Remark { get; set; }
    public string CreateUser { get; set; }

    public string ModifiyUser { get; set; }

    public string System { get; set; }

    public string Series { get; set; }

    public string ProjectCode { get; set; }

    public string ProjectName { get; set; }

    public string LimitInfos { get; set; }
    
    public ProductInventoryPublishStatus Status { get; set; }
    public List<ProductInventModule> Modules { get; set; }
    public List<ProductInventMaterial> Materials { get; set; }

    public ProductInventProduct()
    {
        Modules = new List<ProductInventModule>();
        Materials = new List<ProductInventMaterial>();
    }

    public void SetParentId(Guid id)
    {
        this.ParentId = id;
        if (this.Id == Guid.Empty)
            this.Id = Guid.NewGuid();
        foreach (var item in this.Materials)
        {
            item.SetParentProductId(this.Id);
        }

        foreach (var item in this.Modules)
        {
            item.SetParentId(this.Id);
        }
    }

    public void SetCopyName()
    {
        // this.Name = "(复制)" + this.Name;
        this.Code = "";
        foreach (var module in this.Modules)
        {
            module.SetCopyName();
        }

        foreach (var material in this.Materials)
        {
            material.SetCopyName();
        }
    }
}