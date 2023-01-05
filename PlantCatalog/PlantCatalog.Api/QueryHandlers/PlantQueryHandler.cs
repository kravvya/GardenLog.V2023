using AutoMapper;
using MediatR;
using PlantCatalog.Contract.ViewModels;
using PlantCatalog.Domain.PlantAggregate;
using PlantCatalog.Domain.PlantAggregate.Dto;

namespace PlantCatalog.Api.QueryHandlers;

public interface IPlantQueryHandler
{
    Task<IReadOnlyCollection<PlantViewModel>> GetAllPlants();
    Task<PlantViewModel> GetPlantByPlantId(string plantId);
    Task<string> GetPlantIdByPlantName(string nane);
}

public class PlantQueryHandler : IPlantQueryHandler
{
    private readonly IPlantRepository _plantRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<PlantQueryHandler> _logger;

    public PlantQueryHandler(IPlantRepository repository, IMapper mapper, ILogger<PlantQueryHandler> logger)
    {
        _plantRepository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PlantViewModel> GetPlantByPlantId(string plantId)
    {
        _logger.LogInformation($"Received request to get plant by plantid: {plantId}");
        var plant = await _plantRepository.GetByIdAsync(plantId);

        return _mapper.Map<PlantViewModel>(plant);
    }

    public async Task<string> GetPlantIdByPlantName(string nane)
    {
        _logger.LogInformation($"Received request to get plant id by plant name: {nane}");
        return await _plantRepository.GetIdByNameAsync(nane);
    }

    public async Task<IReadOnlyCollection<PlantViewModel>> GetAllPlants()
    {
        _logger.LogInformation("Received request to get all plants");

        return await _plantRepository.GetAllPlants();
    }

}
