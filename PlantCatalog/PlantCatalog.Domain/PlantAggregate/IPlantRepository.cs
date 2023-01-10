using PlantCatalog.Contract.ViewModels;

namespace PlantCatalog.Domain.PlantAggregate;

public interface IPlantRepository : IRepository<Plant>
{
    void AddPlantGrowInstruction(string plantId, PlantGrowInstruction growInstruction);
    void DeletePlantGrowInstruction(string plantId, PlantGrowInstruction growInstruction);
    Task<IReadOnlyCollection<PlantViewModel>> GetAllPlants();
    Task<Plant> GetByNameAsync(string plantName);
    Task<string> GetIdByNameAsync(string plantName);

    Task<PlantGrowInstruction> GetPlantGrowInstraction(string plantId, string id);
    Task<IReadOnlyCollection<PlantGrowInstruction>> GetPlantGrowInstractions(string plantId);
    void UpdatePlantGrowInstruction(string plantId, PlantGrowInstruction growInstruction);
}
