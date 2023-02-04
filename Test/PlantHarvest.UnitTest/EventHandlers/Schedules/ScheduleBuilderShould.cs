using Castle.Core.Logging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using PlantCatalog.Contract.ViewModels;
using PlantHarvest.Api.Schedules;
using PlantHarvest.Contract.Enum;
using PlantHarvest.Infrastructure.ApiClients;
using System.Net;

namespace PlantHarvest.UnitTest.EventHandlers.Schedules;

public class ScheduleBuilderShould
{
    IConfiguration _configuration;
    IMemoryCache _memoryCache;
    HttpClient _httpPlantCatalogClient;
    HttpClient _httpUserManagementClient;
    ILogger<PlantCatalogApiClient> _loggerPlantCatalogClient;
    ILogger<UserManagementApiClient> _loggerUserManagementClient;

    IPlantCatalogApiClient _apiClinet;
    IUserManagementApiClient _userManagementApiClient;


    public ScheduleBuilderShould()
    {
        var configMock = new Mock<IConfiguration>();
        configMock.SetupGet(x => x[It.Is<string>(s => s == "Services:PlantCatalog.Api")]).Returns(HttpClientsHelper.PLANT_CATALOG_URL);
        configMock.SetupGet(x => x[It.Is<string>(s => s == "Services:UserManagement.Api")]).Returns(HttpClientsHelper.USER_MANAGEMENT_URL);
        _configuration = configMock.Object;

       _memoryCache = new MemoryCache(new MemoryCacheOptions());

        _httpPlantCatalogClient = HttpClientsHelper.GetPlantCatalogHttpClientForGrowInstructions(PlantCatalog.Contract.Enum.PlantingMethodEnum.SeedIndoors);
        _httpUserManagementClient = HttpClientsHelper.GetUserManagementHttpClientForGarden();
        _loggerPlantCatalogClient = new Mock<ILogger<PlantCatalogApiClient>>().Object;
        _loggerUserManagementClient = new Mock<ILogger<UserManagementApiClient>>().Object;

        _apiClinet = new PlantCatalogApiClient(_httpPlantCatalogClient, _configuration, _loggerPlantCatalogClient, _memoryCache);
        _userManagementApiClient = new UserManagementApiClient(_httpUserManagementClient, _configuration, _loggerUserManagementClient, _memoryCache);
    }

    [Fact]
    public async Task Builder_CreatesIndoorSowSchduleAsync()
    {
        var builder = new ScheduleBuilder(_apiClinet, _userManagementApiClient);

        var schedues = await builder.GeneratePlantCalendarBasedOnGrowInstruction(PlantsHelper.PLANT_ID, PlantsHelper.GROW_INSTRUCTION_ID, PlantsHelper.PLANT_VARIETY_ID,UserManagementHelper.GARDEN_ID);

        Assert.NotNull(schedues);
        Assert.NotEmpty(schedues);
        Assert.Contains(schedues, schedule => schedule.TaskType == WorkLogReasonEnum.SowIndoors);
        Assert.Contains(schedues, schedule => schedule.TaskType == WorkLogReasonEnum.TransplantOutside);
        Assert.Contains(schedues, schedule => schedule.TaskType == WorkLogReasonEnum.Harvest);
    }
}
