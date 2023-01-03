using System.Runtime.Serialization;

namespace GardenLogWeb.Models.PlantVariety;

public abstract record PlantVarietyCommandBase
{
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public string Description { get; set; }
    [DataMember]
    public string PlantId { get; set; }
    [DataMember]
    public int? DaysToMaturity { get; set; }
    [DataMember]
    public int? HeightInInches { get; set; }
    [DataMember]
    public bool IsHeirloom { get; set; }
    [DataMember]
    public string MoistureRequirement { get; set; }
    [DataMember]
    public string LightRequirement { get; set; }
    [DataMember]
    public string Title { get; set; }

}