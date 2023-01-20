namespace PlantHarvest.Domain.HarvestAggregate;

public class PlanHarvestCycle : BaseEntity, IEntity
{
    public string UserProfileId { get; private set; }
    public string HarvestCyclId { get; set; }

    public string PlantId { get; set; }

    public string? PlantGrowthInstructionId { get; set; }
    public string? GardenBedId { get; set; }

    public int? NumberOfPlants { get; set; }

    public string Notes { get; set; } = string.Empty;

    private PlanHarvestCycle()
    {
    }

    public static PlanHarvestCycle Create(PlanHarvestCycleBase plant, string userProfileId)
    {
        return new PlanHarvestCycle()
        {
            Id = Guid.NewGuid().ToString(),
            UserProfileId = userProfileId,
            PlantId = plant.PlantId,
            GardenBedId = plant.GardenBedId,
            PlantGrowthInstructionId = plant.PlantGrowthInstructionId,
            HarvestCyclId = plant.HarvestCycleId,
            NumberOfPlants = plant.NumberOfPlants,
            Notes = plant.Notes
        };

    }

    public void Update(PlanHarvestCycleBase plan, Action<HarvestEventTriggerEnum, TriggerEntity> addHarvestEvent)
    {
        this.Set<string>(() => this.PlantId, plan.PlantId);
        this.Set<string?>(() => this.PlantGrowthInstructionId, plan.PlantGrowthInstructionId);
        this.Set<string?>(() => this.GardenBedId, plan.GardenBedId);
        this.Set<int?>(() => this.NumberOfPlants, plan.NumberOfPlants);
        this.Set<string>(() => this.Notes, plan.Notes);

        if (this.DomainEvents != null && this.DomainEvents.Count > 0)
        {
            this.DomainEvents.Clear();
            addHarvestEvent(HarvestEventTriggerEnum.PlantHarvestCycleUpdated, new TriggerEntity(EntityTypeEnum.PlantHarvestCycle, this.Id));
        }
    }

    protected override void AddDomainEvent(string attributeName)
    {
        this.DomainEvents.Add(
            new HarvestChildEvent(HarvestEventTriggerEnum.PlanHarvestCycleUpdated, new TriggerEntity(EntityTypeEnum.PlanHarvestCycle, this.Id)));
    }
}
