using GardenLog.SharedKernel.Enum;
using MongoDB.Driver.Linq;
using PlantHarvest.Domain.HarvestAggregate.Events;
using PlantHarvest.Domain.WorkLogAggregate.Events;
using PlantHarvest.Infrastructure.ApiClients;
using System.Text;

namespace PlantHarvest.Orchestrator.Tasks;


public class FertilizeIndoorsTaskGenerator : INotificationHandler<HarvestEvent>, INotificationHandler<WorkLogEvent>
{
    private readonly IPlantTaskCommandHandler _taskCommandHandler;
    private readonly IPlantTaskQueryHandler _taskQueryHandler;
    private readonly IPlantCatalogApiClient _plantCatalogApi;
    private readonly IHarvestQueryHandler _harvestQueryHandler;
    private readonly ILogger<FertilizeIndoorsTaskGenerator> _logger;

    public FertilizeIndoorsTaskGenerator(IPlantTaskCommandHandler taskCommandHandler, IPlantTaskQueryHandler taskQueryHandler, IPlantCatalogApiClient plantCatalogApi, IHarvestQueryHandler harvestQueryHandler, ILogger<FertilizeIndoorsTaskGenerator> logger)
    {
        _taskCommandHandler = taskCommandHandler;
        _taskQueryHandler = taskQueryHandler;
        _plantCatalogApi = plantCatalogApi;
        _harvestQueryHandler = harvestQueryHandler;
        _logger = logger;
    }

    public async Task Handle(HarvestEvent harvestEvent, CancellationToken cancellationToken)
    {
        switch (harvestEvent.Trigger)
        {
            case HarvestEventTriggerEnum.PlantHarvestCycleGerminated:
                await CreateFertilizeIndoorsTask(harvestEvent);
                break;
            case HarvestEventTriggerEnum.PlantHarvestCycleTransplanted:
                await DeleteFertilizeIndoorsTask(harvestEvent);
                break;
            case HarvestEventTriggerEnum.PlantHarvestCycleDeleted:
                await DeleteFertilizeIndoorsTask(harvestEvent);
                break;

        }
    }

    public async Task Handle(WorkLogEvent workEvent, CancellationToken cancellationToken)
    {
        if (workEvent.Work.Reason == WorkLogReasonEnum.FertilizeIndoors)
        {
            await CompleteFertilizeIndoorsTask(workEvent);
            await CreateFertilizeIndoorsTask(workEvent);
        }
    }

    private async Task CompleteFertilizeIndoorsTask(WorkLogEvent workEvent)
    {
        var plantHarvestRelatedEntity = workEvent.Work.RelatedEntities.FirstOrDefault(e => e.EntityType == RelatedEntityTypEnum.PlantHarvestCycle);

        if (plantHarvestRelatedEntity != null)
        {
            var tasks = await _taskQueryHandler.SearchPlantTasks(new Contract.Query.PlantTaskSearch() { PlantHarvestCycleId = plantHarvestRelatedEntity.EntityId, Reason = WorkLogReasonEnum.FertilizeIndoors });
            if (tasks != null && tasks.Any())
            {
                foreach (var task in tasks)
                {
                    await _taskCommandHandler.CompletePlantTask(new UpdatePlantTaskCommand()
                    {
                        PlantTaskId = task.PlantTaskId,
                        CompletedDateTime = workEvent.Work.EventDateTime,
                        Notes = task.Notes,
                        TargetDateEnd = task.TargetDateEnd,
                        TargetDateStart = task.TargetDateStart
                    });
                };
            }
        }
        else
        {
            _logger.LogError($"Unable to complete task based on workLog: {workEvent.Work.Id}. Plant Harvest is not found");
        }
    }

