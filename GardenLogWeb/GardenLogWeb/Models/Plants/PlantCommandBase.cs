using System.Runtime.Serialization;

namespace GardenLogWeb.Models.Plants;

public abstract record PlantCommandBase
{
    [DataMember]
    public string PlantName { get; set; }
    [DataMember]
    public string Description { get; set; }
    [DataMember]
    public string Color { get; set; }
    [DataMember]
    public string GardenTip { get; set; }
    [DataMember]
    public string Lifecycle { get; set; }
    [DataMember]
    public string Type { get; set; }
    [DataMember]
    public string LightRequirement { get; set; }
    [DataMember]
    public string MoistureRequirement { get; set; }
    [DataMember]
    public int? SeedViableForYears { get; set; }
}
