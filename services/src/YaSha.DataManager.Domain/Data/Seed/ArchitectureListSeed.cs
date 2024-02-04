using YaSha.DataManager.ArchitectureList.AggregateRoot;
using YaSha.DataManager.ArchitectureList.Repository;

namespace YaSha.DataManager.Data.Seed;

public class ArchitectureListSeed : IDataSeedContributor, ITransientDependency
{
    private readonly IArchitectureListTreeRepository _repository;

    public ArchitectureListSeed(IArchitectureListTreeRepository repository)
    {
        _repository = repository;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        var root = new ArchitectureListTree(Guid.NewGuid(), "分类", 0, null);

        var guhua = new ArchitectureListTree(Guid.NewGuid(), "固化", 0, root);

        var ceil = new ArchitectureListTree(Guid.NewGuid(), "吊顶系统", 1, guhua);
        var wall = new ArchitectureListTree(Guid.NewGuid(), "墙面系统", 2, guhua);
        var floor = new ArchitectureListTree(Guid.NewGuid(), "地面系统", 3, guhua);
        var kitchen = new ArchitectureListTree(Guid.NewGuid(), "厨房系统", 4, guhua);
        var bathroom = new ArchitectureListTree(Guid.NewGuid(), "卫浴系统", 5, guhua);
        var partitionWall = new ArchitectureListTree(Guid.NewGuid(), "隔墙系统", 6, guhua);
        var doorWindow = new ArchitectureListTree(Guid.NewGuid(), "门窗系统", 7, guhua);

        var xiangmu = new ArchitectureListTree(Guid.NewGuid(), "项目", 1, root);

        await _repository.InitTree(new List<ArchitectureListTree> { root });
    }
}