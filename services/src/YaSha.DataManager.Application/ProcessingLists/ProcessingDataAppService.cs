using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace YaSha.DataManager.ProcessingLists
{
    public  class ProcessingDataAppService :
      CrudAppService<
         ProcessingData,
         ProcessingDataDto,
         Guid,
         PagedAndSortedResultRequestDto,
         ProcessingDataCreateDto>,
     IProcessingDataAppService
    {
        protected readonly IRepository<ProcessingData, Guid> _repository;

        public ProcessingDataAppService(IRepository<ProcessingData, Guid> repository)
            : base(repository)
        {
            _repository = repository;
        }
    }
}
