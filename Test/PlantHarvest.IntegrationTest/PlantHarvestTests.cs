using PlantHarvest.Contract.ViewModels;
using PlantHarvest.IntegrationTest.Clients;
using PlantHarvest.IntegrationTest.Fixture;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit.Abstractions;

namespace PlantHarvest.IntegrationTest;

public class PlantHarvestTests : IClassFixture<PlantHarvestServiceFixture>
{
    private readonly ITestOutputHelper _output;
    private readonly PlantHarvestClient _plantHarvestClient;

    private const string TEST_HARVEST_CYCLE_NAME = "Test Harvest Cycle";
    private const string TEST_PLANT_ID = "Fake-Plant-Id";
    private const string TEST_PLANT_VARIETY_ID = "Fake-Plant-Variety-Id";

    private const string TEST_DELETE_PLANT_ID = "Delete-Fake-Plant-Id";
    private const string TEST_DELETE_VARIETY_ID = "Delete-Fake-Variety-Id";
    private const string TEST_DELETE_HARVEST_CYCLE_NAME = "Delete Harvest Cycle";

    public PlantHarvestTests(PlantHarvestServiceFixture fixture, ITestOutputHelper output)
    {
        _plantHarvestClient = fixture.PlantHarvestClient;
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
        var harvestId = await GetHarvestCycleIdToWorkWith(TEST_DELETE_HARVEST_CYCLE_NAME);

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

    #region Plan Harvest Cycle

    [Fact]
    public async Task Post_PlanHarvestCycle_MayCreateNew()
    {
        var harvestId = await GetHarvestCycleIdToWorkWith(TEST_HARVEST_CYCLE_NAME);

        var response = await _plantHarvestClient.CreatePlanHarvestCycle(harvestId, TEST_PLANT_ID);
        var returnString = await response.Content.ReadAsStringAsync();

        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            Assert.NotEmpty(returnString);
            Assert.True(Guid.TryParse(returnString, out var planHarvestCycleId));
        }
        else
        {
            Assert.True(response.StatusCode == System.Net.HttpStatusCode.BadRequest);
            returnString = await response.Content.ReadAsStringAsync();
            Assert.NotEmpty(returnString);
            Assert.Contains("Plan Harvest Cycle for this plant already exists", returnString);
        }
    }

    [Fact]
    public async Task Get_PlanHarvestCycle_All()
    {
        var plans = await GetPlanHarvestCyclesToWorkWith(TEST_HARVEST_CYCLE_NAME, TEST_PLANT_ID);

        Assert.NotNull(plans);
        _output.WriteLine($"Found '{plans.First().PlantId}' plant in plan haver cycles");
        Assert.NotEmpty(plans.First().PlanHarvestCycleId);
    }

    [Fact]
    public async Task Get_PlanHarvestCycle_One()
    {
        var plans = await GetPlanHarvestCyclesToWorkWith(TEST_HARVEST_CYCLE_NAME, TEST_PLANT_ID);

        var original = plans.First(g => g.PlantId == TEST_PLANT_ID);

        if (plans.Count == 1)
        {
            //create new plan harvest cycle to make sure we only get one back
            var response = await _plantHarvestClient.CreatePlanHarvestCycle(original.HarvestCycleId, TEST_DELETE_PLANT_ID);
        }

        var plan = await GetPlanHarvestCycleToWorkWith(original.HarvestCycleId,original.PlanHarvestCycleId);

        _output.WriteLine($"Found '{plan.PlantId}' plant in plan harvest cycle");
        Assert.Equal(original.PlantId, plan.PlantId);
    }

    [Fact]
    public async Task Put_PlanHarvestCycle_ShouldUpdate()
    {
        var plan = (await GetPlanHarvestCyclesToWorkWith(TEST_HARVEST_CYCLE_NAME, TEST_PLANT_ID)).First(g => g.PlantId == TEST_PLANT_ID);

        plan.Notes = $"{plan.Notes} last update:{DateTime.Now.ToString()}";

        var response = await _plantHarvestClient.UpdatePlanHarvestCycle(plan);

        var returnString = await response.Content.ReadAsStringAsync();

        _output.WriteLine($"Service to up date plan harvest cycle responded with {response.StatusCode} code and {returnString} message");

        Assert.True(response.StatusCode == System.Net.HttpStatusCode.OK);
        Assert.NotEmpty(returnString);
    }

    [Fact]
    public async Task Put_PlanHarvestCycle_ShouldDelete()
    {
        var plan = (await GetPlanHarvestCyclesToWorkWith(TEST_DELETE_HARVEST_CYCLE_NAME, TEST_DELETE_PLANT_ID)).FirstOrDefault(g => g.PlantId == TEST_DELETE_PLANT_ID);

        if (plan == null)
        {
            //oh well. something deleted this plan already. will skip this round
            return;
        }

        var response = await _plantHarvestClient.DeletePlanHarvestCycle(plan.HarvestCycleId, plan.PlanHarvestCycleId);

        var returnString = await response.Content.ReadAsStringAsync();

        _output.WriteLine($"Service to delete plan harvest cycle responded with {response.StatusCode} code and {returnString} message");

        Assert.True(response.StatusCode == System.Net.HttpStatusCode.OK);
        Assert.NotEmpty(returnString);
    }

    #endregion

