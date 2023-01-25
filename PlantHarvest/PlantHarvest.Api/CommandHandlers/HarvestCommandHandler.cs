using GardenLog.SharedInfrastructure.Extensions;
using GardenLog.SharedKernel.Interfaces;

namespace PlantHarvest.Api.CommandHandlers;


public interface IHarvestCommandHandler
{
    Task<string> CreateHarvestCycle(CreateHarvestCycleCommand request);
    Task<string> DeleteHarvestCycle(string id);
    Task<string> UpdateHarvestCycle(UpdateHarvestCycleCommand request);
          
    Task<string> AddPlantHarvestCycle(CreatePlantHarvestCycleCommand request);
    Task<string> DeletePlantHarvestCycle(string harvestCyleId, string id);
    Task<string> UpdatePlantHarvestCycle(UpdatePlantHarvestCycleCommand request);

    Task<string> AddPlanHarvestCycle(CreatePlanHarvestCycleCommand command);
    Task<string> DeletePlanHarvestCycle(string harvestCyleId, string id);
    Task<string> UpdatePlanHarvestCycle(UpdatePlanHarvestCycleCommand command);
    
}

public class HarvestCommandHandler : IHarvestCommandHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHarvestCycleRepository _harvestCycleRepository;
    private readonly ILogger<HarvestCommandHandler> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HarvestCommandHandler(IUnitOfWork unitOfWork, IHarvestCycleRepository harvestCycleRepository, ILogger<HarvestCommandHandler> logger, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _harvestCycleRepository = harvestCycleRepository;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    #region Harvest Cycle

    public async Task<string> CreateHarvestCycle(CreateHarvestCycleCommand request)
    {
        _logger.LogInformation("Received request to create a new harvest cycle {@plant}", request);

        string userProfileId = _httpContextAccessor.HttpContext?.User.GetUserProfileId();

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

        await _unitOfWork.SaveChangesAsync();

        return harvest.Id;
    }

    public async Task<string> UpdateHarvestCycle(UpdateHarvestCycleCommand request)
    {
        _logger.LogInformation("Received request to update harvest cycle {@harvest}", request);

        string userProfileId = _httpContextAccessor.HttpContext?.User.GetUserProfileId();

        var existingHarvestId = await _harvestCycleRepository.GetIdByNameAsync(request.HarvestCycleName, userProfileId);
        if (!string.IsNullOrEmpty(existingHarvestId) && existingHarvestId != request.HarvestCycleId)
        {
            throw new ArgumentException("Another garden plan with this name already exists", nameof(request.HarvestCycleName));
        }

        var harvest = await _harvestCycleRepository.GetByIdAsync(request.HarvestCycleId);

        harvest.Update(request.HarvestCycleName, request.StartDate, request.EndDate, request.Notes, request.GardenId);

        _harvestCycleRepository.Update(harvest);

        await _unitOfWork.SaveChangesAsync();

        return harvest.Id;
    }

    public async Task<string> DeleteHarvestCycle(string id)
    {
        _logger.LogInformation("Received request to delete harvest cycle {@id}", id);

        _harvestCycleRepository.Delete(id);

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
            string userProfileId = _httpContextAccessor.HttpContext?.User.GetUserProfileId();

            var harvest = await _harvestCycleRepository.GetByIdAsync(command.HarvestCycleId);

            if (harvest.Plants.Any(g => g.PlantId == command.PlantId && (g.PlantVarietyId == command.PlantVarietyId)))
            {
                throw new ArgumentException("This plant is already a part of this plan", nameof(command.PlantVarietyId));
            }

            var plantId = harvest.AddPlantHarvestCycle(command, userProfileId);

            _harvestCycleRepository.AddPlantHarvestCycle(plantId, harvest);

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

        _harvestCycleRepository.UpdatePlantHarvestCycle(command.PlantHarvestCycleId, harvest);

        await _unitOfWork.SaveChangesAsync();

        return command.PlantHarvestCycleId;
    }

    public async Task<String> DeletePlantHarvestCycle(string harvestCycleId, string id)
    {
        _logger.LogInformation($"Received request to delete plant harvest cycle  {harvestCycleId} and {id}");

        var harvest = await _harvestCycleRepository.GetByIdAsync(harvestCycleId);

        harvest.DeletePlantHarvestCycle(id);

        _harvestCycleRepository.DeletePlantHarvestCycle(id, harvest);

        await _unitOfWork.SaveChangesAsync();

        return id;
    }

    #endregion

    #region Harvest Cycle Plan

    public async Task<String> AddPlanHarvestCycle(CreatePlanHarvestCycleCommand command)
    {
        _logger.LogInformation("Received request to create plan harvest cycle {@planHarvestCycle}", command);
        try
        {
            string userProfileId = _httpContextAccessor.HttpContext?.User.GetUserProfileId();

            var harvest = await _harvestCycleRepository.GetByIdAsync(command.HarvestCycleId);

            if (harvest.Plants.Any(g => g.PlantId == command.PlantId ))
            {
                throw new ArgumentException("Plan Harvest Cycle for this plant already exists", nameof(command.PlantId));
            }

            var planId = harvest.AddPlanHarvestCycle(command, userProfileId);

            _harvestCycleRepository.AddPlanHarvestCycle(planId, harvest);

            await _unitOfWork.SaveChangesAsync();

            return planId;
        }
        catch (Exception ex)
        {
            _logger.LogCritical("Exception adding plant grow instruction", ex);
            throw;
        }

    }

    public async Task<String> UpdatePlanHarvestCycle(UpdatePlanHarvestCycleCommand command)
    {
        _logger.LogInformation("Received request to create plan harvest cycle {@planHarvestCycle}", command);
        var harvest = await _harvestCycleRepository.GetByIdAsync(command.HarvestCycleId);

        if (harvest.Plans.Any(g => g.PlantId == command.PlantId && g.Id != command.PlanHarvestCycleId))
        {
            throw new ArgumentException("Plan Harvest Cyle for this plant already exists", nameof(command.PlantId));
        }

        harvest.UpdatePlanHarvestCycle(command);

        _harvestCycleRepository.UpdatePlanHarvestCycle(command.PlanHarvestCycleId, harvest);

        await _unitOfWork.SaveChangesAsync();

        return command.PlanHarvestCycleId;
    }

    public async Task<String> DeletePlanHarvestCycle(string harvestCycleId, string id)
    {
        _logger.LogInformation($"Received request to delete plan harvest cycle  {harvestCycleId} and {id}");

        var harvest = await _harvestCycleRepository.GetByIdAsync(harvestCycleId);

        harvest.DeletePlanHarvestCycle(id);

        _harvestCycleRepository.DeletePlanHarvestCycle(id, harvest);

        await _unitOfWork.SaveChangesAsync();

        return id;
    }

    #endregion
   
}

