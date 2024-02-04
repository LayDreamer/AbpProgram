//using SixLabors.ImageSharp.Processing;
using YaSha.DataManager.ProductInventory.Dto;
using YaSha.DataManager.ProductInventory.Manager;

namespace YaSha.DataManager.ProductInventory;

public class ProductIventTreeAppService :DataManagerAppService,IProductIventTreeAppService
{
    private readonly ProductInventTreeManager _service;
    
    public ProductIventTreeAppService(ProductInventTreeManager service)
    {
        _service = service;
    }

    public async Task<List<ProductInventoryTreeDto>> GetRoot()
    {
        var roots = await _service.GetRootTree();
        roots = roots.OrderBy(x => x.Name).ToList();
        foreach (var root in roots)
        {
            Sort(root);
        }
        return roots;
    }

    public async Task<ProductInventoryTreeDto> UpdateRemark(Guid id, string remark)
    {
        return await _service.UpdateRemark(id, remark);
    }


    void Sort(ProductInventoryTreeDto dto)
    {
        if (dto.Children == null || dto.Children.Count == 0)
        {
            return;
        }
        dto.Children = dto.Children.OrderBy(x => x.Name).ToList();
        foreach (var item in dto.Children)
        {
            Sort(item);   
        }
    }
}