namespace GardenLogWeb.Pages.Schedule.Components;

public record PlantScheduleModel :PlantHarvestCycleModel
{
    public List<PlantSchedule>? PlantCalendar { get; set; }
}

public record PlantSchedule()
{
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public PlantTaskType Task { get; set; }
} 

public enum PlantTaskType : int
{
    Unknown = 0,
    SowIndoors= 1,
    SowOutdoors  = 2,
    TransplantOutside = 3,
    Harvest = 4,
    Fertilize = 5
}