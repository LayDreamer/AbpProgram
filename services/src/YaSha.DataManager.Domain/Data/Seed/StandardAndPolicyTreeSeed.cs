using YaSha.DataManager.StandardAndPolicy.AggregateRoot;
using YaSha.DataManager.StandardAndPolicy.Repository;

namespace YaSha.DataManager.Data.Seed;

public class StandardAndPolicyTreeSeed : IDataSeedContributor, ITransientDependency
{
    private readonly IStandardAndPolicyTreeRepository _repository;

    public StandardAndPolicyTreeSeed(IStandardAndPolicyTreeRepository repository)
    {
        _repository = repository;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        var root1 = new StandardAndPolicyTree(Guid.NewGuid(), "标准", null) { Code = 0 };
        var a = new StandardAndPolicyTree(Guid.NewGuid(), "装配式建筑", root1);
        var b = new StandardAndPolicyTree(Guid.NewGuid(), "钢结构建筑", root1);
        var c = new StandardAndPolicyTree(Guid.NewGuid(), "混凝土建筑", root1);
        var d = new StandardAndPolicyTree(Guid.NewGuid(), "绿色建筑评价", root1);
        var e = new StandardAndPolicyTree(Guid.NewGuid(), "全装修", root1);
        var f = new StandardAndPolicyTree(Guid.NewGuid(), "防火", root1);
        var g = new StandardAndPolicyTree(Guid.NewGuid(), "内环境", root1);
        var root2 = new StandardAndPolicyTree(Guid.NewGuid(), "政策", null) { Code = 1 };
        var h = new StandardAndPolicyTree(Guid.NewGuid(), "装配式装修", root2);
        var i = new StandardAndPolicyTree(Guid.NewGuid(), "绿色低碳", root2);
        var j = new StandardAndPolicyTree(Guid.NewGuid(), "建筑节能", root2);
        var k = new StandardAndPolicyTree(Guid.NewGuid(), "城市建设方案", root2);
        var l = new StandardAndPolicyTree(Guid.NewGuid(), "智能建造", root2);
        var m = new StandardAndPolicyTree(Guid.NewGuid(), "BIM", root2);
        var n = new StandardAndPolicyTree(Guid.NewGuid(), "产业链", root2);
        var o = new StandardAndPolicyTree(Guid.NewGuid(), "家居", root2);
        await _repository.InitStandardAndPolicyTree(new List<StandardAndPolicyTree>() { root1, root2 });
    }
}