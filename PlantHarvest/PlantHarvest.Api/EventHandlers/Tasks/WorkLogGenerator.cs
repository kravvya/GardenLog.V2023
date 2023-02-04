using PlantHarvest.Domain.HarvestAggregate.Events;
using System.Text;

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

            StringBuilder note = new();
            if (plant.NumberOfSeeds.HasValue) { note.Append($"{plant.NumberOfSeeds} seeds of "); }
            note.Append(plant.PlantName);
            if (plant.NumberOfSeeds.HasValue) { note.Append($"from {plant.SeedCompanyName} "); }
            note.Append($" were planted indoors on { plant.SeedingDate.Value} ");

            var command = new CreateWorkLogCommand()
            {
                EnteredDateTime = DateTime.Now,
                EventDateTime = plant.SeedingDate.Value,
                Log = note.ToString(),
                Reason = WorkLogReasonEnum.SowIndoors,
                RelatedEntity = WorkLogEntityEnum.PlantHarvestCycle,
                RelatedEntityid = plant.Id
            };
            await _workLogCommandHandler.CreateWorkLog(command);
        }
    }
}
