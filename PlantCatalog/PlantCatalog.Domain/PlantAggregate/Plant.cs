namespace PlantCatalog.Domain.PlantAggregate;

public class Plant : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Color { get; private set; }
    public PlantLifecycleEnum Lifecycle { get; private set; }
    public PlantTypeEnum Type { get; private set; }
    public MoistureRequirementEnum MoistureRequirement { get; private set; }
    public LightRequirementEnum LightRequirement { get; private set; }
    public GrowToleranceEnum GrowTolerance { get; private set; }
    public string GardenTip { get; private set; }
    public int? SeedViableForYears { get; private set; }

    private readonly List<string> _tags = new();
    public IReadOnlyCollection<string> Tags => _tags.AsReadOnly();

    private readonly List<GrowInstruction> _growingInstructions = new();
    public IReadOnlyCollection<GrowInstruction> GrowingInstructions => _growingInstructions.AsReadOnly();

    private Plant() { }

    private Plant(string Name, string Description, string Color, PlantLifecycleEnum Lifecycle, PlantTypeEnum Type, MoistureRequirementEnum MoistureRequirement, LightRequirementEnum LightRequirement, GrowToleranceEnum GrowTolerance, string GardenTip, int? SeedViableForYears, List<string> Tags)//, List<GrowInstruction> GrowingInstructions)
    {
        this.Name = Name;
        this.Description = Description;
        this.Color = Color;
        this.Lifecycle = Lifecycle;
        this.Type = Type;
        this.MoistureRequirement = MoistureRequirement;
        this.LightRequirement = LightRequirement;
        this.GrowTolerance = GrowTolerance;
        this.GardenTip = GardenTip;
        this.SeedViableForYears = SeedViableForYears;
        _tags = Tags;
       // _growingInstructions= GrowingInstructions;
    }

    public static Plant Create(
        string name,
        string description,
        string color,
        PlantTypeEnum type,
        PlantLifecycleEnum lifecycle,
        MoistureRequirementEnum moistureRequirement,
        LightRequirementEnum lightRequirement,
        GrowToleranceEnum growTolerance,
        string gardenTip,
        int? seedViableForYears,
        IList<string> tags
        )
    {
        var plant = new Plant()
        {
            Id = System.Guid.NewGuid().ToString(),
            Name = name ?? throw new ArgumentNullException(nameof(name)),
            Description = description ?? throw new ArgumentNullException(nameof(description)),
            Color = color ?? throw new ArgumentNullException(nameof(color)),
            Type = type,
            Lifecycle = lifecycle,
            MoistureRequirement = moistureRequirement,
            LightRequirement = lightRequirement,
            GrowTolerance = growTolerance,
            GardenTip = gardenTip,
            SeedViableForYears = seedViableForYears
        };

        plant._tags.AddRange(tags);

        plant.DomainEvents.Add(
            new PlantEvent(plant, PlantEventTriggerEnum.PlantCreated, new TriggerEntity(EntityTypeEnum.Plant, plant.Id)));

        return plant;

    }

    public void Update(
        string name,
        string description,
        string color,
        PlantTypeEnum type,
        PlantLifecycleEnum lifecycle,
        MoistureRequirementEnum moistureRequirement,
        LightRequirementEnum lightRequirement,
        GrowToleranceEnum growTolerance,
        string gardenTip,
        int? seedViableForYears,
        List<string> tags)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Color = color ?? throw new ArgumentNullException(nameof(color));
        Type = type;
        Lifecycle = lifecycle;
        MoistureRequirement = moistureRequirement;
        LightRequirement = lightRequirement;
        GrowTolerance = growTolerance;
        GardenTip = gardenTip;
        SeedViableForYears = seedViableForYears;

        var elementsToRemove = this._tags.Where(t => !tags.Contains(t));
        if(elementsToRemove.Any())
        {
            //logic to do something in case if tags are in us
            this._tags.RemoveAll(t => elementsToRemove.Contains(t));
        }

         tags.RemoveAll(t => this._tags.Contains(t));

        this._tags.AddRange(tags);

        this.DomainEvents.Add(
           new PlantEvent(this, PlantEventTriggerEnum.PlantUpdated, new TriggerEntity(EntityTypeEnum.Plant, this.Id)));
    }

    #region Events
    protected override void AddDomainEvent(string attributeName)
    {
        this.DomainEvents.Add(
          new PlantEvent(this, PlantEventTriggerEnum.PlantUpdated, new TriggerEntity(EntityTypeEnum.Plant, this.Id)));
    }

    private void AddChildDomainEvent(PlantEventTriggerEnum trigger, TriggerEntity entity)
    {
        var newEvent = new PlantEvent(this, trigger, entity);

        if (!this.DomainEvents.Contains(newEvent))
            this.DomainEvents.Add(newEvent);
    }
    #endregion

    #region Grow Instructions

    public void AddGrowinInstruction(GrowInstruction instructions)
    {
        this._growingInstructions.Add(instructions);

        this.DomainEvents.Add(
          new PlantEvent(this, PlantEventTriggerEnum.GrowInstructionAddedToPlant, new TriggerEntity(EntityTypeEnum.GrowingInstruction, instructions.Id)));
    }

    public void UpdatePlantGrowInstructions(GrowInstructionUpdateDto dto)
    {
        this.GrowingInstructions.First(i => i.Id == dto.PlantGrowInstructionId).Update(dto, AddChildDomainEvent);
    }

    #endregion



}
