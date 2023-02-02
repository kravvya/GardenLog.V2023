namespace PlantHarvest.Orchestrator.Schedules;

public class HarvestScheduler : SchedulerBase, IScheduler
{
    public bool CanSchedule(PlantGrowInstructionViewModel growInstruction)
    {
        return true;
    }

    public CreatePlantScheduleCommand Schedule(PlantGrowInstructionViewModel growInstruction, GardenViewModel garden, int? daysToMaturityMin, int? daysToMaturityMax)
    {
        DateTime? transplantDate = GetStartDateBasedOnWeatherCondition(growInstruction.TransplantAheadOfWeatherCondition, 
                                    growInstruction.TransplantWeeksAheadOfWeatherCondition.Value,
                                    garden);

        if (transplantDate.HasValue && daysToMaturityMin.HasValue && daysToMaturityMax.HasValue)
        {
            return new CreatePlantScheduleCommand()
            {
                TaskType = harvest.WorkLogReasonEnum.Harvest,
                StartDate = transplantDate.Value.AddDays(daysToMaturityMin.Value),
                EndDate = transplantDate.Value.AddDays(daysToMaturityMax.Value),
                IsSystemGenerated = true,
                Notes = growInstruction.HarvestInstructions
            };
        }

        return null;
    }
}
