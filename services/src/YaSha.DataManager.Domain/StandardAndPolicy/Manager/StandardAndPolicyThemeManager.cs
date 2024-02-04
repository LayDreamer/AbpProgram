using Microsoft.Extensions.Caching.Distributed;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Caching;
using YaSha.DataManager.ProductInventory.Dto;
using YaSha.DataManager.StandardAndPolicy.AggregateRoot;
using YaSha.DataManager.StandardAndPolicy.Dto;
using YaSha.DataManager.StandardAndPolicy.Repository;

namespace YaSha.DataManager.StandardAndPolicy.Manager;

public class StandardAndPolicyThemeManager : DataManagerDomainService
{
    private readonly IStandardAndPolicyThemeRepository _themeRepository;
    private readonly IStandardAndPolicyLibRepository _libRepository;
    private readonly IStandardAndPolicyTreeRepository _treeRepository;
    private readonly IStandardAndPolicyCollectRepository _collectRepository;
    private readonly IDistributedCache<Dictionary<Guid, string>> _treeCache;
    private readonly IDistributedCache<List<StandardAndPolicyPageDto>> _pageHomeCache;

    public StandardAndPolicyThemeManager(IStandardAndPolicyThemeRepository themeRepository, IStandardAndPolicyLibRepository libRepository, IStandardAndPolicyTreeRepository treeRepository, IDistributedCache<Dictionary<Guid, string>> treeCache, IStandardAndPolicyCollectRepository collectRepository, IDistributedCache<List<StandardAndPolicyPageDto>> pageHomeCache)
    {
        _themeRepository = themeRepository;
        _libRepository = libRepository;
        _treeRepository = treeRepository;
        _treeCache = treeCache;
        _collectRepository = collectRepository;
        _pageHomeCache = pageHomeCache;
    }

    #region 增

    public async Task<List<StandardAndPolicyThemeDto>> Insert(StandardAndPolicyCreateAndUpdateDto input)
    {
        await _pageHomeCache.RemoveAsync(StandardAndPolicyConsts.StandardAndPolicyPageHomeCacheKey);
        var lib = ObjectMapper.Map<StandardAndPolicyLibDto, StandardAndPolicyLib>(input.Data);
        var insertLib = await _libRepository.InsertAsync(lib, autoSave: true);
        var themes = new List<StandardAndPolicyTheme>();
        foreach (var id in input.Themes)
        {
            themes.Add(new StandardAndPolicyTheme()
            {
                TreeId = id,
                LibId = insertLib.Id,
            });
        }

        await _themeRepository.InsertManyAsync(themes, autoSave: true);
        return await FindThemeByLibId(insertLib.Id);
    }

    #endregion

    #region 查

    public async Task<List<StandardAndPolicyLibDto>> GetAllPolicyLists()
    {
        var db = await _libRepository.GetListAsync();
        return ObjectMapper.Map<List<StandardAndPolicyLib>, List<StandardAndPolicyLibDto>>(db);
    }
    
    static bool CheckContains<T>(IEnumerable<T> A, IEnumerable<T> B)
    {
        if (B == null || !B.Any())
        {
            return true;
        }

        return B.All(t => A.Any(b => b.Equals(t)));
    }

    private async Task<List<StandardAndPolicyThemeDto>> FindThemeByLibId(Guid id, bool include = true)
    {
        var themes = await _themeRepository.FindById(id, include);
        return ObjectMapper.Map<List<StandardAndPolicyTheme>, List<StandardAndPolicyThemeDto>>(themes);
    }

    public async Task<List<StandardAndPolicyThemeDto>> FindThemeByTreeId(Guid id, bool include = true)
    {
        var themes = await _themeRepository.FindByTreeId(id, include);
        return ObjectMapper.Map<List<StandardAndPolicyTheme>, List<StandardAndPolicyThemeDto>>(themes);
    }

