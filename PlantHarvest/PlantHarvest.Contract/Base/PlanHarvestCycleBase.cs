namespace PlantHarvest.Contract.Base;

public record PlanHarvestCycleBase
{
    public string HarvestCycleId { get; set; } = string.Empty;

    public string PlantId { get; set; } = string.Empty;
    
    public string? PlantGrowthInstructionId { get; set; }
    public string? GardenBedId { get; set; }

    public int? NumberOfPlants { get; set; }
   
    public string Notes { get; set; } = string.Empty;
}
