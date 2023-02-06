using MongoDB.Driver.Linq;
using PlantCatalog.Contract.Enum;
using System.Text;
using GardenLog.SharedInfrastructure.Extensions;

namespace PlantHarvest.Api.Schedules;

public class IndoorSowScheduler : SchedulerBase, IScheduler
{
    public bool CanSchedule(PlantGrowInstructionViewModel growInstruction)
    {
        return growInstruction.PlantingMethod == plant.PlantingMethodEnum.SeedIndoors && growInstruction.StartSeedWeeksAheadOfWeatherCondition.HasValue;
    }

    public CreatePlantScheduleCommand Schedule(PlantHarvestCycle plantHarvest, PlantGrowInstructionViewModel growInstruction, GardenViewModel garden,int? daysToMaturityMin, int? daysToMaturityMax)
    {
        DateTime? startDate = GetStartDateBasedOnWeatherCondition(growInstruction.StartSeedAheadOfWeatherCondition,
                                   growInstruction.StartSeedWeeksAheadOfWeatherCondition.Value,
                                   garden);

        if (startDate.HasValue)
        {
            StringBuilder sb = new();
            if (plantHarvest.DesiredNumberOfPlants.HasValue) sb.Append($"Desired number of plants: {plantHarvest.DesiredNumberOfPlants}. " );
            if (!string.IsNullOrEmpty(plantHarvest.SeedCompanyName)) sb.Append($"Seeds from {plantHarvest.SeedCompanyName}. ");
            if (growInstruction.FertilizerForSeedlings != FertilizerEnum.Unspecified) sb.Append($"Fertilize with {growInstruction.FertilizerForSeedlings.GetDescription()}. ");
            sb.Append(growInstruction.StartSeedInstructions.ToString());

            return new CreatePlantScheduleCommand()
            {
                TaskType = WorkLogReasonEnum.SowIndoors,
                StartDate = startDate.Value,
                EndDate = startDate.Value.AddDays(7 * growInstruction.StartSeedWeeksRange.Value),
                IsSystemGenerated = true,
                Notes = sb.ToString()
            };
        }

        return null;
    }
}
