namespace YaSha.DataManager.ArchitectureList.Dto;

public class ArchitectureListFileDto
{
    public Guid Id { get; set; }
    
    public string FileName { get; set; }
    public string FilePath { get; set; }
    
    public string FileEncryptionPath { get; set; }
    
    public ArchitectureListFileStatus Type { get; set; }
}