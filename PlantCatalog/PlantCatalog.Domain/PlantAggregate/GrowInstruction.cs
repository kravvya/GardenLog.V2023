namespace PlantCatalog.Domain.PlantAggregate;

public class GrowInstruction : BaseEntity
{
    public string Name { get; private set; }

    public PlantingDepthEnum PlantingDepthInInches { get; private set; }

    public int? SpacingInInches { get; private set; }

    public PlantingMethodEnum PlantingMethod { get; private set; }

    public WeatherConditionEnum StartSeedAheadOfWeatherCondition { get; private set; }

    public int? StartSeedWeeksAheadOfWeatherCondition { get; private set; }

    public HarvestSeasonEnum HarvestSeason { get; private set; }

    public int? TransplantWeeksAheadOfWeatherCondition { get; private set; }

    public WeatherConditionEnum TransplantAheadOfWeatherCondition { get; private set; }

    public string StartSeedInstructions { get; private set; }

    public string GrowingInstructions { get; private set; }

    public int? StartSeedWeeksRange { get; private set; }

    public int? TransplantWeeksRange { get; private set; }

    public string HarvestInstructions { get; private set; }

    public FertilizerEnum FertilizerAtPlanting { get; private set; }

    public FertilizerEnum Fertilizer { get; private set; }

    public int? FertilizeFrequencyInWeeks { get; private set; }

    public int? DaysToSproutMin { get; private set; }
    public int? DaysToSproutMax { get; private set; }

    private GrowInstruction() { }

    public static GrowInstruction Create(
        string id,
        string name,
        PlantingDepthEnum plantingDepthInInches,
        int? spacingInInches,
        PlantingMethodEnum plantingMethod,
        WeatherConditionEnum startSeedAheadOfWeatherCondition,
        int? startSeedWeeksAheadOfWeatherCondition,
        int? startSeedWeeksRange,
        HarvestSeasonEnum harvestSeason,
        int? transplantWeeksAheadOfWeatherCondition,
        WeatherConditionEnum transplantAheadOfWeatherCondition,
        int? transplantWeeksRange,
        string startSeedInstructions,
        string growingInstructions,
        string harvestInstructions,
        FertilizerEnum fertilizerAtPlanting,
        FertilizerEnum fertilizer,
        int? fertilizeFrequencyInWeeks,
        int? daysToSproutMin,
        int? daysToSproutMax
    )
    {
        return new GrowInstruction()
        {
            Id = id,
            Name = name ?? throw new ArgumentNullException(nameof(name)),
            PlantingDepthInInches = plantingDepthInInches,
            SpacingInInches = spacingInInches,
            PlantingMethod = plantingMethod,
            StartSeedAheadOfWeatherCondition = startSeedAheadOfWeatherCondition,
            StartSeedWeeksAheadOfWeatherCondition = startSeedWeeksAheadOfWeatherCondition,
            StartSeedWeeksRange = startSeedWeeksRange,
            StartSeedInstructions = startSeedInstructions,
            HarvestSeason = harvestSeason,
            TransplantAheadOfWeatherCondition = transplantAheadOfWeatherCondition,
            TransplantWeeksAheadOfWeatherCondition = transplantWeeksAheadOfWeatherCondition,
            TransplantWeeksRange = transplantWeeksRange,
            GrowingInstructions = growingInstructions,
            HarvestInstructions = harvestInstructions,
            FertilizerAtPlanting = fertilizerAtPlanting,
            Fertilizer=fertilizer,
            FertilizeFrequencyInWeeks= fertilizeFrequencyInWeeks,
            DaysToSproutMin=daysToSproutMin,
            DaysToSproutMax=daysToSproutMax
        };

    }

    public void Update(
        GrowInstructionUpdateDto dto,
        Action<PlantEventTriggerEnum, TriggerEntity> addPlantEvent
    )
    {
        Set<string>(() => this.Name, dto.Name ?? throw new ArgumentNullException(nameof(dto.Name)));
        Set<PlantingDepthEnum>(() => this.PlantingDepthInInches, dto.PlantingDepthInInches);
        Set<int?>(() => this.SpacingInInches, dto.SpacingInInches);
        Set<PlantingMethodEnum>(() => this.PlantingMethod, dto.PlantingMethod);
        Set<WeatherConditionEnum>(() => this.StartSeedAheadOfWeatherCondition, dto.StartSeedAheadOfWeatherCondition);
        Set<int?>(() => this.StartSeedWeeksAheadOfWeatherCondition, dto.StartSeedWeeksAheadOfWeatherCondition);
        Set<int?>(() => this.StartSeedWeeksRange, dto.StartSeedWeeksRange);
        Set<string>(() => this.StartSeedInstructions, dto.StartSeedInstructions);
        Set<HarvestSeasonEnum>(() => this.HarvestSeason, dto.HarvestSeason);
        Set<WeatherConditionEnum>(() => this.TransplantAheadOfWeatherCondition, dto.TransplantAheadOfWeatherCondition);
        Set<int?>(() => this.TransplantWeeksAheadOfWeatherCondition, dto.TransplantWeeksAheadOfWeatherCondition);
        Set<int?>(() => this.TransplantWeeksRange, dto.TransplantWeeksRange);
        Set<string>(() => this.GrowingInstructions, dto.GrowingInstructions);
        Set<string>(() => this.HarvestInstructions, dto.HarvestInstructions);
        Set<FertilizerEnum>(() => this.FertilizerAtPlanting, dto.FertilizerAtPlanting);
        Set<FertilizerEnum>(() => this.Fertilizer, dto.Fertilizer);
        Set<int?>(() => this.FertilizeFrequencyInWeeks, dto.FertilizeFrequencyInWeeks);
        Set<int?>(() => this.DaysToSproutMin, dto.DaysToSproutMin);
        Set<int?>(() => this.DaysToSproutMax, dto.DaysToSproutMax);

        if (this.DomainEvents != null && this.DomainEvents.Count > 0)
        {
            this.DomainEvents.Clear();
            addPlantEvent(PlantEventTriggerEnum.GrowInstructionUpdated, new TriggerEntity(EntityTypeEnum.GrowingInstruction, this.Id));
        }
    }

    protected override void AddDomainEvent(string attributeName)
    {
        this.DomainEvents.Add(
            new PlantChildEvent(PlantEventTriggerEnum.GrowInstructionUpdated, new TriggerEntity(EntityTypeEnum.GrowingInstruction, this.Id)));
    }
}
