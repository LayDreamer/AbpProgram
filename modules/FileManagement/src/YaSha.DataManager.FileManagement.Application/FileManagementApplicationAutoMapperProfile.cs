namespace YaSha.DataManager.FileManagement;

public class FileManagementApplicationAutoMapperProfile : Profile
{
    public FileManagementApplicationAutoMapperProfile()
    {
        CreateMap<YaSha.DataManager.FileManagement.Files.File, PagingFileOutput>();
    }
}