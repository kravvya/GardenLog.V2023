namespace ImageCatalog.Api.Data;

public record AzureBlobConfig
{
    public const string Key = "AzureBlobConfig";

    public string AccountName { get; set; }
    public string ImageContainer { get; set; }
    public string ThumbnailContainer { get; set; }
}



