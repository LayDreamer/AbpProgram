using Volo.Abp.Domain.Entities.Auditing;

namespace YaSha.DataManager.ArchitectureList.AggregateRoot;

public class ArchitectureListModule : FullAuditedAggregateRoot<Guid>
{
    public Guid ParentId { get; set; }

    public ArchitectureListTree Parent { get; set; }
    
    public string System { get; set; }
    
    public string Name { get; set; }
    
    public string Category { get; set; }
    
    public string Model { get; set; }
    
    public string ProcessingMode { get; set; }
    
    public string Length { get; set; }

    public string Width { get; set; }

    public string Height { get; set; }
    
    public string Unit { get; set; }
    
    public string ProcessingCode { get; set; }
    
    public string SupplyMode { get; set; }
    
    public string ModuleSpecification { get; set; }
    
    public string ProcessNum { get; set; }

    public string AssemblyDrawingNum { get; set; }

    public string DetailNum { get; set; }
    
    public string Remark { get; set; }
    
    public string Path { get; set; }
    
    public string CreateUser { get; set; }

    public string ModifyUser { get; set; }
    
    /// <summary>
    /// 选配信息
    /// </summary>
    public string Optional { get; set; }
    
    /// <summary>
    /// true 表示从定制加工清单导入的
    /// </summary>
    public bool IsProcessing { get; set; }
    public ArchitectureListPublishStatus Status { get; set; }
    
    public List<ArchitectureListMaterial> Materials { get; set; }

    public ArchitectureListModule()
    {
        Materials = new List<ArchitectureListMaterial>();
    }

    public void SetParentId(Guid id)
    {
        this.ParentId = id;
        if (this.Id == Guid.Empty)
            this.Id = Guid.NewGuid();
        foreach (var item in this.Materials)
        {
            item.SetParentId(this.Id);
        }
    }

    public void SetCopyName()
    {
        foreach (var material in this.Materials)
        {
            material.SetCopyName();
        }
    }
}


