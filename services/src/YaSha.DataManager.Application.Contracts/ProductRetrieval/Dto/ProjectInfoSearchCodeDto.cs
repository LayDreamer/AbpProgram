namespace YaSha.DataManager.ProductRetrieval.Dto;

public class ProjectInfoSearchCodeDto
{
    public string Code { get; set; }
    
    public ProjectInfoInputType Type { get; set; }


    public string CalculateCacheKey()
    {
        var results = "_" + this.Code + "_" + this.Type;
        return results;
    }
}