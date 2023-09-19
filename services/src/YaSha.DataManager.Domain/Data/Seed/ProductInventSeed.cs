using YaSha.DataManager.ProductInventory;
using YaSha.DataManager.ProductInventory.AggregateRoot;
using YaSha.DataManager.Repository.ProductInventory;

namespace YaSha.DataManager.Data.Seed;

public class ProductInventSeed : IDataSeedContributor, ITransientDependency
{
    private readonly IProductInvTreeRepository _repository;

    public ProductInventSeed(IProductInvTreeRepository repository)
    {
        _repository = repository;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        var productTree = new ProductInventTree(Guid.NewGuid(), "产品", null);
        
        var ceil = new ProductInventTree(Guid.NewGuid(), "吊顶系统", productTree);
        var wall = new ProductInventTree(Guid.NewGuid(), "墙面系统", productTree);
        var floor = new ProductInventTree(Guid.NewGuid(), "地面系统", productTree);
        var kitchen = new ProductInventTree(Guid.NewGuid(), "厨房系统", productTree);
        var bathroom = new ProductInventTree(Guid.NewGuid(), "卫浴系统", productTree);
        var partitionWall = new ProductInventTree(Guid.NewGuid(), "隔墙系统", productTree);
        var doorWindow = new ProductInventTree(Guid.NewGuid(), "门窗系统", productTree);
        var heating = new ProductInventTree(Guid.NewGuid(), "地暖系统", productTree);
        var levelling = new ProductInventTree(Guid.NewGuid(), "调平系统", productTree);
        
        var kitchenKRCZ = new ProductInventTree(Guid.NewGuid(), "科睿厨房系列·瓷砖", kitchen);
        var ceilKRLJ = new ProductInventTree(Guid.NewGuid(), "科睿吊顶系列·琉晶", ceil);
        var wallKRKN = new ProductInventTree(Guid.NewGuid(), "科睿墙面系列·科耐", wall);
        var wallKRKY = new ProductInventTree(Guid.NewGuid(), "科睿墙面系列·科岩", wall);
        var wallKYKN = new ProductInventTree(Guid.NewGuid(), "科宜墙面系列·科耐", wall);
        var wallKYKY = new ProductInventTree(Guid.NewGuid(), "科宜墙面系列·科岩", wall);
        var wallKYZP = new ProductInventTree(Guid.NewGuid(), "科宜墙面系列·找平基层", wall);
        var wallKYUKN = new ProductInventTree(Guid.NewGuid(), "科誉墙面系列·科耐", wall);
        var wallKRKJYL = new ProductInventTree(Guid.NewGuid(), "科睿墙面系列·科晶(医疗)", wall);
        var wallKRKJ = new ProductInventTree(Guid.NewGuid(), "科睿墙面系列·科晶", wall);
        var wallKRKNBJQ = new ProductInventTree(Guid.NewGuid(), "科睿墙面系列·科耐·背景墙", wall);
        var levellingKY38 = new ProductInventTree(Guid.NewGuid(), "科宜墙面调平系列·38系碳钢龙骨", levelling);
        var levellingKR50 = new ProductInventTree(Guid.NewGuid(), "科睿墙面调平系列·50系碳钢龙骨", levelling);
        var partitionWallKRCS = new ProductInventTree(Guid.NewGuid(), "科睿隔墙系列·CS隔墙", partitionWall);
        var partitionWallKYGZ = new ProductInventTree(Guid.NewGuid(), "科誉隔墙系列·钢制隔墙", partitionWall);
        var partitionWallKRRGS = new ProductInventTree(Guid.NewGuid(), "科睿隔墙系列·RGS隔墙", partitionWall);
        var floorCSJK = new ProductInventTree(Guid.NewGuid(), "科睿地面系列·CS架空", floor);
        var floorKRKY = new ProductInventTree(Guid.NewGuid(), "科睿地面系列·科岩", floor);
        var bathroomKRCZ = new ProductInventTree(Guid.NewGuid(), "科睿卫浴系列·瓷砖", bathroom);
        var bathroomKRKY = new ProductInventTree(Guid.NewGuid(), "科睿卫浴系列·科岩", bathroom);
        var bathroomKRCZCeil = new ProductInventTree(Guid.NewGuid(), "顶面", bathroomKRCZ);
        var bathroomKRCZWall = new ProductInventTree(Guid.NewGuid(), "墙面", bathroomKRCZ);
        var bathroomKRCZFloor = new ProductInventTree(Guid.NewGuid(), "地面", bathroomKRCZ);
        
        await _repository.InitProductInvTree(productTree);
        
        var projectTree = new ProductInventTree(Guid.NewGuid(), "项目", null);
        
        await _repository.InsertAsync(projectTree,autoSave:true);
    }
}