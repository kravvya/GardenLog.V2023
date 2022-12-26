using GardenLogWeb.Models.Images;

namespace GardenLogWeb.Models.PlantVariety;

public record PlantVarietyModel : PlantVarietyCommandBase
{
    public string PlantVarietyId { get; set; }
    public List<ImageModel> Images { get; set; }
    public string ImageFileName { get; set; }
    public string ImageLabel { get; set; }
}