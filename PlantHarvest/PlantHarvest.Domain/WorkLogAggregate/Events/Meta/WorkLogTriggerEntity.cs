namespace PlantHarvest.Domain.WorkLogAggregate.Events.Meta;

public record WorkLogTriggerEntity(WorkLogEntityTypeEnum entityType, string entityId);