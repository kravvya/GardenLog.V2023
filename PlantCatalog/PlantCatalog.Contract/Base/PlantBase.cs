﻿namespace PlantCatalog.Contract.Base;


public abstract record PlantBase
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Color { get; set; }
    public PlantLifecycleEnum Lifecycle { get; set; }
    public PlantTypeEnum Type { get; set; }
    public MoistureRequirementEnum MoistureRequirement { get; set; }
    public LightRequirementEnum LightRequirement { get; set; }
    public GrowToleranceEnum GrowTolerance { get; set; }
    public string GardenTip { get; set; }
    public int? SeedViableForYears { get; set; }
    public List<string> Tags { get; set; } = new();
    public List<string> VarietyColors { get; set; } = new();
}