    private async Task CreateFertilizeIndoorsTask(HarvestEvent harvestEvent)
    {
        var plantHarvest = harvestEvent.Harvest.Plants.First(plant => plant.Id == harvestEvent.TriggerEntity.EntityId);
        if (plantHarvest.PlantingMethod != PlantingMethodEnum.SeedIndoors || !plantHarvest.GerminationDate.HasValue || plantHarvest.TransplantDate.HasValue)
        {
            return;
        }

        var growInstruction = await _plantCatalogApi.GetPlantGrowInstruction(plantHarvest.PlantId, plantHarvest.PlantGrowthInstructionId);

        if (growInstruction == null || growInstruction.FertilizerForSeedlings == PlantCatalog.Contract.Enum.FertilizerEnum.Unspecified)
        {
            return;
        }

        //assume this is first time we are going to fertilize. So use germination date as a base. All subsequent fertilizations will be based onthe last fertilie event from WorkLog
        if (growInstruction != null)
        {
            var firstFertilizeDate = growInstruction.FertilizeFrequencyForSeedlingsInWeeks.HasValue ?
                                plantHarvest.GerminationDate.Value.AddDays(7 * growInstruction.FertilizeFrequencyForSeedlingsInWeeks.Value) :
                                plantHarvest.GerminationDate.Value.AddDays(7 * GlobalConstants.DEFAULT_FertilizeFrequencyForSeedlingsInWeeks);


            var command = new CreatePlantTaskCommand()
            {
                CreatedDateTime = DateTime.UtcNow,
                HarvestCycleId = harvestEvent.HarvestId,
                IsSystemGenerated = true,
                PlantHarvestCycleId = plantHarvest.Id,
                PlantName = string.IsNullOrEmpty(plantHarvest.PlantVarietyName) ? plantHarvest.PlantName : $"{plantHarvest.PlantName} - {plantHarvest.PlantVarietyName}",
                PlantScheduleId = string.Empty,
                TargetDateStart = firstFertilizeDate,
                TargetDateEnd = firstFertilizeDate.AddDays(1),
                Type = WorkLogReasonEnum.FertilizeIndoors,
                Title = "FErtilize Seedlings",
                Notes = GetFertilizeIndoorsNotes(growInstruction)
            };

            await _taskCommandHandler.CreatePlantTask(command);

        }

    }

    private async Task CreateFertilizeIndoorsTask(WorkLogEvent workLogEvent)
    {
        return;
        //TODO - to make this work, worklog needs both HarvestCycleId and PlantHarvestCycleId. 
        var plantHarvest = await _harvestQueryHandler.GetPlantHarvestCycle("", "");

        if (plantHarvest.PlantingMethod != PlantingMethodEnum.SeedIndoors || !plantHarvest.GerminationDate.HasValue || plantHarvest.TransplantDate.HasValue)
        {
            return;
        }

        var growInstruction = await _plantCatalogApi.GetPlantGrowInstruction(plantHarvest.PlantId, plantHarvest.PlantGrowthInstructionId);

        if (growInstruction == null || growInstruction.FertilizerForSeedlings == PlantCatalog.Contract.Enum.FertilizerEnum.Unspecified)
        {
            return;
        }

        //assume this is first time we are going to fertilize. So use germination date as a base. All subsequent fertilizations will be based onthe last fertilie event from WorkLog

        var firstFertilizeDate = growInstruction.FertilizeFrequencyForSeedlingsInWeeks.HasValue ?
                            plantHarvest.GerminationDate.Value.AddDays(7 * growInstruction.FertilizeFrequencyForSeedlingsInWeeks.Value) :
                            plantHarvest.GerminationDate.Value.AddDays(7 * GlobalConstants.DEFAULT_FertilizeFrequencyForSeedlingsInWeeks);

        var command = new CreatePlantTaskCommand()
        {
            CreatedDateTime = DateTime.UtcNow,
            HarvestCycleId = plantHarvest.HarvestCycleId,
            IsSystemGenerated = true,
            PlantHarvestCycleId = plantHarvest.PlantHarvestCycleId,
            PlantName = string.IsNullOrEmpty(plantHarvest.PlantVarietyName) ? plantHarvest.PlantName : $"{plantHarvest.PlantName} - {plantHarvest.PlantVarietyName}",
            PlantScheduleId = string.Empty,
            TargetDateStart = firstFertilizeDate,
            TargetDateEnd = firstFertilizeDate.AddDays(1),
            Type = WorkLogReasonEnum.FertilizeIndoors,
            Title = "Fertilize Seedlings",
            Notes = GetFertilizeIndoorsNotes(growInstruction)
        };

        await _taskCommandHandler.CreatePlantTask(command);
    }

    private static string GetFertilizeIndoorsNotes(PlantGrowInstructionViewModel growInstruction)
    {
        StringBuilder notes = new();
        notes.Append($"Fertilize with {growInstruction.FertilizerForSeedlings.GetDescription()} ");
        if (growInstruction.FertilizeFrequencyForSeedlingsInWeeks.HasValue)
            notes.Append($" every {growInstruction.FertilizeFrequencyForSeedlingsInWeeks.Value} weeks.");
        else
            notes.Append($" every {GlobalConstants.DEFAULT_FertilizeFrequencyForSeedlingsInWeeks} weeks.");
        return notes.ToString();
    }

    private async Task DeleteFertilizeIndoorsTask(HarvestEvent harvestEvent)
    {
        var plantHarvest = harvestEvent.Harvest.Plants.First(plant => plant.Id == harvestEvent.TriggerEntity.EntityId);
        var tasks = await _taskQueryHandler.SearchPlantTasks(new Contract.Query.PlantTaskSearch() { PlantHarvestCycleId = plantHarvest.Id });
        if (tasks != null && tasks.Any())
        {
            foreach (var task in tasks)
            {
                await _taskCommandHandler.DeletePlantTask(task.PlantTaskId);
            }
        }
    }

}
