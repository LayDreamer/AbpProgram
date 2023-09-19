namespace YaSha.DataManager.ProductInventory.Dto;

public class ProductInventoryFullDto
{
    public Guid? ParentId { get; set; }

    public Guid? Id => this.Data?.Id;
    public ProductInventroyTag ParentTag { get; set; } = ProductInventroyTag.Undefined;
    public ProductInventroyTag Tag { get; set; } = ProductInventroyTag.Undefined;
    public List<ProductInventoryFullDto> Children { get; set; }
    public ProductInventoryModifyStatus Status { get; set; } = ProductInventoryModifyStatus.Normal;
    public ProductInventoryEditDto Data { get; set; } = null;
    public ProductInventoryFullDto()
    {
        Children = new List<ProductInventoryFullDto>();
    }
    public ProductInventoryFullDto AddModule(ProductInventoryEditDto data)
    {
        var child = new ProductInventoryFullDto()
        {
            ParentId = this.Data.Id,
            ParentTag = this.Tag,
            Tag = ProductInventroyTag.Modules,
            Data = data
        };
        this.Children.Add(child);
        return child;
    }
    
    public ProductInventoryFullDto AddMaterial(ProductInventoryEditDto data)
    {
        var child = new ProductInventoryFullDto()
        {
            ParentId = this.Data.Id,
            ParentTag = this.Tag,
            Tag = ProductInventroyTag.Material,
            Data = data
        };
        this.Children.Add(child);
        return child;
    }
}