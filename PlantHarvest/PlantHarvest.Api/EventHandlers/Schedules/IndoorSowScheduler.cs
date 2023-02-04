using MongoDB.Driver.Linq;
namespace PlantHarvest.Api.Schedules;

public class IndoorSowScheduler : SchedulerBase, IScheduler
{
    public bool CanSchedule(PlantGrowInstructionViewModel growInstruction)
    {
        return growInstruction.PlantingMethod == plant.PlantingMethodEnum.SeedIndoors && growInstruction.StartSeedWeeksAheadOfWeatherCondition.HasValue;
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
                TaskType= WorkLogReasonEnum.SowIndoors,
                StartDate = startDate.Value, 
                EndDate = startDate.Value.AddDays(7 * growInstruction.StartSeedWeeksRange.Value),
                IsSystemGenerated = true,
                Notes = growInstruction.StartSeedInstructions
            };
        }

        return null;
    }
}
