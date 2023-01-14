using ImageCatalog.Contract.Enum;

namespace ImageCatalog.Contract.Base;

public abstract record ImageBase
{
    public string ImageName { get; set; }
    public string Label { get; set; }
    public ImageEntityEnum RelatedEntityType { get; set; }
    public string RelatedEntityId { get; set; }
    public string FileName { get; set; }
    public string FileType { get; set; }
}
