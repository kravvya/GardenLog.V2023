namespace UserManagement.Api.Model;

public record GardenEvent : BaseDomainEvent
{
    public Garden PlantLocation { get; init; }
    public UserProfileEventTriggerEnum Trigger { get; init; }
    public UserManagment.Api.Model.Meta.TriggerEntity TriggerEntity { get; init; }
    public string UserProfileId { get { return PlantLocation.UserProfileId; } init { } }

    private GardenEvent() { }

    public GardenEvent(Garden plantLocation, UserProfileEventTriggerEnum trigger, UserManagment.Api.Model.Meta.TriggerEntity entity)
    {
        PlantLocation = plantLocation;
        Trigger = trigger;
        TriggerEntity = entity;
    }

}
