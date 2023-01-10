using PlantCatalog.Contract.Validators;

namespace GardenLogWeb.Models.Plants;

public record PlantModel : PlantViewModel
{
    public List<ImageModel> Images { get; set; }
    public string ImageFileName { get; set; }
    public string ImageLabel { get; set; }
}

public class PlantModelValidator : PlantValidator<PlantModel>
{
    public PlantModelValidator()
    {
    }
}