using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using YaSha.DataManager.MeasuringExcels;
using YaSha.DataManager.ProductInventory.Dto;
using YaSha.DataManager.StandardAndPolicy.Dto;

namespace YaSha.DataManager.SaleOderList
{
    public  interface ISaleOderListAppService : IApplicationService
    {

        Task<ApiResultDto> UploadFactoryFile(IFormFile file);


        string CreateNewSplitFile(SaleOderFormDto input);


        Task<ApiResultDto> CreateSaleOderFile(object json);


        Task<PagedResultDto<SaleOderDto>> GetListAsync(PagedAndSortedResultRequestDto input);

        Task<SaleOderDto> GetAsync(Guid id);

         Task DeleteAsync(Guid id);
    }
}
