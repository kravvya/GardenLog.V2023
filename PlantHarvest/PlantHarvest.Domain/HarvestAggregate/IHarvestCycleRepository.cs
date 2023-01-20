namespace PlantHarvest.Domain.HarvestAggregate;


public interface IHarvestCycleRepository : IRepository<HarvestCycle>
{
    void AddPlantHarvestCycle(string plantHarvestCyclceId, HarvestCycle harvestCyclce);
    void DeletePlantHarvestCycle(string plantHarvestCyclceId, HarvestCycle harvestCyclce);
    void UpdatePlantHarvestCycle(string plantHarvestCyclceId, HarvestCycle harvestCyclce);
    Task<PlantHarvestCycleViewModel> GetPlantHarvestCycle(string harvestCycleId, string id);
    Task<IReadOnlyCollection<PlantHarvestCycleViewModel>> GetPlantHarvestCycles(string harvestCycleId);

    void AddPlanHarvestCycle(string planHarvestCyclceId, HarvestCycle harvestCyclce);
    void DeletePlanHarvestCycle(string planHarvestCyclceId, HarvestCycle harvestCyclce);
    void UpdatePlanHarvestCycle(string planHarvestCyclceId, HarvestCycle harvestCyclce);
    Task<PlanHarvestCycleViewModel> GetPlanHarvestCycle(string harvestCycleId, string id);
    Task<IReadOnlyCollection<PlanHarvestCycleViewModel>> GetPlanHarvestCycles(string harvestCycleId);
    
    Task<String> GetIdByNameAsync(string harvestCycleName, string userProfileId);
    Task<IReadOnlyCollection<HarvestCycleViewModel>> GetAllHarvestCycles(string userProfileId);
}
