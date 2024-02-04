using Volo.Abp.Domain.Repositories;
using YaSha.DataManager.ProductInventory.AggregateRoot;
using YaSha.DataManager.StandardAndPolicy.AggregateRoot;

namespace YaSha.DataManager.StandardAndPolicy.Repository;

public interface IStandardAndPolicyTreeRepository : IBasicRepository<StandardAndPolicyTree, Guid>
{
    Task InitStandardAndPolicyTree(List<StandardAndPolicyTree> roots);

    Task DeleteTree(Guid id);

    Task ExchangeTowNode(Guid id1, Guid id2);
    
    Task<int> GetSameParentTotalCount(Guid id);
    
    Task<List<StandardAndPolicyTree>> GetRootTree();

    Task<string> GetParentName(Guid id);
    
    Task<List<Guid>> FindAllChildren(Guid id);

    Task<List<StandardAndPolicyTree>> HideTree(List<Guid> id, bool hide);
}