using YaSha.DataManager.StandardAndPolicy.Dto;

namespace YaSha.DataManager.StandardAndPolicy;

public interface IStandardAndPolicyTreeAppService : IApplicationService
{
    Task<List<StandardAndPolicyTreeDto>> GetRoot();

    Task<List<StandardAndPolicyTreeDto>> HideTree(List<Guid> id, bool hide);
    
    Task<StandardAndPolicyTreeDto> AddChildren(Guid id, string nodeName);

    Task<ApiResultDto> DeleteChildren(Guid id);

    Task<StandardAndPolicyTreeDto> ReNameChildren(Guid id, string nodeName);

    Task<ApiResultDto> ExChangeTowNode(Guid id1, Guid id2);
    
}