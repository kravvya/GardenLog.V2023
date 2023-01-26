using PlantHarvest.Contract.ViewModels;
using PlantHarvest.IntegrationTest.Clients;
using PlantHarvest.IntegrationTest.Fixture;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit.Abstractions;

namespace PlantHarvest.IntegrationTest;

 public partial class PlantHarvestTests : IClassFixture<PlantHarvestServiceFixture>
{
    private readonly ITestOutputHelper _output;
    private readonly PlantHarvestClient _plantHarvestClient;
    private readonly WorkLogClient _workLogClient;

    public const string TEST_HARVEST_CYCLE_NAME = "Test Harvest Cycle";
    private const string TEST_PLANT_ID = "Fake-Plant-Id";
    private const string TEST_PLANT_VARIETY_ID = "Fake-Plant-Variety-Id";

    private const string TEST_DELETE_PLANT_ID = "Delete-Fake-Plant-Id";
    private const string TEST_DELETE_VARIETY_ID = "Delete-Fake-Variety-Id";
    private const string TEST_DELETE_HARVEST_CYCLE_NAME = "Delete Harvest Cycle";

    public PlantHarvestTests(PlantHarvestServiceFixture fixture, ITestOutputHelper output)
    {
        _plantHarvestClient = fixture.PlantHarvestClient;
        _workLogClient=fixture.WorkLogClient;
        _output = output;
        _output.WriteLine($"Service id {fixture.FixtureId} @ {DateTime.Now.ToString("F")}");
    }

