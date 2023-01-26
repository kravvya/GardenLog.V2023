namespace PlantCatalog.Contract.ViewModels;

public record PlantNameOnlyViewModel
{
    public string PlantId { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
}
