using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace YaSha.DataManager.NewFamilyLibrary
{
    public class NewFamilyTreeDto : AuditedEntityDto<Guid>
    {
        public NewFamilyTreeDto()
        {
            Children= new List<NewFamilyTreeDto>();
        }

      
        public Guid? ParentId { get; set; }
        public string Code { get; set; }
        public int Level { get; set; }
        public NewFamilyTreeDto Parent { get; set; }
        public List<NewFamilyTreeDto> Children { get; set; }
        public string DisplayName { get; set; }
       
    }

}