    public async Task<List<StandardAndPolicyPageDto>> PageHome(int showCount)
    {
        return await _pageHomeCache.GetOrAddAsync(StandardAndPolicyConsts.StandardAndPolicyPageHomeCacheKey, async () =>
        {
            var results = new List<StandardAndPolicyPageDto>();
            var dictionary = await _treeCache.GetOrAddAsync(StandardAndPolicyConsts.StandardAndPolicyAllNotHideTreeCacheKey, async () =>
            {
                var roots = await _treeRepository.GetRootTree();
                return roots.SelectMany(root => root.Children)
                    .Where(x => !x.Hide)
                    .ToDictionary(rootChild => rootChild.Id, rootChild => rootChild.Name);
            }, () => new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });
            foreach (var item in dictionary)
            {
                var parentName = await _treeRepository.GetParentName(item.Key);
                var data = await _themeRepository.FindByTreeId(item.Key, true, false, true, showCount);
                var tmp = data.Select(theme => ObjectMapper.Map<StandardAndPolicyLib, StandardAndPolicyCardDto>(theme.Lib)).ToList();
                results.Add(new StandardAndPolicyPageDto
                {
                    ParentName = parentName,
                    Id = item.Key,
                    Theme = item.Value,
                    Data = tmp,
                });
            }
            return results;
        },() => new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
        });
    }

    public async Task<List<StandardAndPolicyPageDto>> PageSelectTree(Guid id, int showCount)
    {
        var results = new List<StandardAndPolicyPageDto>();
        var dictionary = await _treeCache.GetOrAddAsync(StandardAndPolicyConsts.StandardAndPolicyAllTreeCacheKey, async () =>
        {
            var roots = await _treeRepository.GetRootTree();
            return roots.SelectMany(root => root.Children)
                .ToDictionary(rootChild => rootChild.Id, rootChild => rootChild.Name);
        }, () => new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        });
        var children = await _treeRepository.FindAllChildren(id);
        foreach (var child in children)
        {
            var filter = await _themeRepository.FindByTreeId(child, true, false, true, showCount);
            var tmp = filter.Select(theme => ObjectMapper.Map<StandardAndPolicyLib, StandardAndPolicyCardDto>(theme.Lib)).ToList();
            if (dictionary.TryGetValue(child, out var value))
            {
                var parentName = await _treeRepository.GetParentName(child);
                results.Add(new StandardAndPolicyPageDto
                {
                    ParentName = parentName,
                    Id = child,
                    Theme = value,
                    Data = tmp
                });
            }
        }

        return results;
    }

    public async Task<PagedResultDto<StandardAndStandLibWithAllTreeDto>> Page(
        Guid userId,
        Guid? id,
        string type,
        int category,
        string search,
        string province,
        bool isCollect,
        List<Guid> themes,
        string sorting,
        int skip,
        int maxCount)
    {
        var results = new List<StandardAndStandLibWithAllTreeDto>();
        var dictionary = await _treeCache.GetOrAddAsync(StandardAndPolicyConsts.StandardAndPolicyAllTreeCacheKey, async () =>
        {
            var roots = await _treeRepository.GetRootTree();
            return roots.SelectMany(root => root.Children)
                .ToDictionary(rootChild => rootChild.Id, rootChild => rootChild.Name);
        }, () => new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        });

        if (province == "国家级")
        {
            province = "中华人民共和国";
        }
        var allLibs = await _libRepository.Page(type, category, search, province, sorting);
        if (string.IsNullOrEmpty(sorting))
        {
            allLibs = allLibs.OrderByDescending(e => e.ImplementationDate).ToList();
        }
        foreach (var lib in allLibs)
        {
            //收藏过滤    
            if (isCollect)
            {
                var like = await _collectRepository.FindByUserIdAndLibId(userId, lib.Id);
                if (null == like || !like.Collect) continue;
            }

            var treeId = (await _themeRepository.FindById(lib.Id, false)).Select(x => x.TreeId);
            var tmpThemes = treeId as Guid[] ?? treeId.ToArray();
            //点击Home主题过滤
            if (id != null && !tmpThemes.Contains(id.Value)) continue;
            //点击详情多个主题过滤
            if (CheckContains(tmpThemes, themes))
            {
                var obj = new StandardAndStandLibWithAllTreeDto
                {
                    Lib = ObjectMapper.Map<StandardAndPolicyLib, StandardAndPolicyLibDto>(lib)
                };
                foreach (var tmpTheme in tmpThemes)
                {
                    if (dictionary.TryGetValue(tmpTheme, out var value))
                        obj.Themes.Add(value);
                }

                results.Add(obj);
            }
        }

        int count = results.Count;

        results = results.Skip((skip - 1) * maxCount).Take(maxCount).ToList();

        return new PagedResultDto<StandardAndStandLibWithAllTreeDto>()
        {
            Items = results,
            TotalCount = count,
        };
    }

    public async Task<StandardAndPolicyCardDetailDto> GetCardDetail(Guid userId, Guid id)
    {
        var result = new StandardAndPolicyCardDetailDto();

        var dictionary = await _treeCache.GetOrAddAsync(StandardAndPolicyConsts.StandardAndPolicyAllTreeCacheKey, async () =>
        {
            var roots = await _treeRepository.GetRootTree();
            return roots.SelectMany(root => root.Children)
                .ToDictionary(rootChild => rootChild.Id, rootChild => rootChild.Name);
        }, () => new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        });

        var lib = await _libRepository.FindAsync(id);

        result.Lib = ObjectMapper.Map<StandardAndPolicyLib, StandardAndPolicyLibDto>(lib);

        var threeIds = (await _themeRepository.FindById(id, false)).Select(x => x.TreeId);

        foreach (var item in threeIds)
        {
            if (dictionary.TryGetValue(item, out var value))
            {
                result.Themes.Add(value);
            }
        }

        var collect = await _collectRepository.FindByUserIdAndLibId(userId, id);
        result.Collect = collect?.Collect ?? false;
        return result;
    }
    
    #endregion

    #region 改

    public async Task<StandardAndPolicyCollectDto> UpdateCollectStatus(Guid userId, Guid id, bool status)
    {
        var collect = await _collectRepository.FindByUserIdAndLibId(userId, id);
        if (collect != null)
        {
            collect.Collect = status;
            var ent = await _collectRepository.UpdateAsync(collect, autoSave: true);
            return ObjectMapper.Map<StandardAndPolicyCollect, StandardAndPolicyCollectDto>(ent);
        }
        else
        {
            collect = new StandardAndPolicyCollect()
            {
                UserId = userId,
                LibId = id,
                Collect = status
            };
            var ent = await _collectRepository.InsertAsync(collect, autoSave: true);
            return ObjectMapper.Map<StandardAndPolicyCollect, StandardAndPolicyCollectDto>(ent);
        }
    }

    public async Task<StandardAndPolicyLibDto> UpdateLib(Guid id, StandardAndPolicyCreateAndUpdateDto input)
    {
        var find = await _libRepository.FindAsync(id);
        find.Number = input.Data.Number;
        find.Name = input.Data.Name;
        find.Industry = input.Data.Industry;
        find.PublishingUnit = input.Data.PublishingUnit;
        find.PublishingDate = input.Data.PublishingDate;
        find.LoseDate = input.Data.LoseDate;
        find.ImplementationDate = input.Data.ImplementationDate;
        find.ImagePath = input.Data.ImagePath;
        find.PdfPath = input.Data.PdfPath;
        find.LinkPath = input.Data.LinkPath;
        find.Type = input.Data.Type;

        var update = await _libRepository.UpdateAsync(find, true);

        var themes = await _themeRepository.FindById(id);
        var deletes = themes.Where(x => !input.Themes.Any(y => y.Equals(x.TreeId)));
        await _themeRepository.DeleteManyAsync(deletes, true);
        var adds = input.Themes.Where(x => !themes.Any(y => y.TreeId.Equals(x)));
        if (adds.Count() > 0)
        {
            var tmp = new List<StandardAndPolicyTheme>();
            foreach (var guid in adds)
            {
                tmp.Add(new StandardAndPolicyTheme()
                {
                    TreeId = guid,
                    LibId = id,
                });
            }

            await _themeRepository.InsertManyAsync(tmp, autoSave: true);
        }

        return ObjectMapper.Map<StandardAndPolicyLib, StandardAndPolicyLibDto>(update);
    }

    #endregion

    #region 删

    public async Task<List<StandardAndPolicyLibDto>> Delete(List<Guid> ids)
    {
        await _pageHomeCache.RemoveAsync(StandardAndPolicyConsts.StandardAndPolicyPageHomeCacheKey);
        await _collectRepository.Delete(ids);
        await _themeRepository.Delete(ids);
        var ents = await _libRepository.FindLibsByIds(ids);
        await _libRepository.DeleteManyAsync(ents, autoSave: true);
        return ObjectMapper.Map<List<StandardAndPolicyLib>, List<StandardAndPolicyLibDto>>(ents);
    }

    #endregion
}