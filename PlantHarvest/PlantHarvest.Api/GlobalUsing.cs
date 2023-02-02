global using PlantHarvest.Contract.Commands;
global using PlantHarvest.Domain.HarvestAggregate;
global using PlantHarvest.Contract.ViewModels;
global using PlantHarvest.Api.CommandHandlers;
global using PlantHarvest.Api.QueryHandlers;
global using GardenLog.SharedInfrastructure.Extensions;
global using PlantHarvest.Orchestrator.Schedules;
global using PlantHarvest.Domain.PlantTaskAggregate;
global using PlantHarvest.Domain.WorkLogAggregate;
global using PlantHarvest.Contract;

public static class GlobalConstants
{
    public const string PLANTCATALOG_API = "PlantCatalog.Api";
}