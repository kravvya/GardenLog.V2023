global using GardenLogWeb.Models.Images;
global using GardenLogWeb.Models.PlantGrowInstructions;
global using GardenLogWeb.Models.Plants;
global using GardenLogWeb.Models.PlantVariety;
global using GardenLogWeb.Services;
global using GardenLogWeb.Shared;
global using GardenLogWeb.Shared.Extensions;
global using GardenLogWeb.Shared.Services;
global using PlantCatalog.Contract;
global using PlantCatalog.Contract.Enum;
global using PlantCatalog.Contract.ViewModels;
global using System.Text.Json;
global using Blazored.FluentValidation;

namespace GardenLogWeb;

public static class GlobalConstants
{
    public const string PLANTCATALOG_API = "PlantCatalog.Api";

}

