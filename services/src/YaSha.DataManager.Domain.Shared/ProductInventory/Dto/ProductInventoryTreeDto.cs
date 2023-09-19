namespace YaSha.DataManager.ProductInventory.Dto;

public class ProductInventoryTreeDto
{
    public Guid Id { get; set; }
    
    public Guid? ParentId { get; set; }

    public ProductInventoryTreeDto Parent { get; set; }
    
    public string Name { get; set; }
    
    public List<ProductInventoryTreeDto> Children { get; set; }

    public string Remark { get; set; }
    
    public ProductInventoryTreeDto()
    {
        Children = new List<ProductInventoryTreeDto>();
    }
}