    #region Harvest Cycle
    [Fact]
    public async Task Post_HarvestCycle_MayCreateNewHarvestCycle()
    {
        var response = await _plantHarvestClient.CreateHarvestCycle(TEST_HARVEST_CYCLE_NAME);

        var returnString = await response.Content.ReadAsStringAsync();

        _output.WriteLine($"Service to create harvest cycle responded with {response.StatusCode} code and {returnString} message");

        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            Assert.NotEmpty(returnString);
            Assert.True(Guid.TryParse(returnString, out var harvestCycleId));
        }
        else
        {
            Assert.True(response.StatusCode == System.Net.HttpStatusCode.BadRequest);
            Assert.NotEmpty(returnString);
            Assert.Contains("Garden Plan with this name already exists", returnString);
        }
    }

    [Fact]
    public async Task Post_HarvestCycle_ShouldNotCreateNewHarvestCycle_WithoutName()
    {
        var response = await _plantHarvestClient.CreateHarvestCycle("");

        var returnString = await response.Content.ReadAsStringAsync();

        _output.WriteLine($"Service responded with {response.StatusCode} code and {returnString} message");

        Assert.True(response.StatusCode == System.Net.HttpStatusCode.BadRequest);
        Assert.NotEmpty(returnString);
        Assert.Contains("'Harvest Cycle Name' must not be empty.", returnString);
    }

    [Fact]
    public async Task Put_HarvestCycle_ShouldUpdateHarvestCycle()
    {
        var harvest = await GetHarvestCycleToWorkWith(TEST_HARVEST_CYCLE_NAME);

        harvest.Notes = $"{harvest.Notes} last pdated: {DateTime.Now.ToString()}";

        var response = await _plantHarvestClient.UpdateHarvestCycle(harvest);

        var returnString = await response.Content.ReadAsStringAsync();

        _output.WriteLine($"Service to update harvest cycle responded with {response.StatusCode} code and {returnString} message");

        Assert.True(response.StatusCode == System.Net.HttpStatusCode.OK);
        Assert.NotEmpty(returnString);
    }

    [Fact]
    public async Task Delete_HarvestCycle_ShouldDelete()
    {
        var harvestId = await _plantHarvestClient.GetHarvestCycleIdToWorkWith(TEST_DELETE_HARVEST_CYCLE_NAME);

        var response = await _plantHarvestClient.DeleteHarvestCycle(harvestId);

        var returnString = await response.Content.ReadAsStringAsync();

        _output.WriteLine($"Service to delete harvest cycle responded with {response.StatusCode} code and {returnString} message");

        Assert.True(response.StatusCode == System.Net.HttpStatusCode.OK);
        Assert.NotEmpty(returnString);
    }

    [Fact]
    public async Task Get_Should_Return_All_HarvestCycles()
    {
        var response = await _plantHarvestClient.GetAllHarvestCycles();

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
                {
                    new JsonStringEnumConverter(),
                },
        };

        var returnString = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Service for all harvest cycles responded with {response.StatusCode} code and {returnString} message");

        var harvests = await response.Content.ReadFromJsonAsync<List<HarvestCycleViewModel>>(options);

        Assert.NotNull(harvests);
        Assert.NotEmpty(harvests);

        var testHarvest = harvests.FirstOrDefault(plants => plants.HarvestCycleName == TEST_HARVEST_CYCLE_NAME);

        Assert.NotNull(testHarvest);
    }

    [Fact]
    public async Task Get_Should_Return_HarvestCycle()
    {
        var plant = await GetHarvestCycleToWorkWith(TEST_HARVEST_CYCLE_NAME);

        Assert.NotNull(plant);
        Assert.Equal(plant.HarvestCycleName, TEST_HARVEST_CYCLE_NAME);
    }

    #endregion

    #region Plant Harevst Cycle
    [Fact]
    public async Task Post_PlantHarvestCycle_MayCreateNew()
    {
        var harvestId = await _plantHarvestClient.GetHarvestCycleIdToWorkWith(TEST_HARVEST_CYCLE_NAME);

        var original = await GetPlantHarvestCycleToWorkWith(harvestId, TEST_PLANT_ID, TEST_PLANT_VARIETY_ID);
                
        if (original != null)
        {
            var response = await _plantHarvestClient.DeletePlantHarvestCycle(original.PlantId, original.PlantHarvestCycleId);
        }

        var harvestCycleId = await CreatePlantHarvestCycleToWorkWith(harvestId, TEST_PLANT_ID, TEST_PLANT_VARIETY_ID);
        Assert.NotNull(harvestCycleId);
    }

    
    [Fact]
    public async Task Get_PlantHarvestCycle_ByHarvestId()
    {
        var harvestId = await _plantHarvestClient.GetHarvestCycleIdToWorkWith(TEST_HARVEST_CYCLE_NAME);
        var plant = await GetPlantHarvestCycleToWorkWith(harvestId, TEST_PLANT_ID, TEST_PLANT_VARIETY_ID);

        Assert.NotNull(plant);
        _output.WriteLine($"Found '{plant.PlantVarietyId}' Plant Harvest Cycle");
        Assert.NotEmpty(plant.PlantVarietyId);
    }

    [Fact]
    public async Task Get_PlantHarvestCycle_One()
    {
        var harvestId = await _plantHarvestClient.GetHarvestCycleIdToWorkWith(TEST_HARVEST_CYCLE_NAME);
        var plants = await GetPlantHarvestCyclesToWorkWith(harvestId,  TEST_PLANT_ID, TEST_PLANT_VARIETY_ID);

        var original = plants.First(g => g.PlantVarietyId == TEST_PLANT_VARIETY_ID);

        if (plants.Count == 1)
        {
            //create new HarvestCycle to make sure we only get one back
            var response = await _plantHarvestClient.CreatePlantHarvestCycle(harvestId,  TEST_PLANT_ID, TEST_PLANT_VARIETY_ID);
        }

        var plant = await GetPlantHarvestCycleToWorkWith(harvestId, original.PlantHarvestCycleId);

        _output.WriteLine($"Found '{plant.PlantVarietyId}' Plant Harvest Cycle");
        Assert.Equal(original.PlantVarietyId, plant.PlantVarietyId);
    }

    [Fact]
    public async Task Get_PlantHarvestCycles_ByPlantId()
    {
        var harvestId = await _plantHarvestClient.GetHarvestCycleIdToWorkWith(TEST_HARVEST_CYCLE_NAME);
        var plant = await GetPlantHarvestCycleToWorkWith(harvestId, TEST_PLANT_ID, TEST_PLANT_VARIETY_ID);

        var searchResponse = await _plantHarvestClient.GetPlantHarvestCyclesByPlantId(TEST_PLANT_ID);

        Assert.NotNull(searchResponse);
        Assert.True(searchResponse.StatusCode == System.Net.HttpStatusCode.OK);

        var returnString = await searchResponse.Content.ReadAsStringAsync();
              
        _output.WriteLine($"Get_PlantHarvestCycles_ByPlantId - Found '{returnString}' by searching by plant Id");
        Assert.Contains(plant.PlantHarvestCycleId, returnString);
    }

    [Fact]
    public async Task Put_PlantHarvestCycle_ShouldUpdate()
    {
        var harvestId = await _plantHarvestClient.GetHarvestCycleIdToWorkWith(TEST_HARVEST_CYCLE_NAME);
        var plant = await GetPlantHarvestCycleToWorkWith(harvestId, TEST_PLANT_ID, TEST_PLANT_VARIETY_ID);

      
        plant.NumberOfSeeds += 1;

        var response = await _plantHarvestClient.UpdatePlantHarvestCycle(plant);

        var returnString = await response.Content.ReadAsStringAsync();

        _output.WriteLine($"Service to update Plant Harvest Cycle responded with {response.StatusCode} code and {returnString} message");

        Assert.True(response.StatusCode == System.Net.HttpStatusCode.OK);
        Assert.NotEmpty(returnString);
    }

    [Fact]
    public async Task Delete_PlantHarvestCycle_ShouldDelete()
    {
        var harvestId = await _plantHarvestClient.GetHarvestCycleIdToWorkWith(TEST_DELETE_HARVEST_CYCLE_NAME);

        var plant = await GetPlantHarvestCycleToWorkWith(harvestId, TEST_DELETE_PLANT_ID, TEST_DELETE_VARIETY_ID);
        

        var response = await _plantHarvestClient.DeletePlantHarvestCycle(plant.HarvestCycleId, plant.PlantHarvestCycleId);

        var returnString = await response.Content.ReadAsStringAsync();

        _output.WriteLine($"Service to delete plant HarvestCycle responded with {response.StatusCode} code and {returnString} message");

        Assert.True(response.StatusCode == System.Net.HttpStatusCode.OK);
        Assert.NotEmpty(returnString);
    }
    #endregion

    #region Shared Private Functions
   

    private async Task<HarvestCycleViewModel> GetHarvestCycleToWorkWith(string harvestName)
    {

        var harvestId = await _plantHarvestClient.GetHarvestCycleIdToWorkWith(harvestName);

        var response = await _plantHarvestClient.GetHarvestCycle(harvestId);
        var returnString = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Service responded with {response.StatusCode} code and {returnString} message");

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
                {
                    new JsonStringEnumConverter(),
                },
        };
        var harvest = await response.Content.ReadFromJsonAsync<HarvestCycleViewModel>(options);

        return harvest;
    }
       
    private async Task<PlantHarvestCycleViewModel> GetPlantHarvestCycleToWorkWith(string harvestId, string plantId, string plantVarietyId)
    {
        var response = await _plantHarvestClient.GetPlantHarvestCycles(harvestId);

        _output.WriteLine($"Service to get all plant harvest cycle responded with {response.StatusCode} code");

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
                {
                    new JsonStringEnumConverter(),
                },
        };
        var plants = await response.Content.ReadFromJsonAsync<List<PlantHarvestCycleViewModel>>(options);
        PlantHarvestCycleViewModel plant = null;

        if (plants == null || plants.Count == 0)
        {
            var plantHarvestCycleId  = await CreatePlantHarvestCycleToWorkWith(harvestId, plantId, plantVarietyId);

            response = await _plantHarvestClient.GetPlantHarvestCycle(harvestId, plantHarvestCycleId);

            _output.WriteLine($"Service to getplant harvest cycle responded with {response.StatusCode} code");

            plant = await response.Content.ReadFromJsonAsync<PlantHarvestCycleViewModel>(options);
        }
        else
        {
            plant = plants.First(p => p.PlantVarietyId == plantVarietyId);
        }

        return plant;
    }

    private async Task<List<PlantHarvestCycleViewModel>> GetPlantHarvestCyclesToWorkWith(string harvestId, string plantId, string plantVarietyId)
    {
        var response= await _plantHarvestClient.GetPlantHarvestCycles(harvestId);
        _output.WriteLine($"Service to get all plant harvest cycles responded with {response.StatusCode} code");

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
                {
                    new JsonStringEnumConverter(),
                },
        };
        var plants = await response.Content.ReadFromJsonAsync<List<PlantHarvestCycleViewModel>>(options);

        if (plants == null || plants.Count == 0)
        {
            await CreatePlantHarvestCycleToWorkWith(harvestId, plantId, plantVarietyId);

            response = await _plantHarvestClient.GetPlantHarvestCycles(harvestId);

            _output.WriteLine($"Service to get all plan harvest cycles responded with {response.StatusCode} code");

            plants = await response.Content.ReadFromJsonAsync<List<PlantHarvestCycleViewModel>>(options);
        }

        return plants;

    }

    private async Task<PlantHarvestCycleViewModel> GetPlantHarvestCycleToWorkWith(string harvestId, string id)
    {
        var response = await _plantHarvestClient.GetPlantHarvestCycle(harvestId, id);

        _output.WriteLine($"Service to get single plant harvest cycle responded with {response.StatusCode} code");

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
                {
                    new JsonStringEnumConverter(),
                },
        };
        var plant = await response.Content.ReadFromJsonAsync<PlantHarvestCycleViewModel>(options);

        Assert.NotNull(plant);

        return plant;
    }

    private async Task<string> CreatePlantHarvestCycleToWorkWith(string harvestId, string plantId, string plantVarietyId)
    {
        var response = await _plantHarvestClient.CreatePlantHarvestCycle(harvestId, plantId, plantVarietyId);
        var returnString = await response.Content.ReadAsStringAsync();

        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            Assert.NotEmpty(returnString);
            Assert.True(Guid.TryParse(returnString, out var plantHarvestCycleId));
        }
        else
        {
            Assert.True(response.StatusCode == System.Net.HttpStatusCode.BadRequest);
            returnString = await response.Content.ReadAsStringAsync();
            Assert.NotEmpty(returnString);
            Assert.Contains("This plant is already a part of this plan", returnString);
        }
        return returnString;
    }

    #endregion
}
