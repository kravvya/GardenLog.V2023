namespace PlantHarvest.Contract.ViewModels;

public record PlantHarvestCycleViewModel:PlantHarvestCycleBase
{
    public string PlantHarvestCycleId { get; set; }=string.Empty;
}

public class PlantHarvestCycleViewModelValidator : PlantHarvestCycleValidator<PlantHarvestCycleViewModel>
{
    public PlantHarvestCycleViewModelValidator()
    {
    }
}