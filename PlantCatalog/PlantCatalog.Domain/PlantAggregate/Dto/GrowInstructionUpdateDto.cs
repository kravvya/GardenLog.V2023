namespace PlantCatalog.Domain.PlantAggregate.Dto;

public record GrowInstructionUpdateDto
{
    public string PlantGrowInstructionId { get; set; }
    public string Name { get; init; }
    public PlantingDepthEnum PlantingDepthInInches { get; init; }
    public int? SpacingInInches { get; init; }

    public PlantingMethodEnum PlantingMethod { get; init; }

    public WeatherConditionEnum StartSeedAheadOfWeatherCondition { get; init; }

    public int? StartSeedWeeksAheadOfWeatherCondition { get; init; }

    public HarvestSeasonEnum HarvestSeason { get; init; }

    public int? TransplantWeeksAheadOfWeatherCondition { get; init; }

    public WeatherConditionEnum TransplantAheadOfWeatherCondition { get; init; }

    public string StartSeedInstructions { get; init; }

    public string GrowingInstructions { get; init; }

    public int? StartSeedWeeksRange { get; init; }

    public int? TransplantWeeksRange { get; init; }

    public string HarvestInstructions { get; init; }

    public FertilizerEnum FertilizerAtPlanting { get; init; }

    public FertilizerEnum Fertilizer { get; init; }

    public int? FertilizeFrequencyInWeeks { get; init; }

    public int? DaysToSproutMin { get; init; }
    public int? DaysToSproutMax { get; init; }
}
