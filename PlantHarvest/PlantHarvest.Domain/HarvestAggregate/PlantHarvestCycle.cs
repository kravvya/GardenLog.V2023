using PlantHarvest.Contract.Commands;
using PlantHarvest.Contract.Enum;

namespace PlantHarvest.Domain.HarvestAggregate;

public class PlantHarvestCycle : BaseEntity, IEntity
{
    public string UserProfileId { get; private set; }
    public string PlantId { get; private set; }
    public string PlantName { get; private set; }

    public string? PlantVarietyId { get; private set; }
    public string? PlantVarietyName { get; private set; }

    public string? PlantGrowthInstructionId { get; private set; }
    public string? PlantGrowthInstructionName { get; private set; }
    public PlantingMethodEnum PlantingMethod { get; private set; }

    public string? GardenBedId { get; private set; }
    public string? GardenBedName { get; private set; }

    public int? NumberOfSeeds { get; private set; }
    public string? SeedCompanyId { get; private set; }
    public string? SeedCompanyName { get; private set; }

    public DateTime? SeedingDate { get; private set; }

    public DateTime? GerminationDate { get; private set; }
    public decimal? GerminationRate { get; private set; }

    public int? NumberOfTransplants { get; private set; }
    public DateTime? TransplantDate { get; private set; }

    public DateTime? FirstHarvestDate { get; private set; }
    public DateTime? LastHarvestDate { get; private set; }

    public decimal? TotalWeightInPounds { get; private set; }
    public int? TotalItems { get; private set; }

    public string Notes { get; private set; }
    public int? DesiredNumberOfPlants { get; private set; }


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
            PlantName= plant.PlantName,
            PlantVarietyId = plant.PlantVarietyId,
            PlantVarietyName= plant.PlantVarietyName,
            PlantGrowthInstructionId = plant.PlantGrowthInstructionId,
            PlantGrowthInstructionName= plant.PlantGrowthInstructionName,
            GardenBedId = plant.GardenBedId,
            GardenBedName = plant.GardenBedName,
            NumberOfSeeds = plant.NumberOfSeeds,
            SeedCompanyId = plant.SeedVendorId,
            SeedCompanyName = plant.SeedVendorName,
            SeedingDate = plant.SeedingDateTime,
            GerminationDate = plant.GerminationDate,
            GerminationRate = plant.GerminationRate,
            NumberOfTransplants = plant.NumberOfTransplants,
            TransplantDate = plant.TransplantDate,
            FirstHarvestDate = plant.FirstHarvestDate,
            LastHarvestDate = plant.LastHarvestDate,
            TotalWeightInPounds = plant.TotalWeightInPounds,
            TotalItems = plant.TotalItems,
            Notes = plant.Notes,
            DesiredNumberOfPlants = plant.DesiredNumberOfPlants,
            PlantingMethod= plant.PlantingMethod,
        };

    }


    public void Update(UpdatePlantHarvestCycleCommand command, Action<HarvestEventTriggerEnum, TriggerEntity> addHarvestEvent)
    {
        this.Set<string?>(() => this.PlantVarietyId, command.PlantVarietyId);
        this.Set<string?>(() => this.PlantVarietyName, command.PlantVarietyName);
        this.Set<string?>(() => this.GardenBedId, command.GardenBedId);
        this.Set<string?>(() => this.GardenBedName, command.GardenBedName);
        this.Set<int?>(() => this.NumberOfSeeds, command.NumberOfSeeds);
        this.Set<string?>(() => this.SeedCompanyId, command.SeedVendorId);
        this.Set<string?>(() => this.SeedCompanyName, command.SeedVendorName);
        this.Set<DateTime?>(() => this.SeedingDate, command.SeedingDateTime);
        this.Set<DateTime?>(() => this.GerminationDate, command.GerminationDate);
        this.Set<decimal?>(() => this.GerminationRate, command.GerminationRate);
        this.Set<int?>(() => this.NumberOfTransplants, command.NumberOfTransplants);
        this.Set<DateTime?>(() => this.TransplantDate, command.TransplantDate);
        this.Set<DateTime?>(() => this.FirstHarvestDate, command.FirstHarvestDate);
        this.Set<DateTime?>(() => this.LastHarvestDate, command.LastHarvestDate);
        this.Set<decimal?>(() => this.TotalWeightInPounds, command.TotalWeightInPounds);
        this.Set<int?>(() => this.TotalItems, command.TotalItems);
        this.Set<string>(() => this.Notes, command.Notes);
        this.Set<int?>(() => this.DesiredNumberOfPlants, command.DesiredNumberOfPlants);
        this.Set<PlantingMethodEnum>(()=>this.PlantingMethod, command.PlantingMethod);

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
