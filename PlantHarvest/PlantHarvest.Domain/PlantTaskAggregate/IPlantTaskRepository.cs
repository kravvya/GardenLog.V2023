

namespace PlantHarvest.Domain.PlantTaskAggregate;

public interface IPlantTaskRepository : IRepository<PlantTask>
{
    Task<IReadOnlyCollection<PlantTaskViewModel>> GetPlantTasksForUser(string userProfileId);
    Task<IReadOnlyCollection<PlantTaskViewModel>> GetActivePlantTasksForUser(string userProfileId);
}
