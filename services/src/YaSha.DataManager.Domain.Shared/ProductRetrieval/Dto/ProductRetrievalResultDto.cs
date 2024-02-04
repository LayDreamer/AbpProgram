using YaSha.DataManager.ProductInventory;

namespace YaSha.DataManager.ProductRetrieval.Dto;

public class ProductRetrievalResultDto
{
    public ProductRetrievalResultDto( string productName, string productCode, string moduleName, string moduleCode, string materialName, string materialCode)
    {
        Id = productCode + "-" + moduleCode + "-" + materialCode;
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

    public string Id { get; set; }

    public string MaterialCount { get; set; }
    public string MaterialMoney { get; set; }

    public ProductInventroyTag Tag { get; set; }

    public string ProductName { get; set; }

    public string ProductCode { get; set; }

    public string ModuleName { get; set; }

    public string ModuleCode { get; set; }

    public string MaterialName { get; set; }

    public string MaterialCode { get; set; }
}