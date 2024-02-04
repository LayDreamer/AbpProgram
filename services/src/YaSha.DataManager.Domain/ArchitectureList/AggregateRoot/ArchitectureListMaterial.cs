using Volo.Abp.Domain.Entities.Auditing;

namespace YaSha.DataManager.ArchitectureList.AggregateRoot;

public class ArchitectureListMaterial : FullAuditedAggregateRoot<Guid>
{
    public Guid ParentId { get; set; }

    public ArchitectureListModule Parent { get; set; }

    public string Composition { get; set; }

    public string Name { get; set; }

    public string Code { get; set; }

    public string Length { get; set; }

    public string Width { get; set; }

    public string Height { get; set; }

    public string MaterialQuality { get; set; }
    
    public string BasicPerformance { get; set; }

    public string Usage { get; set; }

    public string Unit { get; set; }

    public string IsProcess { get; set; }

    public string InstallationCode { get; set; }

    public string Remark { get; set; }

    public ArchitectureListMaterialTag Tag { get; set; }

    public string OptionalSerial { get; set; }

    public void SetParentId(Guid id)
    {
        ParentId = id;
        if (Id == Guid.Empty)
        {
            Id = Guid.NewGuid();
        }
    }


    public ArchitectureListMaterial Clone(Guid parentId)
    {
        var clone = new ArchitectureListMaterial();
        if (clone.Id == Guid.Empty)
            clone.Id = Guid.NewGuid();
        clone.ParentId = parentId;
        clone.Composition = Composition;
        clone.Name = Name;
        clone.Code = Code;
        clone.Length = Length;
        clone.Width = Width;
        clone.Height = Height;
        clone.MaterialQuality = MaterialQuality;
        clone.BasicPerformance = BasicPerformance;
        clone.Usage = Usage;
        clone.Unit = Unit;
        clone.IsProcess = IsProcess;
        clone.InstallationCode = InstallationCode;
        clone.Remark = Remark;
        clone.Tag = Tag;
        clone.OptionalSerial = OptionalSerial;
        return clone;
    }

    public void SetCopyName()
    {
        this.Code = "";
    }
}