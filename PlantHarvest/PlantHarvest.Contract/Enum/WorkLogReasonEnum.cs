using System.ComponentModel;

namespace PlantHarvest.Contract.Enum;

public enum WorkLogReasonEnum : int
{
    [Description("Unspecified")]
    Unspecified = 0,
    [Description("Fertilize")]
    Fertilize = 1,
    [Description("Harden Off")]
    Harden = 2,
    [Description("Harvest")]
    Harvest = 3,
    [Description("Information")]
    Information =4,
    [Description("Issue")]
    Issue = 5,
    [Description("Issue Resolution")]
    IssueResolution = 6,
    [Description("Plant")]
    Plant = 7,
    [Description("Maintenance")]
    Maintenance = 8,
    [Description("So Indoors")]
    SowIndoors = 9,
    [Description("Sow Outside")]
    SowOutside = 9,
    [Description("Transplant Outside")]
    TransplantOutside = 10
}
