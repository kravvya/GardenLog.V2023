



namespace PlantHarvest.Api.Schedules;

public class HarvestScheduler : SchedulerBase, IScheduler
{
    public bool CanSchedule(PlantGrowInstructionViewModel growInstruction)
    {
        return growInstruction.TransplantWeeksAheadOfWeatherCondition.HasValue;
    }

    public CreatePlantScheduleCommand? Schedule(PlantHarvestCycle harvestCycle, PlantGrowInstructionViewModel growInstruction, GardenViewModel garden, int? daysToMaturityMin, int? daysToMaturityMax)
    {
        DateTime? transplantDate = GetStartDateBasedOnWeatherCondition(growInstruction.TransplantAheadOfWeatherCondition, 
                                    growInstruction.TransplantWeeksAheadOfWeatherCondition!.Value,
                                    garden);

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
