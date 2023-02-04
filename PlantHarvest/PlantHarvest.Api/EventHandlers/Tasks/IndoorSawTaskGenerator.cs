using PlantHarvest.Domain.HarvestAggregate.Events;
using PlantHarvest.Domain.WorkLogAggregate.Events;

namespace PlantHarvest.Orchestrator.Tasks;


public class IndoorSawTaskGenerator : INotificationHandler<HarvestEvent>, INotificationHandler<WorkLogEvent>
{
    private readonly IPlantTaskCommandHandler _taskCommandHandler;
    private readonly IPlantTaskQueryHandler _taskQueryHandler;

    public IndoorSawTaskGenerator(IPlantTaskCommandHandler taskCommandHandler, IPlantTaskQueryHandler taskQueryHandler)
    {
        _taskCommandHandler = taskCommandHandler;
        _taskQueryHandler = taskQueryHandler;
    }

    public async Task Handle(HarvestEvent harvestEvent, CancellationToken cancellationToken)
    {
        switch (harvestEvent.Trigger)
        {
            case HarvestEventTriggerEnum.PlantAddedToHarvestCycle:
                await CreateIndoorSowTask(harvestEvent);
                break;
            case HarvestEventTriggerEnum.PlantHarvestCycleDeleted:
                await DeleteIndoorSowTask(harvestEvent);
                break;

        }
    }

    public async Task Handle(WorkLogEvent workEvent, CancellationToken cancellationToken)
    {
        if (workEvent.Work.Reason == WorkLogReasonEnum.SowIndoors && !string.IsNullOrEmpty(workEvent.Work.RelatedEntityid))
        {
            await CompleteIndoorSowTasks(workEvent);
        }
    }

    private async Task CompleteIndoorSowTasks(WorkLogEvent workTask)
    {
        var tasks = await _taskQueryHandler.SearchPlantTasks(new Contract.Query.PlantTaskSearch() { PlantHarvestCycleId = workTask.Work.RelatedEntityid, Reason = WorkLogReasonEnum.SowIndoors });
        if (tasks != null && tasks.Any())
        {
            foreach (var task in tasks)
            {
                await _taskCommandHandler.CompletePlantTask(new UpdatePlantTaskCommand()
                {
                    CompletedDateTime = workTask.Work.EventDateTime,
                    Notes = task.Notes,
                    TargetDateEnd = task.TargetDateEnd,
                    TargetDateStart = task.TargetDateStart
                });
            };
        }
    }

    private async Task DeleteIndoorSowTask(HarvestEvent harvestEvent)
    {
        var plant = harvestEvent.Harvest.Plants.First(plant => plant.Id == harvestEvent.TriggerEntity.entityId);
        var tasks = await _taskQueryHandler.SearchPlantTasks(new Contract.Query.PlantTaskSearch() { PlantHarvestCycleId = plant.Id });
        if (tasks != null && tasks.Any())
        {
            foreach (var task in tasks)
            {
                await _taskCommandHandler.DeletePlantTask(task.PlantTaskId);
            }
        }
    }

    private async Task CreateIndoorSowTask(HarvestEvent harvestEvent)
    {
        var plant = harvestEvent.Harvest.Plants.First(plant => plant.Id == harvestEvent.TriggerEntity.entityId);
        var schedule = plant.PlantCalendar.FirstOrDefault(s => s.TaskType == WorkLogReasonEnum.SowIndoors);

        if (schedule != null)
        {
            var command = new CreatePlantTaskCommand()
            {
                CreatedDateTime = DateTime.UtcNow,
                HarvestCycleId = harvestEvent.HarvestId,
                IsSystemGenerated = true,
                PlantHarvestCycleId = plant.Id,
                PlantName = string.IsNullOrEmpty(plant.PlantVarietyName)? plant.PlantName : $"{plant.PlantName} - {plant.PlantVarietyName}",
                PlantScheduleId = schedule.Id,
                TargetDateStart = schedule.StartDate,
                TargetDateEnd = schedule.EndDate,
                Type = WorkLogReasonEnum.SowIndoors,
                Title = "Sow Seeds Indoors",
                Notes = schedule.Notes,
            };

            await _taskCommandHandler.CreatePlantTask(command);

        }

    }


}
