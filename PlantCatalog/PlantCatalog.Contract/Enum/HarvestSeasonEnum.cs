using System.ComponentModel;

namespace PlantCatalog.Contract.Enum
{
    [Flags]
    public enum HarvestSeasonEnum :int
    {
        [Description("Unspecified")]
        Unspecified = 0,
        [Description("Spring")]
        Spring = 1,
        [Description("EarlySummer")]
        EarlySummer = 2,
        [Description("Summer")]
        Summer = 3,
        [Description("Fall")]
        Fall = 4,
        [Description("Late Fall")]
        LateFall = 5
    }
}
