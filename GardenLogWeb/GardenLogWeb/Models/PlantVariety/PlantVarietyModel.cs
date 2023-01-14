namespace GardenLogWeb.Models.PlantVariety;

public record PlantVarietyModel : PlantVarietyViewModel
{
    public List<ImageModel> Images { get; set; }
    public string ImageFileName { get; set; }
    public string ImageLabel { get; set; }
}