﻿namespace PlantCatalog.Contract.Base;

public abstract record PlantGrowInstructionBase
{
    public string Name { get; set; }
    public string PlantId { get; set; }
    public HarvestSeasonEnum HarvestSeason { get; set; }
    public PlantingMethodEnum PlantingMethod { get; set; }

    public string GrowingInstructions { get; set; }
    public string HarvestInstructions { get; set; }

    public PlantingDepthEnum PlantingDepthInInches { get; set; }
    public int? SpacingInInches { get; set; }

    public string StartSeedInstructions { get; set; }
    public WeatherConditionEnum StartSeedAheadOfWeatherCondition { get; set; }
    public int? StartSeedWeeksAheadOfWeatherCondition { get; set; }
    public int? StartSeedWeeksRange { get; set; }

    public int? TransplantWeeksAheadOfWeatherCondition { get; set; }
    public WeatherConditionEnum TransplantAheadOfWeatherCondition { get; set; }
    public int? TransplantWeeksRange { get; set; }
    public string TransplantInstructions { get; set; }

    public FertilizerEnum FertilizerAtPlanting { get; set; }
    public FertilizerEnum FertilizerForSeedlings {get; set;}
    public FertilizerEnum Fertilizer { get; set; }

    public int? FertilizeFrequencyForSeedlingsInWeeks { get; set; }
    public int? FertilizeFrequencyInWeeks { get; set; }

    public int? DaysToSproutMin { get; set; }
    public int? DaysToSproutMax { get; set; }
}
