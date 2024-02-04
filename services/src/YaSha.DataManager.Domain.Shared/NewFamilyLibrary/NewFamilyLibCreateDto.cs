using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace YaSha.DataManager.NewFamilyLibrary
{
    public class NewFamilyLibCreateDto 
    {
      
        public Guid Id { get; set; }
        public string Status { get; set; }
        public Guid CategoryId { get; set; }
        public Guid? ParentId { get; set; }
        public string Code { get; set; }
        public int Level { get; set; }
 
        public string DisplayName { get; set; }
        public string Number { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string Unit { get; set; }
        public string Version { get; set; }
        public string UploadUser { get; set; }
        public string FilePath { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }
        public string ExternalData { get; set; }
        public string Hierarchy { get; set; }
        public string Type { get; set; }
        public string ProcessMode { get; set; }
        public string Notes { get; set; }
        public string Usage { get; set; }

    }
}
