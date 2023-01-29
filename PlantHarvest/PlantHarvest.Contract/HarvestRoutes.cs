﻿namespace PlantCatalog.Contract;

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
    public const string GetPlantHarvestCyclesByPlant = PlantHarvestBase + "/PlanHarvestCycles/Plants/{plantId}/";
   

    public const string WorkLogBase = "/v1/api/WorkLog";
    public const string CreateWorkLog = WorkLogBase;
    public const string UpdateWorkLog = WorkLogBase + "/{id}";
    public const string DeleteWorkLog = WorkLogBase + "/{id}";

    public const string GetWorkLogs = WorkLogBase + "/{entityType}/{entityId}";
}