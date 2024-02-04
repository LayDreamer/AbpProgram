using Microsoft.AspNetCore.Http;

namespace YaSha.DataManager.ProductInventory.Dto;

public class ImageFileDto
{
    public Guid Id { get; set; }

    public IFormFile File { get; set; }
    
}

public class ImageResponseDto
{
    public int Code { get; set; }
    
    public string ServerPath { get; set; }
    
    public string Error { get; set; }
}