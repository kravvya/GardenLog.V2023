using MongoDB.Driver.Linq;
namespace PlantHarvest.Orchestrator.Schedules;

public class IndoorSowScheduler : SchedulerBase, IScheduler
{
    public bool CanSchedule(PlantGrowInstructionViewModel growInstruction)
    {
        return growInstruction.PlantingMethod == PlantingMethodEnum.SeedIndoors && growInstruction.StartSeedWeeksAheadOfWeatherCondition.HasValue;
    }

    public CreatePlantScheduleCommand Schedule(PlantGrowInstructionViewModel growInstruction, GardenViewModel garden,int? daysToMaturityMin, int? daysToMaturityMax)
    {
        DateTime? startDate = GetStartDateBasedOnWeatherCondition(growInstruction.StartSeedAheadOfWeatherCondition,
                                   growInstruction.StartSeedWeeksAheadOfWeatherCondition.Value,
                                   garden);

        if (startDate.HasValue)
        {
            return new CreatePlantScheduleCommand()
            {
                TaskType= harvest.WorkLogReasonEnum.SowIndoors,
                StartDate = startDate.Value, 
                EndDate = startDate.Value.AddDays(7 * growInstruction.StartSeedWeeksRange.Value),
                IsSystemGenerated = true,
                Notes = growInstruction.StartSeedInstructions
            };
        }

        return null;
    }
}
