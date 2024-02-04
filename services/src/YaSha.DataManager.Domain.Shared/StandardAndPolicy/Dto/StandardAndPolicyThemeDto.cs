using Volo.Abp.Application.Dtos;

namespace YaSha.DataManager.StandardAndPolicy.Dto;

public class StandardAndPolicyThemeDto : AuditedEntityDto<Guid>
{
    public Guid LibId { get; set; } 
    
    public StandardAndPolicyLibDto Lib { get; set; }
    
    public Guid TreeId { get; set; }

    public StandardAndPolicyTreeDto Tree { get; set; }
    
}