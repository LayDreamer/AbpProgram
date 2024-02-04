using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using YaSha.DataManager.ArchitectureList.AggregateRoot;
using YaSha.DataManager.ArchitectureList.Repository;
using YaSha.DataManager.EntityFrameworkCore;
using YaSha.DataManager.NewFamilyLibrary.Repository;

namespace YaSha.DataManager.NewFamilyLibrary
{
    public class NewFamilyTreeRepository : EfCoreRepository<DataManagerDbContext, NewFamilyTree, Guid>,
    INewFamilyTreeRepository
    {
        public NewFamilyTreeRepository(IDbContextProvider<DataManagerDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

     

    }
}
