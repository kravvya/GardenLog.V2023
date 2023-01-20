namespace PlantHarvest.Contract.ViewModels;

public record PlanHarvestCycleViewModel:PlanHarvestCycleBase
{
    public string PlanHarvestCycleId { get; set; }=string.Empty;
}

public class PlanHarvestCycleViewModelValidator : PlanHarvestCycleValidator<PlanHarvestCycleViewModel>
{
    public PlanHarvestCycleViewModelValidator()
    {
    }
}