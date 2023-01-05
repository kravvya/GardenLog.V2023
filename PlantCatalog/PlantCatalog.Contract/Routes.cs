namespace PlantCatalog.Contract;

public static class Routes
{
    public const string PlantCatalogBase = "/v1/api/Plants";
    public const string CreatePlant = PlantCatalogBase;
    public const string GetAllPlants = PlantCatalogBase;
    public const string GetPlantById = PlantCatalogBase + "/{id}";
    public const string UpdatePlant = PlantCatalogBase + "/{id}";
    public const string DeletePlant = PlantCatalogBase + "/{id}";

}
