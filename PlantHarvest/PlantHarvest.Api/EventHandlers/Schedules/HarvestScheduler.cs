using p = PlantCatalog.Contract.Enum;



namespace PlantHarvest.Api.Schedules;

public class HarvestScheduler : SchedulerBase, IScheduler
{
    public bool CanSchedule(PlantGrowInstructionViewModel growInstruction)
    {
        return true;
    }

    public CreatePlantScheduleCommand? Schedule(PlantHarvestCycle harvestCycle, PlantGrowInstructionViewModel growInstruction, GardenViewModel garden, int? daysToMaturityMin, int? daysToMaturityMax)
    {
        DateTime originDate;

        switch (growInstruction.PlantingMethod)
        {
            case p.PlantingMethodEnum.SeedIndoors:
            case p.PlantingMethodEnum.Transplanting:
                if (growInstruction.TransplantAheadOfWeatherCondition == p.WeatherConditionEnum.Unspecified) return null;

                int weeksAhead = growInstruction.TransplantWeeksAheadOfWeatherCondition.HasValue ? growInstruction.TransplantWeeksAheadOfWeatherCondition.Value : 0;

                DateTime? transplantDate = GetStartDateBasedOnWeatherCondition(growInstruction.TransplantAheadOfWeatherCondition, weeksAhead, garden);

                if (transplantDate.HasValue) { originDate = transplantDate.Value; } else return null;

                break;
            case p.PlantingMethodEnum.DirectSeed:
                if (growInstruction.StartSeedAheadOfWeatherCondition == p.WeatherConditionEnum.Unspecified) return null;

                weeksAhead = growInstruction.StartSeedWeeksAheadOfWeatherCondition.HasValue ? growInstruction.StartSeedWeeksAheadOfWeatherCondition.Value : 0;

                DateTime? seedDate = GetStartDateBasedOnWeatherCondition(growInstruction.StartSeedAheadOfWeatherCondition, weeksAhead, garden);

                if (seedDate.HasValue) { originDate = seedDate.Value; } else return null;

                break;
            default:
                return null;
        }

        if (daysToMaturityMin.HasValue && daysToMaturityMax.HasValue)
        {
            return new CreatePlantScheduleCommand()
            {
                TaskType = WorkLogReasonEnum.Harvest,
                StartDate = originDate.AddDays(daysToMaturityMin.Value),
                EndDate = originDate.AddDays(daysToMaturityMax.Value),
                IsSystemGenerated = true,
                Notes = growInstruction.HarvestInstructions
            };
        }

        return null;
    }
}
