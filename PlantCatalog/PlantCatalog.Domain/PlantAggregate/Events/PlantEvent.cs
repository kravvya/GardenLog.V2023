using GardenLog.SharedKernel;
using PlantCatalog.Domain.PlantAggregate.Events.Meta;

namespace PlantCatalog.Domain.PlantAggregate.Events
{
    public record PlantEvent : BaseDomainEvent
    {
        public Plant Plant { get; init; }
        public PlantEventTriggerEnum Trigger { get; init; }
        public TriggerEntity TriggerEntity { get; init; }
        public string PlantId { get { return Plant.Id; } init { } }
        private PlantEvent() { }

        public PlantEvent(Plant plant, PlantEventTriggerEnum trigger, TriggerEntity entity)
        {
            Plant = plant;
            Trigger = trigger;
            TriggerEntity = entity;
        }

       
    }




}
