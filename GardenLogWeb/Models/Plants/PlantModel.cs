using GardenLogWeb.Models.Images;

namespace GardenLogWeb.Models.Plants;

public record PlantModel : PlantCommandBase
{
    public string PlantId { get; set; }
    public List<ImageModel> Images { get; set; }
    public string ImageFileName { get; set; }
    public string ImageLabel { get; set; }

    public int VarietyCount { get; set; }
    public int GrowCount { get; set; }
}
