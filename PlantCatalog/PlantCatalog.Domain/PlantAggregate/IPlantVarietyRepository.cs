using PlantCatalog.Contract.ViewModels;

namespace PlantCatalog.Domain.PlantAggregate;

public interface IPlantVarietyRepository : IRepository<PlantVariety>
{
    Task<IReadOnlyCollection<PlantVarietyViewModel>> GetAllPlantVarieties(string plantId);
    Task<PlantVariety> GetByNameAsync(string plantId, string plantName);
    Task<string> GetIdByNameAsync(string plantId, string plantName);
}
