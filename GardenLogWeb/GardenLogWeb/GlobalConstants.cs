global using GardenLogWeb.Models;
global using GardenLogWeb.Models.Harvest;
global using GardenLogWeb.Models.Plants;
global using GardenLogWeb.Models.Work;
global using GardenLogWeb.Services;
global using GardenLogWeb.Shared;
global using GardenLogWeb.Shared.Extensions;
global using GardenLogWeb.Shared.Services;
global using ImageCatalog.Contract.Enum;
global using ImageCatalog.Contract.ViewModels;
global using PlantCatalog.Contract;
global using PlantCatalog.Contract.Enum;
global using PlantCatalog.Contract.ViewModels;
global using PlantHarvest.Contract.Enum;
global using PlantHarvest.Contract.ViewModels;
global using System.Text.Json;
global using UserManagement.Contract.ViewModels;
global using GardenLogWeb.Models.UserProfile;
global using UserManagement.Contract;
global using PlantHarvest.Contract;

namespace GardenLogWeb;

public static class GlobalConstants
{
    public const string PLANTCATALOG_API = "PlantCatalog.Api";
    public const string PLANTHARVEST_API = "PlantHarvest.Api";
    public const string IMAGEPLANTCATALOG_API = "ImageCatalog.Api";
    public const string USERMANAGEMENT_API = "UserManagement.Api";

    public const string ModalFormColor = "#A0C136";
}

