using MongoDB.Driver.Linq;


namespace PlantHarvest.Api.Schedules;

public class TransplantScheduler : SchedulerBase, IScheduler
{
    public bool CanSchedule(PlantGrowInstructionViewModel growInstruction)
    {
        return growInstruction.PlantingMethod != plant.PlantingMethodEnum.DirectSeed && growInstruction.TransplantWeeksAheadOfWeatherCondition.HasValue;
    }

    public CreatePlantScheduleCommand Schedule(PlantGrowInstructionViewModel growInstruction, GardenViewModel garden, int? daysToMaturityMin, int? daysToMaturityMax)
    {
        DateTime? startDate = GetStartDateBasedOnWeatherCondition(growInstruction.TransplantAheadOfWeatherCondition, 
                                    growInstruction.TransplantWeeksAheadOfWeatherCondition.Value,
                                    garden);

        if (startDate.HasValue)
        {
            return new CreatePlantScheduleCommand()
            {
                TaskType = WorkLogReasonEnum.TransplantOutside,
                StartDate = startDate.Value,
                EndDate = startDate.Value.AddDays(7 * growInstruction.StartSeedWeeksRange.Value),
                IsSystemGenerated = true,
                Notes = growInstruction.TransplantInstructions
            };
        }

        return null;
    }
}
