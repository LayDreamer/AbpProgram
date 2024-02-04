using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using YaSha.DataManager.ProductInventory.AggregateRoot;

namespace YaSha.DataManager.ProcessingLists.Repository
{
    public  interface IProcessingListRespository : IBasicRepository<ProcessingList, Guid>
    {


    }
}
