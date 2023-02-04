using PlantHarvest.Domain.HarvestAggregate.Events;

namespace PlantHarvest.Api.EventHandlers.Tasks;

public class WorkLogGenerator : INotificationHandler<HarvestEvent>
{
    private readonly IWorkLogCommandHandler _workLogCommandHandler;

    public WorkLogGenerator(IWorkLogCommandHandler workLogCommandHandler)
    {
        _workLogCommandHandler = workLogCommandHandler;
    }

    public async Task Handle(HarvestEvent harvestEvent, CancellationToken cancellationToken)
    {

        if (harvestEvent.Trigger == HarvestEventTriggerEnum.PlantHarvestCycleSeeded)
        {
            var plant = harvestEvent.Harvest.Plants.First(p => p.Id == harvestEvent.TriggerEntity.entityId);

            var command = new CreateWorkLogCommand()
            {
                EnteredDateTime = DateTime.Now,
                EventDateTime = plant.SeedingDate.Value,
                Log = $"{plant.NumberOfSeeds} seeds of {plant.PlantName} from {plant.SeedCompanyName} were planted indoors on {plant.SeedingDate.Value}",
                Reason = WorkLogReasonEnum.SowIndoors,
                RelatedEntity = WorkLogEntityEnum.PlantHarvestCycle,
                RelatedEntityid = plant.Id
            };
            await _workLogCommandHandler.CreateWorkLog(command);
        }
    }
}
