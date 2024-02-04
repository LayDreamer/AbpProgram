using Volo.Abp.Domain.Repositories;
using YaSha.DataManager.MaterialManage.AggregateRoot;

namespace YaSha.DataManager.MaterialManage.Repository;

public interface IMaterialManageInfoRepository : IBasicRepository<MaterialManageInfo, Guid>
{
    Task<List<MaterialManageInfo>> FindByIds(List<Guid> ids);

    Task<MaterialManageInfo> FindBySequenceCodeAndSupplierCode(string sequenceCode, string supplierCode);

    Task<List<MaterialManageInfo>> FindImageByMaterialName(string name);

    Task<List<MaterialManageInfo>> FindImageBySeriesName(string name);

    Task<Tuple<List<string>, List<string>, List<string>>> GetManageExtraInfo();
    
    Task<Tuple<int, List<MaterialManageInfo>>> PageManage(
        List<string> matchKeys,
        string search,
        List<string> status,
        List<string> materialType,
        List<string> supplier,
        List<string> supplierCode,
        List<string> seriesName,
        string sorting,
        int skipCount,
        int maxResultCount);

    Task<Tuple<int, List<MaterialManageInfo>, List<string>, List<string>, List<string>>> PageHome(
        Dictionary<Guid, float> maps,
        List<string> materialType,
        List<string> materialTexture,
        List<string> materialSurface,
        List<string> supplier,
        string search,
        string sorting,
        int skipCount,
        int maxResultCount);
}