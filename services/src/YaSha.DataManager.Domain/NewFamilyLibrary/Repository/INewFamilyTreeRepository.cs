using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using YaSha.DataManager.ArchitectureList.AggregateRoot;

namespace YaSha.DataManager.NewFamilyLibrary.Repository
{
    public interface INewFamilyTreeRepository : IBasicRepository<NewFamilyTree, Guid>
    {
    }
}
