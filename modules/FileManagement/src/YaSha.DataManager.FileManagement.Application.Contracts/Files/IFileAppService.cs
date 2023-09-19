using System.Collections.Generic;
using System.Threading.Tasks;
using YaSha.DataManager.FileManagement.Files.Dto;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace YaSha.DataManager.FileManagement.Files;

public interface IFileAppService : IApplicationService
{
    Task<FileTokenOutput> GetFileTokenAsync();
    Task CreateAsync(CreateFileInput input);

    Task<PagedResultDto<PagingFileOutput>> PagingAsync(PagingFileInput input);
}