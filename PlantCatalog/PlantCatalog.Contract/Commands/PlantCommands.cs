using PlantCatalog.Contract.Validators;

namespace PlantCatalog.Contract.Commands;

public record CreatePlantCommand : PlantBase
{ }

public record UpdatePlantCommand : PlantBase
{
    public string PlantId { get; init; }
}

public class CreatePlantCommandValidator : PlantValidator<CreatePlantCommand>
{
    public CreatePlantCommandValidator()
    {
    }
}
