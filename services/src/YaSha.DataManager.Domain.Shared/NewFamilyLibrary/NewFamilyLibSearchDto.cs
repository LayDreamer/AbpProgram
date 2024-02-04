using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace YaSha.DataManager.NewFamilyLibrary
{
    public class NewFamilyLibSearchDto : PagedAndSortedResultRequestDto
    {
        public string Key { get; set; }

        public string SearchValue { get; set; }

        public string SearchCode { get; set; }

        public string CalculateCacheKey()
        {
            return "_" + this.SearchValue + "_" + this.SearchCode + "_" + this.Sorting + "_" + this.SkipCount + "_" + this.MaxResultCount;
        }
    }
}
