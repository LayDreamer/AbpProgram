using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using YaSha.DataManager.ProcessingLists;
using YaSha.DataManager.StandardAndPolicy.Dto;

namespace YaSha.DataManager.MeasuringExcels
{
    public interface IMeasuringExcelAppService :
      ICrudAppService<
          MeasuringExcelDto,
          Guid,
          PagedAndSortedResultRequestDto>
    {

        Task<PagedResultDto<MeasuringExcelDto>> GetListAsync(PagedAndSortedResultRequestDto input);


        Task<ApiResultDto> UploadFile(IFormFile file);

        Task<ApiResultDto> AddAsync(Object input);

        Task<byte[]> DownloadExcelAsync(Guid id);

        Task DeleteAsync(Guid id);
    }



}
