using System.ComponentModel;

namespace PlantHarvest.Contract.Enum;

public enum WorkLogReasonEnum : int
{
    [Description("Unspecified")]
    Unspecified = 0,
    [Description("Fertilizing")]
    Fertilize = 1,
    [Description("Hardening")]
    Hardening = 2,
    [Description("Harvesting")]
    Harvesting = 3,
    [Description("Information")]
    Information =4,
    [Description("Issue")]
    Issue = 5,
    [Description("Issue Resolution")]
    IssueResolution = 6,
    [Description("Planting")]
    Planting = 7,
    [Description("Maintenance")]
    Maintenance = 8,
    [Description("Sowing")]
    Sowing = 9,
    [Description("Transplanting")]
    Transplanting = 10
}
