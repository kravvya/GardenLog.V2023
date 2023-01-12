using PlantCatalog.Contract.ViewModels;

namespace PlantCatalog.Domain.PlantAggregate;

public interface IPlantRepository : IRepository<Plant>
{
    void AddPlantGrowInstruction(string plantId, PlantGrowInstruction growInstruction, int growInstructionsCount);
    void DeletePlantGrowInstruction(string plantId, string id, int growInstructionsCount);
    Task<bool> ExistsAsync(string plantId);
    Task<IReadOnlyCollection<PlantViewModel>> GetAllPlants();
    Task<Plant> GetByNameAsync(string plantName);
    Task<string> GetIdByNameAsync(string plantName);

    Task<PlantGrowInstructionViewModel> GetPlantGrowInstraction(string plantId, string id);
    Task<IReadOnlyCollection<PlantGrowInstructionViewModel>> GetPlantGrowInstractions(string plantId);
    void UpdatePlantGrowInstruction(string plantId, PlantGrowInstruction growInstruction);
}
