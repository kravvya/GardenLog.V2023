using System.ComponentModel;
using System.Reflection;
using GardenLogWeb.Shared.Services;

namespace GardenLogWeb.Services;

public interface IVerifyService
{
    IReadOnlyCollection<KeyValuePair<string, string>> GetHarvestSeasonCodeList();
    IReadOnlyCollection<KeyValuePair<string, string>> GetHarvestSeasonCodeList(bool excludeDefault);
    IReadOnlyCollection<KeyValuePair<string, string>> GetLightRequirementCodeList();
    IReadOnlyCollection<KeyValuePair<string, string>> GetLightRequirementCodeList(bool excludeDefault);
    IReadOnlyCollection<KeyValuePair<string, string>> GetMoistureRequirementCodeList();
    IReadOnlyCollection<KeyValuePair<string, string>> GetMoistureRequirementCodeList(bool excludeDefault);
    IReadOnlyCollection<KeyValuePair<string, string>> GetPlantingDepthCodeList();
    IReadOnlyCollection<KeyValuePair<string, string>> GetPlantingDepthCodeList(bool excludeDefault);
    IReadOnlyCollection<KeyValuePair<string, string>> GetPlantingMethodCodeList();
    IReadOnlyCollection<KeyValuePair<string, string>> GetPlantingMethodCodeList(bool excludeDefault);
    IReadOnlyCollection<KeyValuePair<string, string>> GetPlantLifecycleCodeList();
    IReadOnlyCollection<KeyValuePair<string, string>> GetPlantLifecycleCodeList(bool excludeDefault);
    IReadOnlyCollection<KeyValuePair<string, string>> GetPlantTypeCodeList();
    IReadOnlyCollection<KeyValuePair<string, string>> GetPlantTypeCodeList(bool excludeDefault);
    IReadOnlyCollection<KeyValuePair<string, string>> GetWeatherConditionCodeList(bool excludeDefault);
    IReadOnlyCollection<KeyValuePair<string, string>> GetWeatherConditionCodeList();
    string GetHarvestDescription(string key);
    string GetWeatherConditionDescription(string key);
    string GetPlantingMethodDescription(string key);
    string GetPlantingDepthDescription(string key);
    string GetMoistureRequirementDescription(string key);
    string GetLightRequirementDescription(string key);
}

public class VerifyService : IVerifyService
{
    private const string KEY_TEMPLATE = "Verify_{0}";
    private readonly ICacheService _cacheService;

