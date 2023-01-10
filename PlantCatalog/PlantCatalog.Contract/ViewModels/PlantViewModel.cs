namespace PlantCatalog.Contract.ViewModels;

public record PlantViewModel: PlantBase
{
    public string PlantId { get; set; }
    public int VarietyCount { get; set; }
    public int GrowCount { get; set; }
}
