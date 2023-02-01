using PlantHarvest.Contract.Enum;

namespace PlantHarvest.Contract.Base;

public abstract record PlantHarvestCycleBase
{

    public string HarvestCycleId { get; set; } = string.Empty;

    public string PlantId { get; set; } = string.Empty;
    public string PlantName { get; set; } = string.Empty;

    public string? PlantVarietyId { get; set; }
    public string? PlantVarietyName { get; set; }

    public string? PlantGrowthInstructionId { get; set; }
    public string? PlantGrowthInstructionName { get; set; }
    public PlantingMethodEnum PlantingMethod { get; set; }

    public string? GardenBedId { get; set; }
    public string? GardenBedName { get; set; }

    public int? NumberOfSeeds { get; set; }

    public string? SeedVendorId { get; set; }
    public string? SeedVendorName { get; set; }

    public DateTime? SeedingDateTime { get; set; }

    public DateTime? GerminationDate { get; set; }
    public decimal? GerminationRate { get; set; }

    public int? NumberOfTransplants { get; set; }
    public DateTime? TransplantDate { get; set; }

    public DateTime? FirstHarvestDate { get; set; }
    public DateTime? LastHarvestDate { get; set; }

    public decimal? TotalWeightInPounds { get; set; }

    public int? TotalItems { get; set; }

    public string Notes { get; set; } = string.Empty;
    public int? DesiredNumberOfPlants { get; set; }

}

