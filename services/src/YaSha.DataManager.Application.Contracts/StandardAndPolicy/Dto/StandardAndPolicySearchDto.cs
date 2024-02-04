using Volo.Abp.Application.Dtos;

namespace YaSha.DataManager.StandardAndPolicy.Dto;

public class StandardAndPolicySearchDto : PagedAndSortedResultRequestDto
{
    public StandardAndPolicySearchDto(Guid? id, string type, int category, string search, bool isCollect, List<Guid> themes, string province)
    {
        Id = id;
        Type = type;
        Category = category;
        Search = search;
        IsCollect = isCollect;
        Themes = themes;
        Province = province;
    }

    /// <summary>
    /// Id
    /// </summary>
    public Guid? Id { get; }

    /// <summary>
    /// 标准/政策
    /// </summary>
    public string Type { get;}
    
    
    /// <summary>
    /// 类别 [全部、国家标准、.....]
    /// </summary>
    public int  Category { get; }
    
    
    /// <summary>
    /// 搜索内容  [标题、编号、单位]
    /// </summary>
    public string Search { get; }
    
    
    /// <summary>
    /// 省份
    /// </summary>
    public string Province { get; }
    
    
    /// <summary>
    /// 当前收藏
    /// </summary>
    public bool IsCollect { get; }
    
    /// <summary>
    /// 主题
    /// </summary>
    public List<Guid> Themes { get; }
}