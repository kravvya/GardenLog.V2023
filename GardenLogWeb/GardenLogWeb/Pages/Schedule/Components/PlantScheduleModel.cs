namespace GardenLogWeb.Pages.Schedule.Components;

public record PlantScheduleModel :PlantHarvestCycleModel
{
    public List<PlantSchedule>? PlantCalendar { get; set; }
}

public record PlantSchedule()
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public WorkLogReasonEnum Task { get; set; }
    public bool IsSystemGenerated { get; set; }
} 

