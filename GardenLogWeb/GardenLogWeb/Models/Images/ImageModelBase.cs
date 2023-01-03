namespace GardenLogWeb.Models.Images;

public abstract record ImageModelBase
{
    public string ImageName { get; set; }
    public string Label { get; set; }
    public string RelatedEntityType { get; set; }
    public string RelatedEntityId { get; set; }
    public string FileName { get; set; }
    public string FileType { get; set; }
}
