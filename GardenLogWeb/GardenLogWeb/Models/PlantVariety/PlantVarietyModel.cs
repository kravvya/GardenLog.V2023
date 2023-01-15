

namespace GardenLogWeb.Models.PlantVariety;

public record PlantVarietyModel : PlantVarietyViewModel
{
    public List<ImageViewModel> Images { get; set; }
    public string ImageFileName { get; set; }
    public string ImageLabel { get; set; }
}