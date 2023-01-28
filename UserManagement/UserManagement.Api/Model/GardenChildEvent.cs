namespace UserManagement.Api.Model;

public record GardenChildEvent : BaseDomainEvent
{
    public UserProfileEventTriggerEnum Trigger { get; init; }
    public TriggerEntity TriggerEntity { get; init; }

    public GardenChildEvent(UserProfileEventTriggerEnum trigger, TriggerEntity entity)
    {
        Trigger = trigger;
        TriggerEntity = entity;
    }
}