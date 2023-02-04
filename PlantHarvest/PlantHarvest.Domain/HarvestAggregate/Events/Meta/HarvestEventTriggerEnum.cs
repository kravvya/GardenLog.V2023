namespace PlantHarvest.Domain.HarvestAggregate.Events.Meta;

public enum HarvestEventTriggerEnum
{
    HarvestCycleCreated = 1,
    HarvestCycleUpdated = 2,
    HarvestCycleDeleted = 3,
    PlantAddedToHarvestCycle = 4,
    PlantHarvestCycleUpdated = 5,
    PlantHarvestCycleDeleted = 6,
    PlantScheduleCreated = 7,
    PlantScheduleUpdated = 8,
    PlantScheduleDeleted = 9,
    PlantHarvestCycleSeeded = 10
}
