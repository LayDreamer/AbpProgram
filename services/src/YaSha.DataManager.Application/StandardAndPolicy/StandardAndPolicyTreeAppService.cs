using Volo.Abp;
using Volo.Abp.Users;
using YaSha.DataManager.StandardAndPolicy.Dto;
using YaSha.DataManager.StandardAndPolicy.Manager;

namespace YaSha.DataManager.StandardAndPolicy;

public class StandardAndPolicyTreeAppService : DataManagerAppService, IStandardAndPolicyTreeAppService
{
    private readonly StandardAndPolicyTreeManager _service;

    private readonly ICurrentUser _currentUser;

    public StandardAndPolicyTreeAppService(StandardAndPolicyTreeManager service, ICurrentUser currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    public async Task<List<StandardAndPolicyTreeDto>> GetRoot()
    {
        if (_currentUser.Id == null)
        {
            throw new BusinessException("YaSha.DataManager:LoginOver");
        }

        return await _service.GetRootTree();
    }

    public async Task<List<StandardAndPolicyTreeDto>> HideTree(List<Guid> id, bool hide)
    {
        return await _service.HideTree(id, hide);
    }

    public async Task<StandardAndPolicyTreeDto> AddChildren(Guid id, string nodeName)
    {
        return await _service.Add(id, nodeName);
    }

    public async Task<ApiResultDto> DeleteChildren(Guid id)
    {
        try
        {
            await _service.Delete(id);
            return new ApiResultDto()
            {
                Success = true
            };
        }
        catch (Exception e)
        {
            return new ApiResultDto()
            {
                Success = false,
                Error = e.Message,
            };
        }
    }

    public async Task<StandardAndPolicyTreeDto> ReNameChildren(Guid id, string nodeName)
    {
        return await _service.ReName(id, nodeName);
    }

    public async Task<ApiResultDto> ExChangeTowNode(Guid id1, Guid id2)
    {
        try
        {
            await _service.ExChangeTowNode(id1, id2);
            return new ApiResultDto()
            {
                Success = true
            };
        }
        catch (Exception e)
        {
            return new ApiResultDto()
            {
                Success = false,
                Error = e.Message,
            };
        }
    }
}