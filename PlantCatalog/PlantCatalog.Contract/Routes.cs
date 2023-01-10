﻿namespace PlantCatalog.Contract;

public static class Routes
{
    public const string PlantCatalogBase = "/v1/api/Plants";
    public const string CreatePlant = PlantCatalogBase;
    public const string GetAllPlants = PlantCatalogBase;
    public const string GetPlantById = PlantCatalogBase + "/{id}";
    public const string GetIdByPlantName = PlantCatalogBase + "/name/{name}";
    public const string UpdatePlant = PlantCatalogBase + "/{id}";
    public const string DeletePlant = PlantCatalogBase + "/{id}";

    public const string GetPlantGrowInstructions = PlantCatalogBase + "/{plantId}/GrowInstructions";
    public const string GetPlantGrowInstruction = PlantCatalogBase + "/{plantId}/GrowInstructions/{id}";
    public const string CreatePlantGrowInstruction = PlantCatalogBase + "/{plantId}/GrowInstructions";
    public const string UpdatePlantGrowInstructions = PlantCatalogBase + "/{plantId}/GrowInstructions/{id}";
    public const string DeletePlantGrowInstructions = PlantCatalogBase + "/{plantId}/GrowInstructions/{id}";
}
