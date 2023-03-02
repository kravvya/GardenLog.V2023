using PlantHarvest.Infrastructure.ApiClients;
using System.Collections.ObjectModel;
using System.Reflection;

namespace PlantHarvest.Api.Schedules;

public interface IScheduleBuilder
{
    Task<ReadOnlyCollection<CreatePlantScheduleCommand>> GeneratePlantCalendarBasedOnGrowInstruction(PlantHarvestCycle plantHarvest, string plantId, string growInstructionId, string plantVarietyId, string gardenId);
}

public class ScheduleBuilder : IScheduleBuilder
{
    private readonly IPlantCatalogApiClient _plantCatalogApi;
    private readonly IUserManagementApiClient _userManagementApi;
    private readonly ILogger<ScheduleBuilder> _logger;
    private List<IScheduler> _schedulers;

    public ScheduleBuilder(IPlantCatalogApiClient plantCatalogApi, IUserManagementApiClient userManagementApi, ILogger<ScheduleBuilder> logger)
    {
        _plantCatalogApi = plantCatalogApi;
        _userManagementApi = userManagementApi;
        _logger = logger;
        LoadSchedulers();
    }

    public async Task<ReadOnlyCollection<CreatePlantScheduleCommand>> GeneratePlantCalendarBasedOnGrowInstruction(PlantHarvestCycle plantHarvest, string plantId, string growInstructionId, string plantVarietyId, string gardenId)
    {
        List<CreatePlantScheduleCommand> plantSchedules = new List<CreatePlantScheduleCommand>();

        var growInstructionTask = _plantCatalogApi.GetPlantGrowInstruction(plantId, growInstructionId);
        var gardenTask = _userManagementApi.GetGarden(gardenId);

        Task.WaitAll(growInstructionTask, gardenTask);

        var growInstruction = growInstructionTask.Result;
        var garden = gardenTask.Result;
        int? daysToMaturityMin;
        int? daysToMaturityMax;

        if (growInstruction == null || garden == null)
        {
            return null;
        }

        if (!string.IsNullOrEmpty(plantVarietyId))
        {
            var plantVariety = await _plantCatalogApi.GetPlantVariety(plantId, plantVarietyId);
            daysToMaturityMin = plantVariety.DaysToMaturityMin;
            daysToMaturityMax = plantVariety.DaysToMaturityMax;
        }
        else
        {
            var plant = await _plantCatalogApi.GetPlant(plantId);
            daysToMaturityMin = plant.DaysToMaturityMin;
            daysToMaturityMax = plant.DaysToMaturityMax;
        }


        _schedulers.Where(s => s.CanSchedule(growInstruction)).ToList().ForEach(s =>
        {
            try
            {
                var schedule = s.Schedule(plantHarvest, growInstruction, garden, daysToMaturityMin, daysToMaturityMax);
                if (schedule != null)
                {
                    plantSchedules.Add(schedule);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Excountered excetion processing scheduler", ex);
            }
           
        });

        return plantSchedules.AsReadOnly();
    }

    private void LoadSchedulers()
    {
        _schedulers = new();
        Assembly asm = Assembly.GetExecutingAssembly();

        foreach (Type type in asm.GetTypes())
        {
            if (type.GetInterfaces().Contains(typeof(IScheduler)))
            {
                IScheduler obj = (IScheduler)Activator.CreateInstance(type);
                Console.WriteLine("Instance created: " + obj.GetType().Name);
                _schedulers.Add(obj);
            }
        }
    }
}
