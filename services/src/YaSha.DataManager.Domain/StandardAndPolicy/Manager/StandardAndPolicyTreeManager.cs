using EasyAbp.Abp.Trees;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Distributed;
using Volo.Abp.Caching;
using Volo.Abp.ObjectMapping;
using YaSha.DataManager.StandardAndPolicy.AggregateRoot;
using YaSha.DataManager.StandardAndPolicy.Dto;
using YaSha.DataManager.StandardAndPolicy.Repository;

namespace YaSha.DataManager.StandardAndPolicy.Manager;

public class StandardAndPolicyTreeManager : DataManagerDomainService
{
    private readonly IStandardAndPolicyTreeRepository _repository;
    private readonly IStandardAndPolicyThemeRepository _themeRepository;
    private readonly IDistributedCache<Dictionary<Guid, string>> _treeCache;
    private readonly IDistributedCache<List<StandardAndPolicyTreeDto>> _treeDtoCache;
    private readonly IDistributedCache<List<StandardAndPolicyPageDto>> _pageHomeCache;
    public StandardAndPolicyTreeManager(IStandardAndPolicyTreeRepository repository, IStandardAndPolicyThemeRepository themeRepository, IDistributedCache<Dictionary<Guid, string>> treeCache, IDistributedCache<List<StandardAndPolicyTreeDto>> treeDtoCache, IDistributedCache<List<StandardAndPolicyPageDto>> pageHomeCache)
    {
        _repository = repository;
        _treeCache = treeCache;
        _treeDtoCache = treeDtoCache;
        _pageHomeCache = pageHomeCache;
        _themeRepository = themeRepository;
    }

    public async Task<List<StandardAndPolicyTreeDto>> GetRootTree()
    {
        return  await _treeDtoCache.GetOrAddAsync(StandardAndPolicyConsts.StandardAndPolicyAllRootTreeCacheKey, async () =>
        {
            var roots = await _repository.GetRootTree();
            return ObjectMapper.Map<List<StandardAndPolicyTree>, List<StandardAndPolicyTreeDto>>(roots);
        }, () => new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        });
    }

    public async Task<StandardAndPolicyTreeDto> Add(Guid id, string name)
    {
        await DeleteCacheKey();
        var count = await _repository.GetSameParentTotalCount(id);
        var tree = new StandardAndPolicyTree
        {
            Name = name,
            ParentId = id,
            Code = count + 1,
        };
        var insert = await _repository.InsertAsync(tree);
        return ObjectMapper.Map<StandardAndPolicyTree, StandardAndPolicyTreeDto>(insert);
    }

    public async Task Delete(Guid id)
    {
        await DeleteCacheKey();
        await _repository.DeleteTree(id);
        var themes = await _themeRepository.FindByTreeId(id);
        await _themeRepository.DeleteManyAsync(themes);
    }

    public async Task DeleteCacheKey()
    {
        await _treeCache.RemoveManyAsync(new List<string>
        {
            StandardAndPolicyConsts.StandardAndPolicyAllTreeCacheKey,
            StandardAndPolicyConsts.StandardAndPolicyAllNotHideTreeCacheKey
        });

        await _treeDtoCache.RemoveAsync(StandardAndPolicyConsts.StandardAndPolicyAllRootTreeCacheKey);
        await _pageHomeCache.RemoveAsync(StandardAndPolicyConsts.StandardAndPolicyPageHomeCacheKey);
    }
    
    public async Task ExChangeTowNode(Guid id1, Guid id2)
    {
        await DeleteCacheKey();
        await _repository.ExchangeTowNode(id1, id2);
    }

    public async Task<StandardAndPolicyTreeDto> ReName(Guid id, string name)
    {
        await DeleteCacheKey();
        var tree = await _repository.FindAsync(id);
        tree.Name = name;
        var update = await _repository.UpdateAsync(tree, autoSave: true);
        return ObjectMapper.Map<StandardAndPolicyTree, StandardAndPolicyTreeDto>(update);
    }

    public async Task<List<StandardAndPolicyTreeDto>> HideTree(List<Guid> id, bool hide)
    {
        await DeleteCacheKey();
        var tree = await _repository.HideTree(id, hide);
        return ObjectMapper.Map<List<StandardAndPolicyTree>, List<StandardAndPolicyTreeDto>>(tree);
    }
    
    
}