    #region Plant Harevst Cycle
    [Fact]
    public async Task Post_PlantHarvestCycle_MayCreateNew()
    {
        var harvestId = await GetHarvestCycleIdToWorkWith(TEST_HARVEST_CYCLE_NAME);

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
        var harvestId = await GetHarvestCycleIdToWorkWith(TEST_HARVEST_CYCLE_NAME);
        var plant = await GetPlantHarvestCycleToWorkWith(harvestId, TEST_PLANT_ID, TEST_PLANT_VARIETY_ID);

        Assert.NotNull(plant);
        _output.WriteLine($"Found '{plant.PlantVarietyId}' Plant Harvest Cycle");
        Assert.NotEmpty(plant.PlantVarietyId);
    }

    [Fact]
    public async Task Get_PlantHarvestCycle_One()
    {
        var harvestId = await GetHarvestCycleIdToWorkWith(TEST_HARVEST_CYCLE_NAME);
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
    public async Task Put_PlantHarvestCycle_ShouldUpdate()
    {
        var harvestId = await GetHarvestCycleIdToWorkWith(TEST_HARVEST_CYCLE_NAME);
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
        var harvestId = await GetHarvestCycleIdToWorkWith(TEST_DELETE_HARVEST_CYCLE_NAME);

        var plant = await GetPlantHarvestCycleToWorkWith(harvestId, TEST_DELETE_PLANT_ID, TEST_DELETE_VARIETY_ID);
        

        var response = await _plantHarvestClient.DeletePlantHarvestCycle(plant.HarvestCycleId, plant.PlantHarvestCycleId);

        var returnString = await response.Content.ReadAsStringAsync();

        _output.WriteLine($"Service to delete plant HarvestCycle responded with {response.StatusCode} code and {returnString} message");

        Assert.True(response.StatusCode == System.Net.HttpStatusCode.OK);
        Assert.NotEmpty(returnString);
    }
    #endregion

    #region Shared Private Functions
    private async Task<string> GetHarvestCycleIdToWorkWith(string harvestName)
    {
        var response = await _plantHarvestClient.GetHarvestCycleIdByHarvestCycleName(harvestName);
        var harvestId = await response.Content.ReadAsStringAsync();

        if (response.StatusCode != System.Net.HttpStatusCode.OK || string.IsNullOrEmpty(harvestId))
        {
            _output.WriteLine($"GetHarvestCycleIdToWorkWith - Harvest Cycle is not found. Will create new one");
            response = await _plantHarvestClient.CreateHarvestCycle(harvestName);

            harvestId = await response.Content.ReadAsStringAsync();

            _output.WriteLine($"GetHarvestCycleIdToWorkWith - Service to create harvest cycle responded with {response.StatusCode} code and {harvestId} message");
        }
        else
        {
            _output.WriteLine($"GetHarvestCycleIdToWorkWith - Harvest Cycle was found with service responded with {response.StatusCode} code and {harvestId} message");
        }

        _output.WriteLine($"GetHarvestCycleIdToWorkWith - Found  {harvestId} harvest to work with.");
        return harvestId;
    }

    private async Task<HarvestCycleViewModel> GetHarvestCycleToWorkWith(string harvestName)
    {

        var harvestId = await GetHarvestCycleIdToWorkWith(harvestName);

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

    private async Task<List<PlanHarvestCycleViewModel>> GetPlanHarvestCyclesToWorkWith(string harvestName, string plantId)
    {
        var harvestId = await GetHarvestCycleIdToWorkWith(harvestName);

        var response = await _plantHarvestClient.GetPlanHarvestCycles(harvestId);

        _output.WriteLine($"GetPlanHarvestCyclesToWorkWith - Service to get all plan harvest cycles responded with {response.StatusCode} code");

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
                {
                    new JsonStringEnumConverter(),
                },
        };
        var plans = await response.Content.ReadFromJsonAsync<List<PlanHarvestCycleViewModel>>(options);

        if (plans == null || plans.Count == 0)
        {
            _output.WriteLine($"GetPlanHarvestCyclesToWorkWith - Harvest {harvestId} has no plans");
            await _plantHarvestClient.CreatePlanHarvestCycle(harvestId, plantId);
            _output.WriteLine($"GetPlanHarvestCyclesToWorkWith - Create new plan for {harvestId} harvest with {plantId} plan");

            response = await _plantHarvestClient.GetPlanHarvestCycles(harvestId);

            _output.WriteLine($"GetPlanHarvestCyclesToWorkWith - Service to get all plan harvest cycles responded with {response.StatusCode} code");

            plans = await response.Content.ReadFromJsonAsync<List<PlanHarvestCycleViewModel>>(options);

            _output.WriteLine($"GetPlanHarvestCyclesToWorkWith - Harvest has {plans.Count} plans to work with");
        }

        return plans;
    }

    private async Task<PlanHarvestCycleViewModel> GetPlanHarvestCycleToWorkWith(string harvestId, string id)
    {
        var response = await _plantHarvestClient.GetPlanHarvestCycle(harvestId, id);

        _output.WriteLine($"Service to get single plant harvest cycle responded with {response.StatusCode} code");

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
                {
                    new JsonStringEnumConverter(),
                },
        };
        var growInstruction = await response.Content.ReadFromJsonAsync<PlanHarvestCycleViewModel>(options);

        Assert.NotNull(growInstruction);

        return growInstruction;
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

            response = await _plantHarvestClient.GetPlanHarvestCycles(harvestId);

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
