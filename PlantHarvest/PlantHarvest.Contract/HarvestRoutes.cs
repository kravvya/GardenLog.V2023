namespace PlantCatalog.Contract;

public static class HarvestRoutes
{
    public const string PlantHarvestBase = "/v1/api/Harvest";
    public const string CreateHarvestCycle = PlantHarvestBase;
    public const string GetAllHarvestCycles = PlantHarvestBase;
    public const string GetHarvestCycleById = PlantHarvestBase + "/{id}";
    public const string GetIdByHarvestCycleName = PlantHarvestBase + "/name/{name}";
    public const string UpdateHarvestCycle = PlantHarvestBase + "/{id}";
    public const string DeleteHarvestCycle= PlantHarvestBase + "/{id}";

    public const string GetPlantHarvestCycles = PlantHarvestBase + "/{harvestId}/PlantHarvestCycles";
    public const string GetPlantHarvestCycle = PlantHarvestBase + "/{harvestId}/PlantHarvestCycles/{id}";
    public const string CreatePlantHarvestCycle = PlantHarvestBase + "/{harvestId}/PlantHarvestCycles";
    public const string UpdatePlantHarvestCycle = PlantHarvestBase + "/{harvestId}/PlantHarvestCycles/{id}";
    public const string DeletePlantHarvestCycle = PlantHarvestBase + "/{harvestId}/PlantHarvestCycles/{id}";

    public const string GetAllPlanHarvestCycles = PlantHarvestBase + "/PlanHarvestCycles";
    public const string GetPlanHarvestCycles = PlantHarvestBase + "/{harvestId}/PlanHarvestCycles";
    public const string GetPlantHarvestCyclesByPlant = PlantHarvestBase + "/PlanHarvestCycles/Plants/{plantId}/";
    public const string GetPlanHarvestCycle = PlantHarvestBase + "/{harvestId}/PlanHarvestCycles/{id}";
    public const string CreatePlanHarvestCycle = PlantHarvestBase + "/{harvestId}/PlanHarvestCycles";
    public const string UpdatePlanHarvestCycle = PlantHarvestBase + "/{harvestId}/PlanHarvestCycles/{id}";
    public const string DeletePlanHarvestCycle = PlantHarvestBase + "/{harvestId}/PlanHarvestCycles/{id}";
}
