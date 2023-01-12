using GardenLog.SharedKernel.Interfaces;
using MediatR;
using Microsoft.AspNetCore.WebUtilities;
using MongoDB.Driver;
using PlantCatalog.Contract.Commands;
using PlantCatalog.Domain.PlantAggregate;

namespace PlantCatalog.Api.CommandHandlers;

public interface IPlantCommandHandler
{
    Task<string> AddPlantGrowInstruction(CreatePlantGrowInstructionCommand command);
    Task<string> CreatePlant(CreatePlantCommand request);
    Task<string> DeletePlant(string id);
    Task<string> DeletePlantGrowInstruction(string plantId, string id);
    Task<string> UpdatePlant(UpdatePlantCommand request);
    Task<string> UpdatePlantGrowInstruction(UpdatePlantGrowInstructionCommand command);
}

public class PlantCommandHandler : IPlantCommandHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPlantRepository _plantRepository;
    private readonly ILogger<PlantCommandHandler> _logger;

    public PlantCommandHandler(IUnitOfWork unitOfWork, IPlantRepository repository, ILogger<PlantCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _plantRepository = repository;
        _logger = logger;
    }

    public async Task<string> CreatePlant(CreatePlantCommand request)
    {
        _logger.LogInformation("Received request to create a new plan {@plant}", request);

        var existingPlanId = await _plantRepository.GetIdByNameAsync(request.Name);
        if (!string.IsNullOrEmpty(existingPlanId))
        {
            throw new ArgumentException("Plant with this name already exists", nameof(request.Name));
        }

        var plant = Plant.Create(
            request.Name,
            request.Description,
            request.Color,
            request.Type,
            request.Lifecycle,
            request.MoistureRequirement,
            request.LightRequirement,
            request.GrowTolerance,
            request.GardenTip,
            request.SeedViableForYears,
            request.Tags,
            request.VarietyColors);

        _plantRepository.Add(plant);

        await _unitOfWork.SaveChangesAsync();

        return plant.Id;
    }

    public async Task<string> UpdatePlant(UpdatePlantCommand request)
    {
        _logger.LogInformation("Received request to update plant {@plant}", request);

        var existingPlanId = await _plantRepository.GetIdByNameAsync(request.Name);
        if (!string.IsNullOrEmpty(existingPlanId) && existingPlanId!=request.PlantId)
        {
            throw new ArgumentException("Another plant with this name already exists", nameof(request.Name));
        }

        var plant = await _plantRepository.GetByIdAsync(request.PlantId);

        plant.Update(request.Name,
            request.Description,
            request.Color,
            request.Type,
            request.Lifecycle,
            request.MoistureRequirement,
            request.LightRequirement,
            request.GrowTolerance,
            request.GardenTip,
            request.SeedViableForYears,
            request.Tags,
            request.VarietyColors);

        _plantRepository.Update(plant);

        await _unitOfWork.SaveChangesAsync();

        return plant.Id;
    }

    public async Task<string> DeletePlant(string id)
    {
        _logger.LogInformation("Received request to delete plant {@id}", id);


        _plantRepository.Delete(id);

        await _unitOfWork.SaveChangesAsync();

        return id;
    }

    public async Task<String> AddPlantGrowInstruction(CreatePlantGrowInstructionCommand command)
    {
        _logger.LogInformation("Received request to create plant grow instructions {@plantgrowinstruction}", command);
        try
        {
            var plant = await _plantRepository.GetByIdAsync(command.PlantId);

            if (plant.GrowInstructions.Any(g => g.Name == command.Name))
            {
                throw new ArgumentException("Grow Instruction with this name already exists", nameof(command.Name));
            }

            var growId = plant.AddPlantGrowInstruction(command);

            _plantRepository.AddPlantGrowInstruction(command.PlantId, plant.GrowInstructions.First(g => g.Id== growId), plant.GrowInstructionsCount);

            await _unitOfWork.SaveChangesAsync();

            return growId;
        }
        catch (Exception ex)
        {
            _logger.LogCritical("Exception adding plant grow instruction", ex);
            throw;
        }
       
    }

    public async Task<String> UpdatePlantGrowInstruction(UpdatePlantGrowInstructionCommand command)
    {
        _logger.LogInformation("Received request to create plant grow instructions {@plantgrowinstruction}", command);
        var plant = await _plantRepository.GetByIdAsync(command.PlantId);

        if (plant.GrowInstructions.Any(g => g.Name == command.Name && g.Id != command.PlantGrowInstructionId))
        {
            throw new ArgumentException("Grow Instruction with this name already exists", nameof(command.Name));
        }

        plant.UpdatePlantGrowInstructions(command);

        _plantRepository.UpdatePlantGrowInstruction(command.PlantId, plant.GrowInstructions.First(g => g.Id == command.PlantGrowInstructionId));

        await _unitOfWork.SaveChangesAsync();

        return command.PlantGrowInstructionId;
    }

    public async Task<String> DeletePlantGrowInstruction(string plantId, string id)
    {
        _logger.LogInformation($"Received request to delete plant grow instructions {plantId} and {id}");
        var plant = await _plantRepository.GetByIdAsync(plantId);

        plant.DeletePlantGrowInstruction(id);

        _plantRepository.DeletePlantGrowInstruction(plantId, id, plant.GrowInstructionsCount);

        await _unitOfWork.SaveChangesAsync();

        return id;
    }
}
