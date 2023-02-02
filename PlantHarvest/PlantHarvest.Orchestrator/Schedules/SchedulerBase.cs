using MongoDB.Driver.Linq;

namespace PlantHarvest.Orchestrator.Schedules;

public abstract class SchedulerBase
{
    public DateTime? GetStartDateBasedOnWeatherCondition(WeatherConditionEnum weatherCondition, int weeksAhead, GardenViewModel garden)
    {
        DateTime? startDate = null;
        int year = DateTime.Now.Year;

        switch (weatherCondition)
        {
            case WeatherConditionEnum.BeforeLastFrost:
                DateTime lastFrost = new DateTime(year, garden.LastFrostDate.Month, garden.LastFrostDate.Day);
                startDate = lastFrost.AddDays(-7 * weeksAhead);
                break;

            case WeatherConditionEnum.BeforeFirstFrost:
                DateTime firstFrost = new DateTime(year, garden.FirstFrostDate.Month, garden.FirstFrostDate.Day);
                startDate = firstFrost.AddDays(-7 * weeksAhead);
                break;

            case WeatherConditionEnum.EarlySpring:
                lastFrost = new DateTime(year, garden.LastFrostDate.Month, garden.LastFrostDate.Day);
                startDate = lastFrost.AddDays(-7 * 4);
                break;
            case WeatherConditionEnum.AfterDangerOfFrost:
                lastFrost = new DateTime(year, garden.LastFrostDate.Month, garden.LastFrostDate.Day);
                startDate = lastFrost.AddDays(7 * weeksAhead);

                break;
            case WeatherConditionEnum.MidSummer:
                lastFrost = new DateTime(year, garden.LastFrostDate.Month, garden.LastFrostDate.Day);
                firstFrost = new DateTime(year, garden.FirstFrostDate.Month, garden.FirstFrostDate.Day);

                double growDays = (firstFrost - lastFrost).TotalDays;
                startDate = lastFrost.AddDays(growDays / 2);
                break;
        }

        return startDate;
    }
}
