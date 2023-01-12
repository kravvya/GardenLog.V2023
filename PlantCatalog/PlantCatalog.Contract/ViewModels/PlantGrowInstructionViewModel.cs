using PlantCatalog.Contract.Validators;

namespace PlantCatalog.Contract.ViewModels;

public record PlantGrowInstructionViewModel: PlantGrowInstructionBase
{
    public string PlantGrowInstructionId { get; set; }
}

public class PlantGrowInstructionViewModelValidator : PlantGrowInstructionValidator<PlantGrowInstructionViewModel>
{
    public PlantGrowInstructionViewModelValidator()
    {
    }
}