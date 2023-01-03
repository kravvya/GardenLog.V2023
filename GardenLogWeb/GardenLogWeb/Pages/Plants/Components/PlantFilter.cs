namespace GardenLogWeb.Pages.Plants.Components;

public class PlantFilter
{

    public List<CheckableEnum> PlantLifecycleEnums { get; set; } = new();
    public List<CheckableEnum> PlantTypeEnums { get; set; } = new();
    public List<CheckableEnum> LightRequirementEnums { get; set; } = new();
    public List<CheckableEnum> MoistureRequirementEnums { get; set; } = new();

    public PlantFilter(IVerifyService verifyService)
    {
        var plantLifeCyclesCodes = verifyService.GetPlantLifecycleCodeList(true);
        var plantTypeCodes = verifyService.GetPlantTypeCodeList(true);
        var lightRequirementCodes = verifyService.GetLightRequirementCodeList(true);
        var moistureRequirementCodes = verifyService.GetMoistureRequirementCodeList(true);

        foreach (var code in plantLifeCyclesCodes)
        {
            PlantLifecycleEnums.Add(new CheckableEnum(code));
        }

        foreach (var code in plantTypeCodes)
        {
            PlantTypeEnums.Add(new CheckableEnum(code));
        }

        foreach (var code in lightRequirementCodes)
        {
            LightRequirementEnums.Add(new CheckableEnum(code));
        }

        foreach (var code in moistureRequirementCodes)
        {
            MoistureRequirementEnums.Add(new CheckableEnum(code));
        }
    }

}

public record CheckableEnum(KeyValuePair<string, string> EnumItem)
{
    public bool IsSelected;
}
