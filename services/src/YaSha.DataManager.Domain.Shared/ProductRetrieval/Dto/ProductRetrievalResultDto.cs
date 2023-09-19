using YaSha.DataManager.ProductInventory;
using YaSha.DataManager.ProductInventory.AggregateRoot;
using YaSha.DataManager.ProductInventory.Dto;

namespace YaSha.DataManager.ProductRetrieval.Dto;

public class ProductRetrievalResultDto
{
    public ProductRetrievalResultDto(string productName, string productCode, string moduleName, string moduleCode, string materialName, string materialCode)
    {
        ProductName = productName;
        ProductCode = productCode;
        ModuleName = moduleName;
        ModuleCode = moduleCode;
        MaterialName = materialName;
        MaterialCode = materialCode;
    }

    public ProductRetrievalResultDto()
    {
        
    }
    
    public ProductInventroyTag Tag { get; set; }

    public string ProductName { get; private set; }
    
    public string ProductCode { get; private set; }
    
    public string ModuleName { get; private set; }
    
    public string ModuleCode { get; private set; }
    
    public string MaterialName { get; private set; }
    
    public string MaterialCode { get; private set; }
}