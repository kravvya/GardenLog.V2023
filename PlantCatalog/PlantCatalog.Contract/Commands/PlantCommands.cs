namespace PlantCatalog.Contract.Commands;

public record CreatePlantCommand : PlantBase
{ }

public record UpdatePlantCommand : PlantBase
{
    public string PlantId { get; init; }
}
