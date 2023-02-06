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
        switch(harvestEvent.Trigger)
        {
            case HarvestEventTriggerEnum.PlantHarvestCycleSeeded:
                await GenerateSowIndoorsWorkLog(harvestEvent);
                break;
            case HarvestEventTriggerEnum.PlantHarvestCycleGerminated:
                await GenerateInformationWorkLogForGenrmatedEvent(harvestEvent);
                break;
        }
   
    }

    public async Task GenerateSowIndoorsWorkLog(HarvestEvent harvestEvent)
    {
        var plant = harvestEvent.Harvest.Plants.First(p => p.Id == harvestEvent.TriggerEntity.EntityId);

        StringBuilder note = new();
        if (plant.NumberOfSeeds.HasValue) { note.Append($"{plant.NumberOfSeeds} seeds of "); }
        note.Append(plant.PlantName);
        if (!string.IsNullOrWhiteSpace(plant.SeedCompanyName)) { note.Append($"from {plant.SeedCompanyName} "); }
        note.Append($" were planted indoors on {plant.SeedingDate.Value} ");

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


    public async Task GenerateInformationWorkLogForGenrmatedEvent(HarvestEvent harvestEvent)
    {
        var plant = harvestEvent.Harvest.Plants.First(p => p.Id == harvestEvent.TriggerEntity.EntityId);

        StringBuilder note = new();
        if (plant.GerminationRate.HasValue) { note.Append($"{plant.GerminationRate.Value}% germanation of "); }
        note.Append(plant.PlantName);
        note.Append($"  on {plant.GerminationDate.Value.ToShortDateString()} ");
        if (!string.IsNullOrEmpty(plant.SeedCompanyName)) { note.Append($" from {plant.SeedCompanyName} "); }
       

        var command = new CreateWorkLogCommand()
        {
            EnteredDateTime = DateTime.Now,
            EventDateTime = plant.GerminationDate.Value,
            Log = note.ToString(),
            Reason = WorkLogReasonEnum.Information,
            RelatedEntity = WorkLogEntityEnum.PlantHarvestCycle,
            RelatedEntityid = plant.Id
        };
        await _workLogCommandHandler.CreateWorkLog(command);
    }
}
