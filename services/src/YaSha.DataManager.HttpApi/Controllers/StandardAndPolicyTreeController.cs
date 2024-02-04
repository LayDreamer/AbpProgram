using YaSha.DataManager.StandardAndPolicy;
using YaSha.DataManager.StandardAndPolicy.Dto;

namespace YaSha.DataManager.Controllers;

/// <summary>
/// 标准与政策树
/// </summary>
[Route("StandardAndPolicyTree")]
public class StandardAndPolicyTreeController : DataManagerController, IStandardAndPolicyTreeAppService
{
    private readonly IStandardAndPolicyTreeAppService _service;

    public StandardAndPolicyTreeController(IStandardAndPolicyTreeAppService service)
    {
        _service = service;
    }

    [HttpPost("GetTree")]
    public async Task<List<StandardAndPolicyTreeDto>> GetRoot()
    {
        return await _service.GetRoot();
    }

    [HttpPost("HideTree")]
    public async Task<List<StandardAndPolicyTreeDto>> HideTree(List<Guid> id, bool hide)
    {
        return await _service.HideTree(id, hide);
    }

    [HttpPost("AddChild")]
    public async Task<StandardAndPolicyTreeDto> AddChildren(Guid id, string nodeName)
    {
        return await _service.AddChildren(id, nodeName);
    }

    [HttpPost("DeleteChild")]
    public async Task<ApiResultDto> DeleteChildren(Guid id)
    {
        return await _service.DeleteChildren(id);
    }

    [HttpPost("ReNameChild")]
    public async Task<StandardAndPolicyTreeDto> ReNameChildren(Guid id, string nodeName)
    {
        return await _service.ReNameChildren(id, nodeName);
    }

    [HttpPost("ExChange")]
    public async Task<ApiResultDto> ExChangeTowNode(Guid id1, Guid id2)
    {
        return await _service.ExChangeTowNode(id1, id2);
    }
}