    public VerifyService(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public IReadOnlyCollection<KeyValuePair<string, string>> GetPlantLifecycleCodeList(bool excludeDefault)
    {
        return GetEnumList(typeof(PlantLifecycleEnum), excludeDefault);
    }

    public IReadOnlyCollection<KeyValuePair<string, string>> GetPlantLifecycleCodeList()
    {
        return GetEnumList(typeof(PlantLifecycleEnum));
    }

    public IReadOnlyCollection<KeyValuePair<string, string>> GetPlantTypeCodeList(bool excludeDefault)
    {
        return GetEnumList(typeof(PlantTypeEnum), excludeDefault);
    }

    public IReadOnlyCollection<KeyValuePair<string, string>> GetPlantTypeCodeList()
    {
        return GetEnumList(typeof(PlantTypeEnum));
    }

    public IReadOnlyCollection<KeyValuePair<string, string>> GetLightRequirementCodeList(bool excludeDefault)
    {
        return GetEnumList(typeof(LightRequirementEnum), excludeDefault);
    }

    public IReadOnlyCollection<KeyValuePair<string, string>> GetLightRequirementCodeList()
    {
        return GetEnumList(typeof(LightRequirementEnum));
    }

    public IReadOnlyCollection<KeyValuePair<string, string>> GetMoistureRequirementCodeList(bool excludeDefault)
    {
        return GetEnumList(typeof(MoistureRequirementEnum), excludeDefault);
    }

    public IReadOnlyCollection<KeyValuePair<string, string>> GetMoistureRequirementCodeList()
    {
        return GetEnumList(typeof(MoistureRequirementEnum));
    }

    public IReadOnlyCollection<KeyValuePair<string, string>> GetPlantingDepthCodeList(bool excludeDefault)
    {
        return GetEnumList(typeof(PlantingDepthEnum), excludeDefault);
    }

    public IReadOnlyCollection<KeyValuePair<string, string>> GetPlantingDepthCodeList()
    {
        return GetEnumList(typeof(PlantingDepthEnum));
    }

    public IReadOnlyCollection<KeyValuePair<string, string>> GetPlantingMethodCodeList(bool excludeDefault)
    {
        return GetEnumList(typeof(PlantingMethodEnum), excludeDefault);
    }

    public IReadOnlyCollection<KeyValuePair<string, string>> GetPlantingMethodCodeList()
    {
        return GetEnumList(typeof(PlantingMethodEnum));
    }

    public IReadOnlyCollection<KeyValuePair<string, string>> GetWeatherConditionCodeList(bool excludeDefault)
    {
        return GetEnumList(typeof(WeatherConditionEnum), excludeDefault);
    }

    public IReadOnlyCollection<KeyValuePair<string, string>> GetWeatherConditionCodeList()
    {
        return GetEnumList(typeof(WeatherConditionEnum));
    }

    public IReadOnlyCollection<KeyValuePair<string, string>> GetHarvestSeasonCodeList(bool excludeDefault)
    {
        return GetEnumList(typeof(HarvestSeasonEnum), excludeDefault);
    }

    public IReadOnlyCollection<KeyValuePair<string, string>> GetHarvestSeasonCodeList()
    {
        return GetEnumList(typeof(HarvestSeasonEnum));
    }
    public string GetLightRequirementDescription(string key)
    {
        return this.GetLightRequirementCodeList().FirstOrDefault(l => l.Key.Equals(key))!.Value;
    }
    public string GetMoistureRequirementDescription(string key)
    {
        return this.GetMoistureRequirementCodeList().FirstOrDefault(l => l.Key.Equals(key))!.Value;
    }
    public string GetPlantingDepthDescription(string key)
    {
        return this.GetPlantingDepthCodeList().FirstOrDefault(l => l.Key.Equals(key))!.Value;
    }
    public string GetPlantingMethodDescription(string key)
    {
        return this.GetPlantingMethodCodeList().FirstOrDefault(l => l.Key.Equals(key))!.Value;
    }
    public string GetWeatherConditionDescription(string key)
    {
        return this.GetWeatherConditionCodeList().FirstOrDefault(l => l.Key.Equals(key))!.Value;
    }
    public string GetHarvestDescription(string key)
    {
        return this.GetHarvestSeasonCodeList().FirstOrDefault(l => l.Key.Equals(key))!.Value;
    }

    private IReadOnlyCollection<KeyValuePair<string, string>> GetEnumList(Type genericEnumType)
    {
        return GetEnumList(genericEnumType, false);
    }



    private IReadOnlyCollection<KeyValuePair<string, string>> GetEnumList(Type genericEnumType, bool excludeDefault)
    {
        string key = string.Format(KEY_TEMPLATE, genericEnumType.Name);

        if (!_cacheService.TryGetValue<List<KeyValuePair<string, string>>>(key, out List<KeyValuePair<string, string>> value))
        {
            value = new List<KeyValuePair<string, string>>();

            foreach (var item in Enum.GetValues(genericEnumType))
            {
                var verify = new KeyValuePair<string, string>(Enum.GetName(genericEnumType, item), GetDescription(((Enum)item)));
                value.Add(verify);
            }

            _cacheService.Set<IReadOnlyCollection<KeyValuePair<string, string>>>(key, value);
        }

        if (!excludeDefault) return value;

        return value.Where(v => !v.Key.Equals("Unspecified")).ToList();
    }


    private static string GetDescription(Enum GenericEnum)
    {
        Type genericEnumType = GenericEnum.GetType();
        MemberInfo[] memberInfo = genericEnumType.GetMember(GenericEnum.ToString());
        if ((memberInfo != null && memberInfo.Length > 0))
        {
            var _Attribs = memberInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            if ((_Attribs != null && _Attribs.Count() > 0))
            {
                return ((System.ComponentModel.DescriptionAttribute)_Attribs.ElementAt(0)).Description;
            }
        }
        return GenericEnum.ToString();
    }
}

public enum HarvestSeasonEnum : int
{
    [Description("Unspecified")]
    Unspecified = 0,
    [Description("Spring")]
    Spring = 1,
    [Description("Early Summer")]
    EarlySummer = 2,
    [Description("Summer")]
    Summer = 3,
    [Description("Fall")]
    Fall = 4,
    [Description("Late Fall")]
    LateFall = 5
}

public enum LightRequirementEnum : int
{
    [Description("Unspecified")]
    Unspecified = 0,
    [Description("Full Shade")]
    FullShade = 1,
    [Description("Part Shade")]
    PartShade = 2,
    [Description("Full Sun")]
    FullSun = 3
}
public enum MoistureRequirementEnum : int
{
    [Description("Unspecified")]
    Unspecified = 0,
    [Description("Drout Tolerant")]
    DroutTolerant = 1,
    [Description("At least 1in per week")]
    InchPerWeek = 2,
    [Description("1 to 2 in per week")]
    TwoInchPerWeek = 3,
    [Description("Consistent Moisture")]
    ConsistentMoisture = 4

}
public enum PlantingDepthEnum : int
{
    [Description("Unspecified")]
    Unspecified = 0,
    [Description("On Surface")]
    Surface = 1,
    [Description("1/16 in")]
    Depth16th = 2,
    [Description("1/8 in")]
    Depth8th = 3,
    [Description("1/4 in")]
    Depth4th = 4,
    [Description("1/2 in")]
    Depth2 = 5,
    [Description("1 in")]
    Depth1 = 6
}
public enum PlantingMethodEnum : int
{
    [Description("Unspecified")]
    Unspecified = 0,
    [Description("Direct Seed")]
    DirectSeed = 1,
    [Description("Start Indoors")]
    SeedIndoors = 2

}
public enum PlantLifecycleEnum : int
{
    [Description("Unspecified")]
    Unspecified = 0,
    [Description("Annual")]
    Annual = 1,
    [Description("Biennial")]
    Biennial = 2,
    [Description("Perennial")]
    Perennial = 3
}
public enum PlantTypeEnum : int
{
    [Description("Unspecified")]
    Unspecified = 0,
    [Description("Vegetable")]
    Vegetable = 1,
    [Description("Berry")]
    Berry = 2,
    [Description("Flower")]
    Flower = 3,
    [Description("Herb")]
    Herb = 4
}
public enum WeatherConditionEnum : int
{
    [Description("Unspecified")]
    Unspecified = 0,
    [Description("Before last frost")]
    BeforeLastFrost = 1,
    [Description("Before first frost")]
    BeforeFirstFrost = 2,
    [Description("In Early Spring")]
    EarlySpring = 3,
    [Description("In Warm Soil")]
    WarmSoil = 4,
    [Description("After danger of frost")]
    AfterDangerOfFrost = 5,
    MidSummer = 6
}
