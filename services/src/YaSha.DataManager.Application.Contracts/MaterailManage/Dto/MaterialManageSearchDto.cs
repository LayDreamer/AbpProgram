using Volo.Abp.Application.Dtos;

namespace YaSha.DataManager.MaterailManage.Dto;

public class MaterialManageSearchDto : PagedAndSortedResultRequestDto
{
    /// <summary>
    /// 搜索内容 （顺序码、物料编码、供应商编码）
    /// </summary>
    public string Search { get; set; }
    
    public List<string> Status { get; set; }
    
    public List<string> MaterialType { get; set; }
    
    public List<string> Supplier { get; set; }
    
    public List<string> SupplierCode { get; set; }
    
    public List<string> SeriesName { get; set; }
}