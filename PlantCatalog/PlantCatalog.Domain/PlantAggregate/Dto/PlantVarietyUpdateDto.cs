namespace PlantCatalog.Domain.PlantAggregate.Dto
{
    public record PlantVarietyUpdateDto
    {
        public string PlantVarietyId { get; init; }
        public string Name { get; init; }
        public int? DaysToMaturityMin { get; init; }
        public int? DaysToMaturityMax { get; init; }
        public int? HeightInInches { get; init; }
        public bool IsHeirloom { get; init; }
        public string Description { get; init; }
        public string Title { get; init; }
        public MoistureRequirementEnum MoistureRequirement { get; init; }
        public LightRequirementEnum LightRequirement { get; init; }
        public GrowToleranceEnum GrowTolerance { get; init; }
    }
}
