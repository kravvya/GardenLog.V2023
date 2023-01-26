using System.ComponentModel;

namespace PlantHarvest.Contract.Enum;

public enum WorkLogReasonEnum : int
{

    [Description("Unspecified")]
    Unspecified = 0,
    [Description("Information")]
    Information = 1,
    [Description("Harvesting")]
    Harvesting = 2,
    [Description("Maintenance")]
    Maintenance = 3,
    [Description("Issue")]
    Issue = 4,
    [Description("Issue Resolution")]
    IssueResolution = 5,
    [Description("Fertilizing")]
    Fertilize = 6,
    [Description("Sowing")]
    Fertilizing = 7,
    [Description("Transplanting")]
    Transplanting = 8,
    [Description("Hardening")]
    Hardening = 9,
    [Description("Planting")]
    Planting = 10


}
