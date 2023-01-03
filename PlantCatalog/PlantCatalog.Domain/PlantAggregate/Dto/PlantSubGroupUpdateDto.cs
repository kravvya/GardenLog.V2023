namespace PlantCatalog.Domain.PlantAggregate.Dto;

public record PlantSubGroupUpdateDto
{
    public string PlantSubGroupId { get; set; }
    public string Name { get; init; }
    public string Description { get; init; }
}
