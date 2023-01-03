using PlantCatalog.Contract.Commands;
using PlantCatalog.Domain.PlantAggregate;

namespace PlantCatalog.Api.CommandHandlers;

public interface IPlantCommandHandler
{
    Task<string> CreatePlant(CreatePlantCommand request);
    Task<string> UpdatePlant(UpdatePlantCommand request);
}

public class PlantCommandHandler : IPlantCommandHandler
{
    private readonly IPlantRepository _plantRepository;
    private readonly ILogger<PlantCommandHandler> _logger;

    public PlantCommandHandler(IPlantRepository repository, ILogger<PlantCommandHandler> logger)
    {
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
            request.SeedViableForYears);

        _plantRepository.Add(plant);

        await _plantRepository.SaveChangesAsync();

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
            request.SeedViableForYears);

        _plantRepository.Update(plant);

        await _plantRepository.SaveChangesAsync();

        return plant.Id;
    }
}
