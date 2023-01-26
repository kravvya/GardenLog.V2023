using System.ComponentModel;

namespace PlantHarvest.Contract.Enum;

public enum WorkLogEntityEnum : int
{
    [Description("Unspecified")]
    Unspecified = 0,
    [Description("Plant")]
    Plant = 1,
    [Description("Plant Location")]
    PlantLocation = 2,
    [Description("Harvest cycle")]
    HarvestCycle = 3,
    [Description("Plant Harvest cycle")]
    PlantHarvestCycle = 4
}
