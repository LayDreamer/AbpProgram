﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace YaSha.DataManager.FamilyTrees
{
    public class FamilyTreeDto : AuditedEntityDto<Guid>
    {
        public Guid? ParentId { get; set; }
        public string Code { get; set; }
        public int Level { get; set; }
        public FamilyTreeDto Parent { get; set; }
        public ICollection<FamilyTreeDto> Children { get; set; }
        public string DisplayName { get; set; }
        public string BlobName { get; set; }
    }
}
