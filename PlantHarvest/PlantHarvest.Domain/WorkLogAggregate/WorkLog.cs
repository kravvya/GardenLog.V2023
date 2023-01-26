using PlantHarvest.Domain.WorkLogAggregate.Events;
using PlantHarvest.Domain.WorkLogAggregate.Events.Meta;

namespace PlantHarvest.Domain.WorkLogAggregate;

public class WorkLog : BaseEntity, IAggregateRoot
{
    public string Log { get; private set; }
    public WorkLogEntityEnum RelatedEntity { get; private set; }
    public string RelatedEntityid { get; private set; }
    public DateTime EnteredDateTime { get; private set; }
    public DateTime EventDateTime { get; private set; }
    public WorkLogReasonEnum Reason { get; private set; }
    public string UserProfileId { get; private set; }

    private WorkLog()
    {

    }

    public WorkLog(
        string log,
        WorkLogEntityEnum entity,
        string entityId,
        DateTime enteredDateTime,
        DateTime eventDateTime,
        WorkLogReasonEnum reason,
        string userProfileId)
    {
        this.Log = log;
        this.RelatedEntity = entity;
        this.RelatedEntityid = entityId;
        this.EnteredDateTime = enteredDateTime;
        this.EventDateTime = eventDateTime;
        this.Reason = reason;
        this.UserProfileId = userProfileId;
    }

    public static WorkLog Create(
        string log,
        WorkLogEntityEnum entity,
        string entityId,
        DateTime eventDateTime,
        WorkLogReasonEnum reason,
        string userProfileId
        )
    {
        DateTime timestamp = DateTime.Now;

        var work = new WorkLog()
        {
            Id = Guid.NewGuid().ToString(),
            Log = log,
            RelatedEntity = entity,
            RelatedEntityid = entityId,
            EnteredDateTime = timestamp,
            EventDateTime = eventDateTime,
            Reason = reason,
            UserProfileId = userProfileId
        };

        work.DomainEvents.Add(
            new WorkLogEvent(work, WorkLogEventTriggerEnum.WorkLogCreated, new WorkLogTriggerEntity(WorkLogEntityTypeEnum.WorkLog, work.Id)));

        return work;
    }

    public void Update(
        string log,
        WorkLogEntityEnum entity,
        string entityId,
        DateTime eventDateTime,
        WorkLogReasonEnum reason)
    {
        this.Set < string>(() => this.Log, log);
        this.Set < WorkLogEntityEnum>(() => this.RelatedEntity , entity);
        this.Set <string>(() => this.RelatedEntityid , entityId);
        this.Set < DateTime>(() => this.EventDateTime , eventDateTime);
        this.Set<WorkLogReasonEnum>(() => this.Reason , reason);

       
    }

    protected override void AddDomainEvent(string attributeName)
    {
        this.DomainEvents.Add(
              new WorkLogEvent(this, WorkLogEventTriggerEnum.WorkLogUpdated, new WorkLogTriggerEntity(WorkLogEntityTypeEnum.WorkLog, this.Id)));
    }
}
