using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YaSha.DataManager.ArchitectureList.AggregateRoot;

namespace YaSha.DataManager.NewFamilyLibrary
{
 
    public static class NewFamilyLibraryQueryableExtensions
    {
        public static IQueryable<NewFamilyTree> IncludeTreeDetails(this IQueryable<NewFamilyTree> queryable,
            bool include = true)
        {
            return !include
                ? queryable
                : queryable
                    .Include(x => x.Children);
        }


        public static IQueryable<NewFamilyLib> IncludeLibDetails(this IQueryable<NewFamilyLib> queryable,
          bool include = true)
        {
            return !include
                ? queryable
                : queryable
                    .Include(x => x.Children);
        }


    }



}
