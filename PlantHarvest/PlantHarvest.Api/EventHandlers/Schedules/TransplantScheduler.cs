namespace PlantHarvest.Api.Schedules;

public class TransplantScheduler : SchedulerBase, IScheduler
{
    public bool CanSchedule(PlantGrowInstructionViewModel growInstruction)
    {
        return growInstruction.PlantingMethod != plant.PlantingMethodEnum.DirectSeed && growInstruction.TransplantWeeksAheadOfWeatherCondition.HasValue;
    }

    public CreatePlantScheduleCommand? Schedule(PlantHarvestCycle plantHarvest, PlantGrowInstructionViewModel growInstruction, GardenViewModel garden, int? daysToMaturityMin, int? daysToMaturityMax)
    {
        DateTime endDate;

        DateTime? startDate = GetStartDateBasedOnWeatherCondition(growInstruction.TransplantAheadOfWeatherCondition,
                                    growInstruction.TransplantWeeksAheadOfWeatherCondition.Value,
                                    garden);

        if (startDate.HasValue)
        {
            endDate = growInstruction.TransplantWeeksRange.HasValue ? startDate.Value.AddDays(7 * growInstruction.TransplantWeeksRange.Value) : startDate.Value;

            return new CreatePlantScheduleCommand()
            {
                TaskType = WorkLogReasonEnum.TransplantOutside,
                StartDate = startDate.Value,
                EndDate = endDate,
                IsSystemGenerated = true,
                Notes = growInstruction.TransplantInstructions
            };
        }

        return null;
    }
}
