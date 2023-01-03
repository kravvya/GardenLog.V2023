using System.ComponentModel;

namespace PlantCatalog.Contract.Enum;

public enum PlantLifecycleEnum: int
{
    [Description("Unspecified")]
    Unspecified = 0,
    [Description("Cool")]
    Cool = 1,
    [Description("Warm")]
    Warm = 2,
    [Description("Biennial")]
    Biennial = 3,
    [Description("Perennial")]
    Perennial = 4
}
