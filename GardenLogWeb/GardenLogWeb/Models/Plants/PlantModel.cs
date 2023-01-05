using GardenLogWeb.Models.Images;


namespace GardenLogWeb.Models.Plants;

public record PlantModel : PlantViewModel
{
    public List<ImageModel> Images { get; set; }
    public string ImageFileName { get; set; }
    public string ImageLabel { get; set; }

    public int VarietyCount { get; set; }
    public int GrowCount { get; set; }
}
