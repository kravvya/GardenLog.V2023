using System.Runtime.Serialization;

namespace GardenLogWeb.Models.PlantGrowInstructions;

public abstract record PlantGrowInstructionCommandBase
{
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public string PlantId { get; set; }
    [DataMember]
    public string PlantingDepthInInches { get; set; }
    [DataMember]
    public int? SpacingInInches { get; set; }
    [DataMember]
    public string PlantingMethod { get; set; }
    [DataMember]
    public string StartSeedAheadOfWeatherCondition { get; set; }
    [DataMember]
    public int? StartSeedWeeksAheadOfWeatherCondition { get; set; }
    [DataMember]
    public string HarvestSeason { get; set; }
    [DataMember]
    public int? TransplantWeeksAheadOfWeatherCondition { get; set; }
    [DataMember]
    public string TransplantAheadOfWeatherCondition { get; set; }
    [DataMember]
    public string StartSeedInstructions { get; set; }
    [DataMember]
    public string GrowingInstructions { get; set; }
    [DataMember]
    public int? StartSeedWeeksRange { get; set; }
    [DataMember]
    public int? TransplantWeeksRange { get; set; }
    [DataMember]
    public string HarvestInstructions { get; set; }
}
