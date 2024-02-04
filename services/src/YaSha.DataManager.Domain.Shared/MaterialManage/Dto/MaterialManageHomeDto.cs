using Volo.Abp.Application.Dtos;

namespace YaSha.DataManager.MaterialManage.Dto;

public class MaterialManageManageExtraInfo
{
    public List<string> Supplier { get; set; }

    public List<string> SupplierCode { get; set; }

    public List<string> SeriesName { get; set; }
}

public class MaterialManageHomeDto : PagedResultDto<MaterialManageHomeItem>
{
    public List<string> MaterialTypes { get; set; }
    public List<string> MaterialTextures { get; set; }
    public List<string> MaterialSurfaces { get; set; }
}

public class MaterialManageHomeItem
{
    public Guid Id { get; set; }
    public string MaterialImageUrl { get; set; }

    public string SequenceCode { get; set; }
    public string MaterialType { get; set; }
    public string MaterialTexture { get; set; }
    public string MaterialSurface { get; set; }

    /// <summary>
    /// 拍照识别相似度
    /// </summary>
    public string Similarity { get; set; }
}