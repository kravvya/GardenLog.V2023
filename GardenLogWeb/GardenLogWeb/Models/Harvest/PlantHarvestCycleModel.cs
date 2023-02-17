namespace GardenLogWeb.Models.Harvest;

public record PlantHarvestCycleModel : PlantHarvestCycleViewModel
{
    public new List<GardenBedPlantHarvestCycleModel>? GardenBedLayout { get; set; }

    public List<ImageViewModel> Images { get; set; }

    public string GetPlantName()
    {
        if (string.IsNullOrEmpty(PlantVarietyName))
        {
            return PlantName;
        }
        else
        {
            return $"{PlantName} - {PlantVarietyName}";
        }
    }
}
