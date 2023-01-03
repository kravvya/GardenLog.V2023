using PlantCatalog.Contract.ViewModels;
using PlantCatalog.Domain.PlantAggregate;

namespace PlantCatalog.Api.QueryHandlers;

public interface IPlantQueryHandler
{
    Task<IReadOnlyCollection<PlantViewModel>> GetAllPlants();
}

public class PlantQueryHandler : IPlantQueryHandler
{
    private readonly IPlantRepository _plantRepository;
    private readonly ILogger<PlantQueryHandler> _logger;

    public PlantQueryHandler(IPlantRepository repository, ILogger<PlantQueryHandler> logger)
    {
        _plantRepository = repository;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<PlantViewModel>> GetAllPlants()
    {
        _logger.LogInformation("Received request to get all plants");

        return await _plantRepository.GetAllPlants();
    }

}
