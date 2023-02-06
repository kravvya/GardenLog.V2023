using GardenLog.SharedKernel;
using GardenLog.SharedKernel.Enum;
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
        var plantHarvest = harvestEvent.Harvest.Plants.First((Func<PlantHarvestCycle, bool>)(p => p.Id == harvestEvent.TriggerEntity.EntityId));

        StringBuilder note = new();
        if (plantHarvest.NumberOfSeeds.HasValue) { note.Append($"{plantHarvest.NumberOfSeeds} seeds of "); }
        note.Append(plantHarvest.PlantName);
        if (!string.IsNullOrWhiteSpace(plantHarvest.SeedCompanyName)) { note.Append($"from {plantHarvest.SeedCompanyName} "); }
        note.Append($" were planted indoors on {plantHarvest.SeedingDate.Value.ToShortDateString()} ");

        var relatedEntities = new List<GardenLog.SharedKernel.RelatedEntity>();
        relatedEntities.Add(new GardenLog.SharedKernel.RelatedEntity(RelatedEntityTypEnum.HarvestCycle, harvestEvent.Harvest.Id, harvestEvent.Harvest.HarvestCycleName));
        string plantName = string.IsNullOrEmpty(plantHarvest.PlantVarietyName) ? plantHarvest.PlantName : $"{plantHarvest.PlantName}-{plantHarvest.PlantVarietyName}";
        relatedEntities.Add(new GardenLog.SharedKernel.RelatedEntity(RelatedEntityTypEnum.PlantHarvestCycle, plantHarvest.Id, plantName));

        var command = new CreateWorkLogCommand()
        {
            EnteredDateTime = DateTime.Now,
            EventDateTime = plantHarvest.SeedingDate.Value,
            Log = note.ToString(),
            Reason = WorkLogReasonEnum.SowIndoors,
            RelatedEntities = relatedEntities
        };
        await _workLogCommandHandler.CreateWorkLog(command);
    }


    public async Task GenerateInformationWorkLogForGenrmatedEvent(HarvestEvent harvestEvent)
    {
        var plantHarvest = harvestEvent.Harvest.Plants.First((Func<PlantHarvestCycle, bool>)(p => p.Id == harvestEvent.TriggerEntity.EntityId));

        StringBuilder note = new();
        if (plantHarvest.GerminationRate.HasValue) { note.Append($"{plantHarvest.GerminationRate.Value}% germanation of "); }
        note.Append(plantHarvest.PlantName);
        note.Append($"  on {plantHarvest.GerminationDate.Value.ToShortDateString()} ");
        if (!string.IsNullOrEmpty(plantHarvest.SeedCompanyName)) { note.Append($" from {plantHarvest.SeedCompanyName} "); }

        var relatedEntities = new List<GardenLog.SharedKernel.RelatedEntity>();
        relatedEntities.Add(new GardenLog.SharedKernel.RelatedEntity(RelatedEntityTypEnum.HarvestCycle, harvestEvent.Harvest.Id, harvestEvent.Harvest.HarvestCycleName));
        relatedEntities.Add(new GardenLog.SharedKernel.RelatedEntity(RelatedEntityTypEnum.PlantHarvestCycle, plantHarvest.Id, plantHarvest.PlantName));

        var command = new CreateWorkLogCommand()
        {
            EnteredDateTime = DateTime.Now,
            EventDateTime = plantHarvest.GerminationDate.Value,
            Log = note.ToString(),
            Reason = WorkLogReasonEnum.Information,
            RelatedEntities = relatedEntities
        };
        await _workLogCommandHandler.CreateWorkLog(command);
    }
}
