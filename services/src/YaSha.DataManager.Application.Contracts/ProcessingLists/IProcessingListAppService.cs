using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using YaSha.DataManager.FamilyLibs;
using YaSha.DataManager.ProductInventory.Dto;

namespace YaSha.DataManager.ProcessingLists
{
    public interface IProcessingListAppService :IApplicationService
    {
        Task<string> UploadExcel(IFormFile file);

        Task<PagedResultDto<ProcessingListDto>> GetAllListAsync(OrderNotificationSearchDto input);

        Task<byte[]> DownloadExcelAsync(Guid id);

        Task<ProcessingListDto> CreateDataFromFile(ProcessingListCreateDto input);

        Task DeleteFileAndData(Guid id );

        Task<string> SendEmail(Object json);

        Task<ProcessingListDto> GetAsync(Guid id);

        Task<PagedResultDto<ProcessingListDto>> GetListAsync(PagedAndSortedResultRequestDto input);
    }
}
