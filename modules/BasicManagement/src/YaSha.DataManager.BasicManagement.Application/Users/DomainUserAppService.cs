using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using YaSha.DataManager.BasicManagement.Users.Dtos;
using YaSha.DataManager.Domain;

namespace YaSha.DataManager.BasicManagement.Users
{
    public class DomainUserAppService : CrudAppService<
            DomainUser, //The Book entity
            DomainUserDto, //Used to show books
            Guid, //Primary key of the book entity
            PagedAndSortedResultRequestDto //Used for paging/sorting
          >, //Used to create/update a book
        IDomainUserAppService //implement the IBookAppService
    {
        IRepository<DomainUser, Guid> _repository;
        public DomainUserAppService(IRepository<DomainUser, Guid> repository)
               : base(repository)
        {

            _repository = repository;
        }

        public async Task CustomCreateAsync(DomainUserDto domainUserDto)
        {
            await _repository.InsertAsync(new DomainUser
            {
                UserName = domainUserDto.UserName,
            }, true);

           var domainUsers =await _repository.GetListAsync();
        }

    }
}
