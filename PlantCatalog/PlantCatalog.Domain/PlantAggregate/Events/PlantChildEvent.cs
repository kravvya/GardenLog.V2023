using GardenLog.SharedKernel;
using PlantCatalog.Domain.PlantAggregate.Events.Meta;

namespace PlantCatalog.Domain.PlantAggregate.Events
{
    public record PlantChildEvent : BaseDomainEvent
    {
        public PlantEventTriggerEnum Trigger { get; init; }
        public TriggerEntity TriggerEntity { get; init; }

        public PlantChildEvent(PlantEventTriggerEnum trigger, TriggerEntity entity)
        {
            Trigger = trigger;
            TriggerEntity = entity;
        }
    }
}
