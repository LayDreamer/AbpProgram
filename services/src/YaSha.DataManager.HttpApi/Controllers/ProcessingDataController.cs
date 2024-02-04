using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using YaSha.DataManager.ProcessingLists;

namespace YaSha.DataManager.Controllers
{
    [Route("ProcessingData")]
    public class ProcessingDataController : DataManagerController, IProcessingDataAppService
    {

        private readonly IProcessingDataAppService _service;

        public ProcessingDataController(IProcessingDataAppService service)
        {
            _service = service;
        }

        [HttpPost("Create")]
        public Task<ProcessingDataDto> CreateAsync(ProcessingDataCreateDto input)
        {
            throw new NotImplementedException();
        }

        [HttpPost("Delete")]
        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        [HttpPost("Get")]
        public Task<ProcessingDataDto> GetAsync(Guid id)
        {
           return _service.GetAsync(id);
        }

        [HttpPost("GetList")]
        public Task<PagedResultDto<ProcessingDataDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            return _service.GetListAsync(input);
        }

        [HttpPost("Update")]
        public Task<ProcessingDataDto> UpdateAsync(Guid id, ProcessingDataCreateDto input)
        {
            throw new NotImplementedException();
        }

    }
}
