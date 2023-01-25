namespace PlantCatalog.Contract.Base;

public abstract record SeedVendorBase
{
    public string Name { get; set; }
    public string WebSiteUrl { get; set; }
    public bool HasPaperCatalog { get; set; }
}
