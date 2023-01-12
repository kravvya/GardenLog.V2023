using PlantCatalog.Contract.Commands;

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
    public int GrowInstructionsCount
    {
        get{
            return _growInstructions.Count();
        }
    }
    public int VarietyCount { get; private set; }

    private readonly List<string> _tags = new();
    public IReadOnlyCollection<string> Tags => _tags.AsReadOnly();

    private readonly List<string> _varietyColors = new();
    public IReadOnlyCollection<string> VarietyColors => _varietyColors.AsReadOnly();

    private readonly List<PlantGrowInstruction> _growInstructions = new();
    public IReadOnlyCollection<PlantGrowInstruction> GrowInstructions => _growInstructions.AsReadOnly();

    private Plant() { }

    private Plant(string Name, string Description, string Color, PlantLifecycleEnum Lifecycle, PlantTypeEnum Type, MoistureRequirementEnum MoistureRequirement
        ,LightRequirementEnum LightRequirement, GrowToleranceEnum GrowTolerance, string GardenTip, int? SeedViableForYears
        ,int GrowInstructionCount, int VarietyCount, List<string> Tags, List<string> VarietyColors, List<PlantGrowInstruction> GrowInstructions)
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
        this.VarietyCount= VarietyCount;
        _tags = Tags;
        _varietyColors = VarietyColors;
        _growInstructions = GrowInstructions;
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
        IList<string> tags,
        IList<string> varietyColors
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
        plant._varietyColors.AddRange(varietyColors);

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
        List<string> tags,
        List<string> varietyColors)
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

        UpdateCollection<string>(this._tags, tags);
        UpdateCollection<string>(this._varietyColors, varietyColors);

        this.DomainEvents.Add(
           new PlantEvent(this, PlantEventTriggerEnum.PlantUpdated, new TriggerEntity(EntityTypeEnum.Plant, this.Id)));
    }

    private static void UpdateCollection<T>(List<T> existingList, List<T> newList)
    {
        var elementsToRemove = existingList.Where(t => !newList.Contains(t));
        if (elementsToRemove.Any())
        {
            //logic to do something in case if tags are in use
            existingList.RemoveAll(t => elementsToRemove.Contains(t));
        }

        newList.RemoveAll(t => existingList.Contains(t));

        existingList.AddRange(newList);
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

    public string AddPlantGrowInstruction(CreatePlantGrowInstructionCommand command)
    {
        var instruction = PlantGrowInstruction.Create(command);

        this._growInstructions.Add(instruction);

        this.DomainEvents.Add(
          new PlantEvent(this, PlantEventTriggerEnum.GrowInstructionAddedToPlant, new TriggerEntity(EntityTypeEnum.GrowingInstruction, instruction.Id)));

        return instruction.Id;
    }

    public void UpdatePlantGrowInstructions(UpdatePlantGrowInstructionCommand command)
    {
        this.GrowInstructions.First(i => i.Id == command.PlantGrowInstructionId).Update(command, AddChildDomainEvent);
    }

    public void DeletePlantGrowInstruction(string plantGrowInstructionId)
    {
        this._growInstructions.RemoveAll(i => i.Id == plantGrowInstructionId);

        AddChildDomainEvent(PlantEventTriggerEnum.GrowInstructionDeleted, new TriggerEntity(EntityTypeEnum.GrowingInstruction, plantGrowInstructionId));

    }

    #endregion

    #region Plant Variety

    public PlantVariety AddPlantVariety(CreatePlantVarietyCommand command)
    {
        var variety = PlantVariety.Create(command, this.Name);
        this.VarietyCount += 1;
                
        this.DomainEvents.Add(
          new PlantEvent(this, PlantEventTriggerEnum.PlantVarietyCreated, new TriggerEntity(EntityTypeEnum.PlantVariety, variety.Id)));

        return variety;
    }

    public void UpdatePlantVariety(UpdatePlantVarietyCommand command, PlantVariety variety)
    {
        
        variety.Update(command, AddChildDomainEvent);
    }

    public void DeletePlantVariety(string varietyId)
    {
        this.VarietyCount -= 1;

        AddChildDomainEvent(PlantEventTriggerEnum.PlantVarietyDeleted, new TriggerEntity(EntityTypeEnum.PlantVariety, varietyId));

    }

    #endregion



}
