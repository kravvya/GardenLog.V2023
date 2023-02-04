namespace PlantHarvest.Contract.Query;

public record  PlantTaskSearch()
{
    public string PlantHarvestCycleId { get; set; }
    public WorkLogReasonEnum? Reason { get; set; }
}
