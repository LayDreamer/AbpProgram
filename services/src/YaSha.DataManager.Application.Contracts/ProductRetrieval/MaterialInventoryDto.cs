using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace YaSha.DataManager.ProductRetrieval
{
    public class MaterialInventoryDto : AuditedEntityDto<Guid>
    {
        public string Data { get; set; }
    }
}
