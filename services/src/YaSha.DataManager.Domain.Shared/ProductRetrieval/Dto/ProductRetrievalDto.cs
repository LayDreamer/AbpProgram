using YaSha.DataManager.ProductInventory;

namespace YaSha.DataManager.ProductRetrieval.Dto;

public class ProductRetrievalDto
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public string Code { get; set; }
    public ProductInventroyTag Tag { get; set; }
    
    public ProductRetrievalDto()
    {
        
    }
}