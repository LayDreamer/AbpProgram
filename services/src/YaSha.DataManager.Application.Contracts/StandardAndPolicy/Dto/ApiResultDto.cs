namespace YaSha.DataManager.StandardAndPolicy.Dto;

public class ApiResultDto
{
    public int Code { get; set; }
    
    public string Error { get; set; }
    
    public string Message { get; set; }
    
    public bool Success { get; set; }
    
    public Object Data { get; set; }
}