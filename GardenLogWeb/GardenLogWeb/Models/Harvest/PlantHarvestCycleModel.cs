namespace GardenLogWeb.Models.Harvest;

public record PlantHarvestCycleModel : PlantHarvestCycleViewModel
{
    public List<ImageViewModel> Images { get; set; }
}
