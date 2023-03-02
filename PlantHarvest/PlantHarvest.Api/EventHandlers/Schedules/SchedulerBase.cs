using MongoDB.Driver.Linq;

namespace PlantHarvest.Api.Schedules;

public abstract class SchedulerBase
{
    public DateTime? GetStartDateBasedOnWeatherCondition(plant.WeatherConditionEnum weatherCondition, int weeksAhead, GardenViewModel garden)
    {
        DateTime? startDate = null;
        int year = DateTime.Now.Year;

        switch (weatherCondition)
        {
            case plant.WeatherConditionEnum.BeforeLastFrost:
                DateTime lastFrost = new DateTime(year, garden.LastFrostDate.Month, garden.LastFrostDate.Day);
                startDate = lastFrost.AddDays(-7 * weeksAhead);
                break;

            case plant.WeatherConditionEnum.BeforeFirstFrost:
                DateTime firstFrost = new DateTime(year, garden.FirstFrostDate.Month, garden.FirstFrostDate.Day);
                startDate = firstFrost.AddDays(-7 * weeksAhead);
                break;

            case plant.WeatherConditionEnum.EarlySpring:
                lastFrost = new DateTime(year, garden.LastFrostDate.Month, garden.LastFrostDate.Day);
                startDate = lastFrost.AddDays(-7 * 4);
                break;
            case plant.WeatherConditionEnum.AfterDangerOfFrost:
                lastFrost = new DateTime(year, garden.LastFrostDate.Month, garden.LastFrostDate.Day);
                startDate = lastFrost.AddDays(7 * weeksAhead);

                break;
            case plant.WeatherConditionEnum.MidSummer:
                lastFrost = new DateTime(year, garden.LastFrostDate.Month, garden.LastFrostDate.Day);
                firstFrost = new DateTime(year, garden.FirstFrostDate.Month, garden.FirstFrostDate.Day);

                double growDays = (firstFrost - lastFrost).TotalDays;
                startDate = lastFrost.AddDays(growDays / 2);
                break;
            case plant.WeatherConditionEnum.WarmSoil:
                DateTime warmSoil = new DateTime(year, garden.WarmSoilDate.Month, garden.WarmSoilDate.Day);
                startDate = warmSoil.AddDays(-7 * weeksAhead);
                break;
        }

        return startDate;
    }
}
