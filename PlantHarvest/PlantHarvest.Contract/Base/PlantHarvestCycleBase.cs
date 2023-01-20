namespace PlantHarvest.Contract.Base;

public abstract record PlantHarvestCycleBase
{

    public string HarvestCycleId { get; set; } = string.Empty;

    public string PlantId { get; set; }=string.Empty;
    public string? PlantVarietyId { get; set; }
    public string? PlantGrowthInstructionId { get; set; }
    public string? GardenBedId { get; set; }

    public bool IsDirectSeed { get; set; }
    public int? NumberOfSeeds { get; set; }
    public string? SeedCompanyId { get; set; }
    public string? SeedCompanyName { get; set; }
    public DateTime? SeedingDateTime { get; set; }

    public DateTime? GerminationDateTime { get; set; }
    public decimal? GerminationRate { get; set; }
    
    public int? NumberOfTransplants { get; set; }
    public DateTime? TransplantDateTime { get; set; }
    
    public DateTime? FirstHarvestDate { get; set; }
    public DateTime? LastHarvestDate { get; set; }
    
    public decimal? TotalWeightInPounds { get; set; }
    
    public int? TotalItems { get; set; }
    
    public string Notes { get; set; }= string.Empty;
}

