using GardenLog.SharedKernel.Interfaces;
using MediatR;
using PlantCatalog.Contract.ViewModels;
using PlantHarvest.Api.Extensions;
using PlantHarvest.Infrastructure.ApiClients;
using System.Collections.ObjectModel;

namespace PlantHarvest.Api.CommandHandlers;


public interface IHarvestCommandHandler
{
    Task<string> CreateHarvestCycle(CreateHarvestCycleCommand request);
    Task<string> DeleteHarvestCycle(string id);
    Task<string> UpdateHarvestCycle(UpdateHarvestCycleCommand request);

    Task<string> AddPlantHarvestCycle(CreatePlantHarvestCycleCommand request);
    Task<string> DeletePlantHarvestCycle(string harvestCyleId, string id);
    Task<string> UpdatePlantHarvestCycle(UpdatePlantHarvestCycleCommand request);

    Task<string> AddPlantSchedule(CreatePlantScheduleCommand command);
    Task<string> UpdatePlantSchedule(UpdatePlantScheduleCommand command);
    Task<string> DeletePlantSchedule(string harvestCycleId, string plantHarvestCycleId, string plantScheduleId);
}

public class HarvestCommandHandler : IHarvestCommandHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHarvestCycleRepository _harvestCycleRepository;
    private readonly ILogger<HarvestCommandHandler> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IScheduleBuilder _scheduleBuilder;
    private readonly IMediator _mediator;

    public HarvestCommandHandler(IUnitOfWork unitOfWork, IHarvestCycleRepository harvestCycleRepository, ILogger<HarvestCommandHandler> logger, IHttpContextAccessor httpContextAccessor, IScheduleBuilder scheduleBuilder, IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _harvestCycleRepository = harvestCycleRepository;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _scheduleBuilder = scheduleBuilder;
        _mediator = mediator;
    }

    #region Harvest Cycle

    public async Task<string> CreateHarvestCycle(CreateHarvestCycleCommand request)
    {
        _logger.LogInformation("Received request to create a new harvest cycle {@plant}", request);

        string userProfileId = _httpContextAccessor.HttpContext?.User.GetUserProfileId(_httpContextAccessor.HttpContext.Request.Headers);

        var existingHarvestId = await _harvestCycleRepository.GetIdByNameAsync(request.HarvestCycleName, userProfileId);

        if (!string.IsNullOrEmpty(existingHarvestId))
        {
            throw new ArgumentException("Garden Plan with this name already exists", nameof(request.HarvestCycleName));
        }

        var harvest = HarvestCycle.Create(
           userProfileId,
           request.HarvestCycleName,
           request.StartDate,
           request.EndDate,
           request.Notes,
           request.GardenId);

        _harvestCycleRepository.Add(harvest);

        await _mediator.DispatchDomainEventsAsync(harvest);

        await _unitOfWork.SaveChangesAsync();

        return harvest.Id;
    }

    public async Task<string> UpdateHarvestCycle(UpdateHarvestCycleCommand request)
    {
        _logger.LogInformation("Received request to update harvest cycle {@harvest}", request);

        string userProfileId = _httpContextAccessor.HttpContext?.User.GetUserProfileId(_httpContextAccessor.HttpContext.Request.Headers);

        var existingHarvestId = await _harvestCycleRepository.GetIdByNameAsync(request.HarvestCycleName, userProfileId);
        if (!string.IsNullOrEmpty(existingHarvestId) && existingHarvestId != request.HarvestCycleId)
        {
            throw new ArgumentException("Another garden plan with this name already exists", nameof(request.HarvestCycleName));
        }

        var harvest = await _harvestCycleRepository.GetByIdAsync(request.HarvestCycleId);

        harvest.Update(request.HarvestCycleName, request.StartDate, request.EndDate, request.Notes, request.GardenId);

        _harvestCycleRepository.Update(harvest);

        await _mediator.DispatchDomainEventsAsync(harvest);

        await _unitOfWork.SaveChangesAsync();

        return harvest.Id;
    }

    public async Task<string> DeleteHarvestCycle(string id)
    {
        _logger.LogInformation("Received request to delete harvest cycle {@id}", id);
        var harvest = await _harvestCycleRepository.GetByIdAsync(id);

        harvest.Delete();

        _harvestCycleRepository.Delete(id);

        await _mediator.DispatchDomainEventsAsync(harvest);

        await _unitOfWork.SaveChangesAsync();

        return id;
    }
    #endregion

    #region Harvest Cycle Plant

    public async Task<String> AddPlantHarvestCycle(CreatePlantHarvestCycleCommand command)
    {
        _logger.LogInformation("Received request to create plant harvest cycle {@plantHarvestCycle}", command);
        try
        {
            string userProfileId = _httpContextAccessor.HttpContext?.User.GetUserProfileId(_httpContextAccessor.HttpContext.Request.Headers);

            var harvest = await _harvestCycleRepository.GetByIdAsync(command.HarvestCycleId);

            if (harvest.Plants.Any(g => g.PlantId == command.PlantId && (g.PlantVarietyId == command.PlantVarietyId)))
            {
                throw new ArgumentException("This plant is already a part of this plan", nameof(command.PlantVarietyId));
            }

            var plantId = harvest.AddPlantHarvestCycle(command);

            try
            {
                var generatedSchedules = await _scheduleBuilder.GeneratePlantCalendarBasedOnGrowInstruction(command.PlantId, command.PlantGrowthInstructionId, command.PlantVarietyId, harvest.GardenId);

                ApplyPlantSchedules(generatedSchedules, harvest, plantId);
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception generating plant schedules. PLantHarvest will still save", ex);
            }


            _harvestCycleRepository.AddPlantHarvestCycle(plantId, harvest);
            //_harvestCycleRepository.Update(harvest);

            await _mediator.DispatchDomainEventsAsync(harvest);

            await _unitOfWork.SaveChangesAsync();

            return plantId;
        }
        catch (Exception ex)
        {
            _logger.LogCritical("Exception adding plant grow instruction", ex);
            throw;
        }

    }

    public async Task<String> UpdatePlantHarvestCycle(UpdatePlantHarvestCycleCommand command)
    {
        _logger.LogInformation("Received request to create plant harvest cycle {@plantHarvestCycle}", command);
        var harvest = await _harvestCycleRepository.GetByIdAsync(command.HarvestCycleId);

        if (harvest.Plants.Any(g => g.PlantId == command.PlantId && g.PlantVarietyId == command.PlantVarietyId && g.Id != command.PlantHarvestCycleId))
        {
            throw new ArgumentException("This plant is already a part of this plan", nameof(command.PlantVarietyId));
        }

        harvest.UpdatePlantHarvestCycle(command);

        try
        {
            var generatedSchedules = await _scheduleBuilder.GeneratePlantCalendarBasedOnGrowInstruction(command.PlantId, command.PlantGrowthInstructionId, command.PlantVarietyId, harvest.GardenId);

            ApplyPlantSchedules(generatedSchedules, harvest, command.PlantHarvestCycleId);

        }
        catch (Exception ex)
        {
            _logger.LogError("Exception generating plant schedules. PLantHarvest will still save", ex);
        }

        _harvestCycleRepository.UpdatePlantHarvestCycle(command.PlantHarvestCycleId, harvest);
        //_harvestCycleRepository.Update(harvest);

        await _mediator.DispatchDomainEventsAsync(harvest);

        await _unitOfWork.SaveChangesAsync();

        return command.PlantHarvestCycleId;
    }

    public async Task<String> DeletePlantHarvestCycle(string harvestCycleId, string id)
    {
        _logger.LogInformation($"Received request to delete plant harvest cycle  {harvestCycleId} and {id}");

        var harvest = await _harvestCycleRepository.GetByIdAsync(harvestCycleId);

        harvest.DeletePlantHarvestCycle(id);

        _harvestCycleRepository.DeletePlantHarvestCycle(id, harvest);

        await _mediator.DispatchDomainEventsAsync(harvest);

        await _unitOfWork.SaveChangesAsync();

        return id;
    }

    #endregion

    #region Plant Schedule
    public async Task<String> AddPlantSchedule(CreatePlantScheduleCommand command)
    {
        _logger.LogInformation("Received request to create plant schedule {@plantHarvestCycle}", command);
        try
        {
            var harvest = await _harvestCycleRepository.GetByIdAsync(command.HarvestCycleId);

            var plant = harvest.Plants.First(p => p.Id == command.PlantHarvestCycleId);
            if (plant == null)
            {
                throw new ArgumentException("Schedule can only be added to a plant that is part of the Garden Plan", nameof(command.PlantHarvestCycleId));
            }

            //if (plant.PlantCalendar.Any(g => g.TaskType == command.TaskType))
            //{
            //    throw new ArgumentException($"This task is already scheduled", nameof(command.TaskType));
            //}

            var scheduleId = harvest.AddPlantSchedule(command);

            _harvestCycleRepository.Update(harvest);

            await _mediator.DispatchDomainEventsAsync(harvest);

            await _unitOfWork.SaveChangesAsync();

            return scheduleId;
        }
        catch (Exception ex)
        {
            _logger.LogCritical("Exception adding plant schedule", ex);
            throw;
        }

    }

    public async Task<String> UpdatePlantSchedule(UpdatePlantScheduleCommand command)
    {
        _logger.LogInformation("Received request to update plant schedule {@plantHarvestCycle}", command);
        var harvest = await _harvestCycleRepository.GetByIdAsync(command.HarvestCycleId);

        var plant = harvest.Plants.First(p => p.Id == command.PlantHarvestCycleId);
        if (plant == null)
        {
            throw new ArgumentException("Schedule can only be added to a plant that is part of the Garden Plan", nameof(command.PlantHarvestCycleId));
        }

        //if (plant.PlantCalendar.Any(g => g.TaskType == command.TaskType && g.Id != command.PlantScheduleId))
        //{
        //    throw new ArgumentException($"This type of task is already scheduled", nameof(command.TaskType));
        //}

        harvest.UpdatePlantSchedule(command);

        _harvestCycleRepository.Update(harvest);

        await _mediator.DispatchDomainEventsAsync(harvest);

        await _unitOfWork.SaveChangesAsync();

        return command.PlantScheduleId;
    }

    public async Task<String> DeletePlantSchedule(string harvestCycleId, string plantHarvestCycleId, string plantScheduleId)
    {
        _logger.LogInformation($"Received request to delete plant schedule  {harvestCycleId} and {plantHarvestCycleId} and {plantScheduleId}");

        var harvest = await _harvestCycleRepository.GetByIdAsync(harvestCycleId);

        harvest.DeletePlantSchedule(plantHarvestCycleId, plantScheduleId);

        _harvestCycleRepository.Update(harvest);

        await _mediator.DispatchDomainEventsAsync(harvest);

        await _unitOfWork.SaveChangesAsync();

        return plantScheduleId;
    }

    #endregion

    private void ApplyPlantSchedules(ReadOnlyCollection<CreatePlantScheduleCommand> schedules, HarvestCycle harvest, string plantHarvestCycleId)
    {
        if (schedules == null || schedules.Count == 0) { return; }

        harvest.DeleteAllSystemGeneratedSchedules(plantHarvestCycleId);

        foreach (var schedule in schedules)
        {
            schedule.PlantHarvestCycleId = plantHarvestCycleId;
            harvest.AddPlantSchedule(schedule);
        }

    }
}

