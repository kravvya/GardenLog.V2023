namespace PlantCatalog.Domain.PlantAggregate;

public class PlantVariety : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public int? DaysToMaturityMin { get; private set; }
    public int? DaysToMaturityMax { get; private set; }
    public int? HeightInInches { get; private set; }
    public string Title { get; private set; }
    public bool IsHeirloom { get; private set; }
    public MoistureRequirementEnum MoistureRequirement { get; private set; }
    public LightRequirementEnum LightRequirement { get; private set; }
    public GrowToleranceEnum GrowTolerance { get; private set; }
    public SubGroup SubGroup { get; private set; }

    private PlantVariety() { }

    public static PlantVariety Create(
        string id,
        string name,
        string description,
        int? daysToMaturityMin,
        int? daysToMaturityMax,
        int? heightInInches,
        bool isHeirloom,
        MoistureRequirementEnum moistureRequirement,
        LightRequirementEnum lightRequirement,
        GrowToleranceEnum growToleranceEnum,
        string title,
        SubGroup subGroup
      )
    {
        return new PlantVariety()
        {
            Id = id,
            Name = name ?? throw new ArgumentNullException(nameof(name)),
            Description = description ?? throw new ArgumentNullException(nameof(description)),
            DaysToMaturityMin = daysToMaturityMin,
            DaysToMaturityMax= daysToMaturityMax,
            HeightInInches = heightInInches,
            IsHeirloom = isHeirloom,
            MoistureRequirement = moistureRequirement,
            LightRequirement = lightRequirement,
            GrowTolerance = growToleranceEnum,
            Title = title,
            SubGroup = subGroup
        };

    }

    public void Update(
        PlantVarietyUpdateDto dto,
        Action<PlantEventTriggerEnum, TriggerEntity> addPlantEvent
    )
    {
        Set<string>(() => this.Name, dto.Name);
        Set<string>(() => this.Description, dto.Description);
        Set<int?>(() => this.DaysToMaturityMin, dto.DaysToMaturityMin);
        Set<int?>(() => this.DaysToMaturityMax, dto.DaysToMaturityMax);
        Set<int?>(() => this.HeightInInches, dto.HeightInInches);
        Set<bool>(() => this.IsHeirloom, dto.IsHeirloom);
        Set<MoistureRequirementEnum>(() => this.MoistureRequirement, dto.MoistureRequirement);
        Set<LightRequirementEnum>(() => this.LightRequirement, dto.LightRequirement);
        Set<GrowToleranceEnum>(() => this.GrowTolerance, dto.GrowTolerance);
        Set<string>(() => this.Title, dto.Title);

        if (SubGroup.Id != dto.SubGroup.PlantSubGroupId)
        {
            SubGroup = new SubGroup() { Id = dto.SubGroup.PlantSubGroupId, Name = dto.SubGroup.Name };
            AddDomainEvent("SubGroup");
        }
        
        if (this.DomainEvents != null && this.DomainEvents.Count > 0)
        {
            this.DomainEvents.Clear();
            addPlantEvent(PlantEventTriggerEnum.PlantVarietyUpdated, new TriggerEntity(EntityTypeEnum.PlantVariety, this.Id));
        }
    }

    protected override void AddDomainEvent(string attributeName)
    {
        this.DomainEvents.Add(
            new PlantChildEvent(PlantEventTriggerEnum.GrowInstructionUpdated, new TriggerEntity(EntityTypeEnum.GrowingInstruction, this.Id)));
    }
}

public record SubGroup
{
    public string Id { get; init; }
    public string Name { get; init; }   
}


