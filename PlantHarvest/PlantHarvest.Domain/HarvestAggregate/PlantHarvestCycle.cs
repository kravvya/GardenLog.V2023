using PlantHarvest.Contract.Commands;

namespace PlantHarvest.Domain.HarvestAggregate;

public class PlantHarvestCycle : BaseEntity, IEntity
{
    public string UserProfileId { get; private set; }
    public string PlantId { get; private set; }
    public string? PlantVarietyId { get; private set; }
    public string? PlantGrowthInstructionId { get; private set; }
    public string? GardenBedId { get; private set; }

    public bool IsDirectSeed { get; private set; }
    public int? NumberOfSeeds { get; private set; }
    public string? SeedCompanyId { get; private set; }
    public string? SeedCompanyName { get; private set; }

    public DateTime? SeedingDateTime { get; private set; }
    
    public DateTime? GerminationDateTime { get; private set; }
    public decimal? GerminationRate { get; private set; }

    public int? NumberOfTransplants { get; private set; }
    public DateTime? TransplantDateTime { get; private set; }

    public DateTime? FirstHarvestDate { get; private set; }
    public DateTime? LastHarvestDate { get; private set; }

    public decimal? TotalWeightInPounds { get; private set; }
    public int? TotalItems { get; private set; }

    public string Notes { get; private set; }


    private PlantHarvestCycle()
    {
    }


    public static PlantHarvestCycle Create(PlantHarvestCycleBase plant, string userProfileId)
    {
        return new PlantHarvestCycle()
        {
            Id = Guid.NewGuid().ToString(),
            UserProfileId = userProfileId,
            PlantId = plant.PlantId,
            PlantVarietyId = plant.PlantVarietyId,
            PlantGrowthInstructionId= plant.PlantGrowthInstructionId,
            GardenBedId = plant.GardenBedId,
            IsDirectSeed = plant.IsDirectSeed,
            NumberOfSeeds = plant.NumberOfSeeds,
            SeedCompanyId= plant.SeedCompanyId,
            SeedCompanyName= plant.SeedCompanyName,
            SeedingDateTime = plant.SeedingDateTime,
            GerminationDateTime = plant.GerminationDateTime,
            GerminationRate = plant.GerminationRate,
            NumberOfTransplants = plant.NumberOfTransplants,
            TransplantDateTime = plant.TransplantDateTime,
            FirstHarvestDate = plant.FirstHarvestDate,
            LastHarvestDate = plant.LastHarvestDate,
            TotalWeightInPounds = plant.TotalWeightInPounds,
            TotalItems = plant.TotalItems,
            Notes = plant.Notes
        };

    }


    public void Update(UpdatePlantHarvestCycleCommand command, Action<HarvestEventTriggerEnum, TriggerEntity> addHarvestEvent)
    {
        this.Set<string?>(() => this.GardenBedId, command.GardenBedId);
        this.Set<bool>(() => this.IsDirectSeed, command.IsDirectSeed);
        this.Set<int?>(() => this.NumberOfSeeds, command.NumberOfSeeds);
        this.Set<string?>(() => this.SeedCompanyId, command.SeedCompanyId);
        this.Set<string?>(() => this.SeedCompanyName, command.SeedCompanyName);
        this.Set<DateTime?>(() => this.SeedingDateTime, command.SeedingDateTime);
        this.Set<DateTime?>(() => this.GerminationDateTime, command.GerminationDateTime);
        this.Set<decimal?>(() => this.GerminationRate, command.GerminationRate);
        this.Set<int?>(() => this.NumberOfTransplants, command.NumberOfTransplants);
        this.Set<DateTime?>(() => this.TransplantDateTime, command.TransplantDateTime);
        this.Set<DateTime?>(() => this.FirstHarvestDate, command.FirstHarvestDate);
        this.Set<DateTime?>(() => this.LastHarvestDate, command.LastHarvestDate);
        this.Set<decimal?>(() => this.TotalWeightInPounds, command.TotalWeightInPounds);
        this.Set<int?>(() => this.TotalItems, command.TotalItems);
        this.Set<string>(() => this.Notes, command.Notes);

        if (this.DomainEvents != null && this.DomainEvents.Count > 0)
        {
            this.DomainEvents.Clear();
            addHarvestEvent(HarvestEventTriggerEnum.PlantHarvestCycleUpdated, new TriggerEntity(EntityTypeEnum.PlantHarvestCycle, this.Id));
        }
    }

    protected override void AddDomainEvent(string attributeName)
    {
        this.DomainEvents.Add(
            new HarvestChildEvent(HarvestEventTriggerEnum.PlantHarvestCycleUpdated, new TriggerEntity(EntityTypeEnum.PlantHarvestCycle, this.Id)));
    }
}
