using AutoMapper;
using MediatR;
using PlantCatalog.Contract.ViewModels;
using PlantCatalog.Domain.PlantAggregate;
using PlantCatalog.Domain.PlantAggregate.Dto;
using SharpCompress.Common;

namespace PlantCatalog.Api.QueryHandlers;

public interface IPlantQueryHandler
{
    Task<IReadOnlyCollection<PlantViewModel>> GetAllPlants();
    Task<PlantViewModel> GetPlantByPlantId(string plantId);
    Task<PlantGrowInstructionViewModel> GetPlantGrowInstruction(string plantId, string id);
    Task<IReadOnlyCollection<PlantGrowInstructionViewModel>> GetPlantGrowInstructions(string plantId);
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

    #region Plant
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
    #endregion

    #region Plant Grow Instruction

    public async Task<IReadOnlyCollection<PlantGrowInstructionViewModel>> GetPlantGrowInstructions(string plantId)
    {
        _logger.LogInformation($"Received request to get plant grow instructions for {plantId}");
        List<PlantGrowInstructionViewModel> response = new();

        try
        {
            var data = await _plantRepository.GetPlantGrowInstractions(plantId);
            foreach (var entry in data)
            {
                var grow = _mapper.Map<PlantGrowInstructionViewModel>(entry);
                grow.PlantId = plantId;
                response.Add(grow);
            }
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"Exception readding grow instructions for {plantId}", ex);
            throw;
        }
      
    }

    public async Task<PlantGrowInstructionViewModel> GetPlantGrowInstruction(string plantId, string id)
    {
        _logger.LogInformation($"Received request to get plant grow instruction for {plantId} and {id}");

        var data =  await _plantRepository.GetPlantGrowInstraction(plantId, id);

        var grow = _mapper.Map<PlantGrowInstructionViewModel>(data);
        grow.PlantId = plantId;

        return grow;
    }

    #endregion
}
