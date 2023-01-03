using PlantCatalog.Contract.ViewModels;

namespace PlantCatalog.Domain.PlantAggregate;

public interface IPlantRepository : IRepository<Plant>
{
    Task<IReadOnlyCollection<PlantViewModel>> GetAllPlants();
    Task<Plant> GetByNameAsync(string plantName);
    Task<string> GetIdByNameAsync(string plantName);
}
