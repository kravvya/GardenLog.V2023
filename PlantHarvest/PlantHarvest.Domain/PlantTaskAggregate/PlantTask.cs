

namespace PlantHarvest.Domain.WorkLogAggregate;

public class PlantTask : BaseEntity, IAggregateRoot
{
    public string Description { get; private set; }
    public WorkLogReasonEnum Type { get; private set; }
    public DateTime CreatedDateTime { get; private set; }
    public DateTime TargetDateStart { get; private set; }
    public DateTime TargetDateEnd { get; private set; }
    public DateTime? CompletedDateTime { get; private set; }
    public string HarvestCycleId { get; private set; }
    public string PlantHarvestCycleId { get; private set; }
    public string PlantName { get; private set; }
    public string PlantScheduleId { get; private set; }
    public string Notes { get; private set; }
    public bool IsSystemGenerated { get; private set; }
    public string UserProfileId { get; private set; }

    private PlantTask()
    {

    }

    private PlantTask(
        string description,
        WorkLogReasonEnum type,
        DateTime createdDateTime,
        DateTime targetDateStart,
        DateTime targetDateEnd,
        DateTime? completedDateTime,
        string harvestCycleId,
        string plantHarvestCycleId,
        string plantName,
        string plantScheduleId,
        string notes,
        bool isSystemGenerated,
        string userProfileId
        )
    {
        this.Description = description;
        this.Type = type;
        this.CreatedDateTime = createdDateTime;
        this.TargetDateStart = targetDateStart;
        this.TargetDateEnd = targetDateEnd;
        this.CompletedDateTime = completedDateTime;
        this.HarvestCycleId = harvestCycleId;
        this.PlantHarvestCycleId = plantHarvestCycleId;
        this.PlantName = plantName;
        this.PlantScheduleId = plantScheduleId;
        this.Notes = notes;
        this.IsSystemGenerated = isSystemGenerated;
        this.UserProfileId = userProfileId;
    }

    public static PlantTask Create(
        string description,
        WorkLogReasonEnum type,
        DateTime createdDateTime,
        DateTime targetDateStart,
        DateTime targetDateEnd,
        DateTime? completedDateTime,
        string harvestCycleId,
        string plantHarvestCycleId,
        string plantName,
        string plantScheduleId,
        string notes,
        bool isSystemGenerated,
        string userProfileId
        )
    {
       
        var task = new PlantTask()
        {
            Id = Guid.NewGuid().ToString(),
            Description = description,
            Type = type,
            CreatedDateTime = createdDateTime,
            TargetDateStart = targetDateStart,
            TargetDateEnd = targetDateEnd,
            CompletedDateTime = completedDateTime,
            HarvestCycleId = harvestCycleId,
            PlantHarvestCycleId = plantHarvestCycleId,
            PlantName = plantName,
            PlantScheduleId = plantScheduleId,
            Notes = notes,
            IsSystemGenerated = isSystemGenerated,
            UserProfileId = userProfileId
        };

        task.DomainEvents.Add(
            new PlantTaskEvent(task, PlantTaskEventTriggerEnum.PlantTaskCreated, new PlantTaskTriggerEntity(PlantTaskEntityTypeEnum.PlantTask, task.Id)));

        return task;
    }

    public void Update(
        DateTime targetDateStart,
        DateTime targetDateEnd,
        DateTime? completedDateTime,
        string notes     
        )
    {
        this.Set<DateTime>(() => this.TargetDateStart, targetDateStart);
        this.Set<DateTime>(() => this.TargetDateStart, targetDateStart);
        this.Set<DateTime?>(() => this.CompletedDateTime, completedDateTime);
        this.Set<string>(() => this.Notes, notes);

    }

    protected override void AddDomainEvent(string attributeName)
    {
        PlantTaskEventTriggerEnum taskEvent = attributeName == "CompletedDate" && this.CompletedDateTime.HasValue ? 
                        PlantTaskEventTriggerEnum.PlantTaskCompleted : PlantTaskEventTriggerEnum.PlantTaskUpdated;
        this.DomainEvents.Add(
              new PlantTaskEvent(this, taskEvent, new PlantTaskTriggerEntity(PlantTaskEntityTypeEnum.PlantTask, this.Id)));
    }
}
