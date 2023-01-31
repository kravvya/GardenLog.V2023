namespace PlantHarvest.Contract.Base;

public record PlantScheduleBase
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public WorkLogReasonEnum Task { get; set; }
    public bool IsSystemGenerated { get; set; }
}
