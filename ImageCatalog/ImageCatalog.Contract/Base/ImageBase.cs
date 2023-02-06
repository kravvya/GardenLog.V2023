using GardenLog.SharedKernel;
using GardenLog.SharedKernel.Enum;

namespace ImageCatalog.Contract.Base;

public abstract record ImageBase
{
    public string ImageName { get; set; }
    public string Label { get; set; }
    public RelatedEntityTypEnum RelatedEntityType { get; set; }
    public string RelatedEntityId { get; set; }
    public string FileName { get; set; }
    public string FileType { get; set; }
    public List<RelatedEntity> RelatedEntities { get; set; } = new();
}
