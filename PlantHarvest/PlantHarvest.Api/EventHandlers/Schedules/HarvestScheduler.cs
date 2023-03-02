



namespace PlantHarvest.Api.Schedules;

public class HarvestScheduler : SchedulerBase, IScheduler
{
    public bool CanSchedule(PlantGrowInstructionViewModel growInstruction)
    {
        return growInstruction.TransplantWeeksAheadOfWeatherCondition.HasValue;
    }

    public CreatePlantScheduleCommand? Schedule(PlantHarvestCycle harvestCycle, PlantGrowInstructionViewModel growInstruction, GardenViewModel garden, int? daysToMaturityMin, int? daysToMaturityMax)
    {
        int weeksAhead = growInstruction.TransplantWeeksAheadOfWeatherCondition.HasValue ? growInstruction.TransplantWeeksAheadOfWeatherCondition.Value : 0;

        DateTime? transplantDate = GetStartDateBasedOnWeatherCondition(growInstruction.TransplantAheadOfWeatherCondition, weeksAhead, garden);

        if (transplantDate.HasValue && daysToMaturityMin.HasValue && daysToMaturityMax.HasValue)
        {
            return new CreatePlantScheduleCommand()
            {
                TaskType = WorkLogReasonEnum.Harvest,
                StartDate = transplantDate.Value.AddDays(daysToMaturityMin.Value),
                EndDate = transplantDate.Value.AddDays(daysToMaturityMax.Value),
                IsSystemGenerated = true,
                Notes = growInstruction.HarvestInstructions
            };
        }

        return null;
    }
}
