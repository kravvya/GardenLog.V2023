using PlantCatalog.Contract.Commands;

namespace PlantCatalog.Domain.PlantAggregate;

public class PlantVariety : BaseEntity, IEntity
{
    public string PlantId { get; private set; }
    public string PlantName { get; set; }
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

    private readonly List<string> _tags = new();
    public IReadOnlyCollection<string> Tags => _tags.AsReadOnly();

    private readonly List<string> _colors = new();
    public IReadOnlyCollection<string> Colors => _colors.AsReadOnly();


    private PlantVariety() { }

    public static PlantVariety Create(
        string plantId, 
        string plantName,
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
        List<string> tags,
        List<string> colors,
         Action<PlantEventTriggerEnum, TriggerEntity> addPlantEvent
      )
    {
        var variety =  new PlantVariety()
        {
            Id = Guid.NewGuid().ToString(),
            PlantName = plantName,
            PlantId = plantId,
            Name = name ?? throw new ArgumentNullException(nameof(name)),
            Description = description ?? throw new ArgumentNullException(nameof(description)),
            DaysToMaturityMin = daysToMaturityMin,
            DaysToMaturityMax= daysToMaturityMax,
            HeightInInches = heightInInches,
            IsHeirloom = isHeirloom,
            MoistureRequirement = moistureRequirement,
            LightRequirement = lightRequirement,
            GrowTolerance = growToleranceEnum,
            Title = title
        };
        variety._tags.AddRange(tags);
        variety._colors.AddRange(colors);

        if (variety.DomainEvents != null && variety.DomainEvents.Count > 0)
        {
            variety.DomainEvents.Clear();
            addPlantEvent(PlantEventTriggerEnum.PlantVarietyCreated, new TriggerEntity(EntityTypeEnum.PlantVariety, variety.Id));
        }

        return variety;
    }

    public void Update(
        UpdatePlantVarietyCommand command,
        Action<PlantEventTriggerEnum, TriggerEntity> addPlantEvent
    )
    {
        Set<string>(() => this.Name, command.Name);
        Set<string>(() => this.Description, command.Description);
        Set<int?>(() => this.DaysToMaturityMin, command.DaysToMaturityMin);
        Set<int?>(() => this.DaysToMaturityMax, command.DaysToMaturityMax);
        Set<int?>(() => this.HeightInInches, command.HeightInInches);
        Set<bool>(() => this.IsHeirloom, command.IsHeirloom);
        Set<MoistureRequirementEnum>(() => this.MoistureRequirement, command.MoistureRequirement);
        Set<LightRequirementEnum>(() => this.LightRequirement, command.LightRequirement);
        Set<GrowToleranceEnum>(() => this.GrowTolerance, command.GrowTolerance);
        Set<string>(() => this.Title, command.Title);

        SetCollection<string>(() => this._tags, command.Tags);
        SetCollection<string>(() => this._colors, command.Colors);

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


