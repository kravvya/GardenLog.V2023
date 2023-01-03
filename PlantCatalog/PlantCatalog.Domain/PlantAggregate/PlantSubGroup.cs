namespace PlantCatalog.Domain.PlantAggregate;

public class PlantSubGroup : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }

    private PlantSubGroup() { }

    public static PlantSubGroup Create(
        string id,
        string name,
        string description
    )
    {
        return new PlantSubGroup()
        {
            Id = id,
            Name = name ?? throw new ArgumentNullException(nameof(name))
        };

    }

    public void Update(
        PlantSubGroupUpdateDto dto,
        Action<PlantEventTriggerEnum, TriggerEntity> addPlantEvent
    )
    {
        Set<string>(() => this.Name, dto.Name ?? throw new ArgumentNullException(nameof(dto.Name)));
        Set<string>(() => this.Description, dto.Description ?? throw new ArgumentNullException(nameof(dto.Description)));

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
