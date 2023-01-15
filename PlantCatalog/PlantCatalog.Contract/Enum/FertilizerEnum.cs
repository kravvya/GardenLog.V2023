using System.ComponentModel;

namespace PlantCatalog.Contract.Enum
{
    public enum FertilizerEnum : int
    {
        [Description("Unspecified")]
        Unspecified = 0,

        [Description("All Purpose")]
        AllPurpose = 1,

        [Description("Nitrogen")]
        Nitrogen = 2,

        [Description("Half Strength Balanced")]
        HalfBalanced = 3,

        [Description("Balanced")]
        Balanced = 4
    }
}
