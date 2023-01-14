using ImageCatalog.Contract.Base;

namespace ImageCatalog.Contract.ViewModels;

public record ImageViewModel : ImageBase
{
    public string ImageId { get; set; }
    public DateTime CreatedDateTimeUtc { get; set; }
}
