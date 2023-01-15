using PlantCatalog.Contract.Commands;

namespace PlantCatalog.Domain.PlantAggregate;

public class PlantGrowInstruction : BaseEntity
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
    public FertilizerEnum FertilizerForSeedlings { get; private set; }
    public FertilizerEnum Fertilizer { get; private set; }

    public int? FertilizerFrequencyForSeedlingsInWeeks { get; private set; }
    public int? FertilizeFrequencyInWeeks { get; private set; }

    public int? DaysToSproutMin { get; private set; }
    public int? DaysToSproutMax { get; private set; }

    public string TransplantInstructions { get; private set; }

    private PlantGrowInstruction() { }

    public static PlantGrowInstruction Create(CreatePlantGrowInstructionCommand command)
    {
        return new PlantGrowInstruction()
        {
            Id = Guid.NewGuid().ToString(),
            Name = command.Name ?? throw new ArgumentNullException(nameof(command.Name)),
            PlantingDepthInInches = command.PlantingDepthInInches,
            SpacingInInches = command.SpacingInInches,
            PlantingMethod = command.PlantingMethod,
            StartSeedAheadOfWeatherCondition = command.StartSeedAheadOfWeatherCondition,
            StartSeedWeeksAheadOfWeatherCondition = command.StartSeedWeeksAheadOfWeatherCondition,
            StartSeedWeeksRange = command.StartSeedWeeksRange,
            StartSeedInstructions = command.StartSeedInstructions,
            HarvestSeason = command.HarvestSeason,
            TransplantAheadOfWeatherCondition = command.TransplantAheadOfWeatherCondition,
            TransplantWeeksAheadOfWeatherCondition = command.TransplantWeeksAheadOfWeatherCondition,
            TransplantWeeksRange = command.TransplantWeeksRange,
            GrowingInstructions = command.GrowingInstructions,
            HarvestInstructions = command.HarvestInstructions,
            FertilizerAtPlanting = command.FertilizerAtPlanting,
            FertilizerForSeedlings= command.FertilizerForSeedlings,
            Fertilizer= command.Fertilizer,
            FertilizerFrequencyForSeedlingsInWeeks= command.FertilizeFrequencyForSeedlingsInWeeks,
            FertilizeFrequencyInWeeks= command.FertilizeFrequencyInWeeks,
            DaysToSproutMin= command.DaysToSproutMin,
            DaysToSproutMax= command.DaysToSproutMax,
            TransplantInstructions= command.TransplantInstructions
        };

    }

    public void Update(
        UpdatePlantGrowInstructionCommand command,
        Action<PlantEventTriggerEnum, TriggerEntity> addPlantEvent
    )
    {
        Set<string>(() => this.Name, command.Name ?? throw new ArgumentNullException(nameof(command.Name)));
        Set<PlantingDepthEnum>(() => this.PlantingDepthInInches, command.PlantingDepthInInches);
        Set<int?>(() => this.SpacingInInches, command.SpacingInInches);
        Set<PlantingMethodEnum>(() => this.PlantingMethod, command.PlantingMethod);
        Set<WeatherConditionEnum>(() => this.StartSeedAheadOfWeatherCondition, command.StartSeedAheadOfWeatherCondition);
        Set<int?>(() => this.StartSeedWeeksAheadOfWeatherCondition, command.StartSeedWeeksAheadOfWeatherCondition);
        Set<int?>(() => this.StartSeedWeeksRange, command.StartSeedWeeksRange);
        Set<string>(() => this.StartSeedInstructions, command.StartSeedInstructions);
        Set<HarvestSeasonEnum>(() => this.HarvestSeason, command.HarvestSeason);
        Set<WeatherConditionEnum>(() => this.TransplantAheadOfWeatherCondition, command.TransplantAheadOfWeatherCondition);
        Set<int?>(() => this.TransplantWeeksAheadOfWeatherCondition, command.TransplantWeeksAheadOfWeatherCondition);
        Set<int?>(() => this.TransplantWeeksRange, command.TransplantWeeksRange);
        Set<string>(() => this.GrowingInstructions, command.GrowingInstructions);
        Set<string>(() => this.HarvestInstructions, command.HarvestInstructions);
        Set<FertilizerEnum>(() => this.FertilizerAtPlanting, command.FertilizerAtPlanting);
        Set<FertilizerEnum>(() => this.FertilizerForSeedlings, command.FertilizerForSeedlings);
        Set<FertilizerEnum>(() => this.Fertilizer, command.Fertilizer);
        Set<int?>(() => this.FertilizerFrequencyForSeedlingsInWeeks, command.FertilizeFrequencyForSeedlingsInWeeks);
        Set<int?>(() => this.FertilizeFrequencyInWeeks, command.FertilizeFrequencyInWeeks);
        Set<int?>(() => this.DaysToSproutMin, command.DaysToSproutMin);
        Set<int?>(() => this.DaysToSproutMax, command.DaysToSproutMax);
        Set<string>(() => this.TransplantInstructions, command.TransplantInstructions);

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
