namespace PlantHarvest.Domain.HarvestAggregate.Events.Meta;

public enum HarvestEventTriggerEnum
{
    HarvestCycleCreated = 1,
    HarvestCycleUpdated = 2,
    PlantAddedToHarvestCycle = 3,
    PlantHarvestCycleUpdated = 4,
    PlantHarvestCycleDeleted = 5

}
