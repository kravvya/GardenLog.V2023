namespace PlantCatalog.IntegrationTest;

public class PlantCatalogTests : IClassFixture<PlantCatalogServiceFixture>
{
    private readonly ITestOutputHelper _output;
    private readonly PlantCatalogClient _plantCatalogClient;
    private const string TEST_PLANT_NAME = "Blackmelon";
    private const string TEST_GROW_INSTRUCTION_NAME = "Start at home and pick in the Summer";
    private const string TEST_DELETE_GROW_INSTRUCTION_NAME = "Go buy something fromt the store";
    private const string TEST_DELETE_PLANT_NAME = "DeletePlantName";

    public PlantCatalogTests(PlantCatalogServiceFixture fixture, ITestOutputHelper output)
    {
        _plantCatalogClient = fixture.PlantCatalogClient;
        _output = output;
        _output.WriteLine($"Service id {fixture.FixtureId} @ {DateTime.Now.ToString("F")}");
    }

    #region Plant
    [Fact]
    public async Task Post_Plant_MayCreateNewProduct()
    {
        var response = await _plantCatalogClient.CreatePlant(TEST_PLANT_NAME);

        var returnString = await response.Content.ReadAsStringAsync();

        _output.WriteLine($"Service responded with {response.StatusCode} code and {returnString} message");

        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            Assert.NotEmpty(returnString);
            Assert.True(Guid.TryParse(returnString, out var plantId));
        }
        else
        {
            Assert.True(response.StatusCode == System.Net.HttpStatusCode.BadRequest);
            Assert.NotEmpty(returnString);
            Assert.Contains("Plant with this name already exists", returnString);
        }
    }

    [Fact]
    public async Task Post_Plant_ShouldNotCreateNewProduct_WithoutName()
    {
        var response = await _plantCatalogClient.CreatePlant("");

        var returnString = await response.Content.ReadAsStringAsync();

        _output.WriteLine($"Service responded with {response.StatusCode} code and {returnString} message");

        Assert.True(response.StatusCode == System.Net.HttpStatusCode.BadRequest);
        Assert.NotEmpty(returnString);
        Assert.Contains("'Name' must not be empty.", returnString);
    }

    [Fact]
    public async Task Put_Plant_ShouldUpdatePlant()
    {
        var response = await _plantCatalogClient.GetPlantIdByPlantName(TEST_PLANT_NAME);

        var returnString = await response.Content.ReadAsStringAsync();

        _output.WriteLine($"Plant to update was found with service responded with {response.StatusCode} code and {returnString} message");

        if (response.StatusCode != System.Net.HttpStatusCode.OK || string.IsNullOrEmpty(returnString))
        {
            _output.WriteLine($"Plant to update is not found. Will create new one");
            response = await _plantCatalogClient.CreatePlant(TEST_PLANT_NAME);

            returnString = await response.Content.ReadAsStringAsync();

            _output.WriteLine($"Service responded with {response.StatusCode} code and {returnString} message");
        }

        response = await _plantCatalogClient.GetPlant(returnString);
        returnString = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Service responded with {response.StatusCode} code and {returnString} message");

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
                {
                    new JsonStringEnumConverter(),
                },
        };
        var plant = await response.Content.ReadFromJsonAsync<PlantViewModel>(options);

        plant.SeedViableForYears += 1;

        response = await _plantCatalogClient.UpdatePlant(plant);

        returnString = await response.Content.ReadAsStringAsync();

        _output.WriteLine($"Service responded with {response.StatusCode} code and {returnString} message");

        Assert.True(response.StatusCode == System.Net.HttpStatusCode.OK);
        Assert.NotEmpty(returnString);
    }

    [Fact]
    public async Task Delete_Plant()
    {
        var response = await _plantCatalogClient.GetPlantIdByPlantName(TEST_DELETE_PLANT_NAME);

        var returnString = await response.Content.ReadAsStringAsync();

        _output.WriteLine($"Service responded with {response.StatusCode} code and {returnString} message");

        if (response.StatusCode != System.Net.HttpStatusCode.OK || string.IsNullOrEmpty(returnString))
        {
            _output.WriteLine($"Plant to delete is not found. Will create new one");
            response = await _plantCatalogClient.CreatePlant(TEST_DELETE_PLANT_NAME);

            returnString = await response.Content.ReadAsStringAsync();

            _output.WriteLine($"Service responded with {response.StatusCode} code and {returnString} message");
        }

        response = await _plantCatalogClient.DeletePLant(returnString);

        returnString = await response.Content.ReadAsStringAsync();

        _output.WriteLine($"Service responded with {response.StatusCode} code and {returnString} message");

        Assert.True(response.StatusCode == System.Net.HttpStatusCode.OK);
        Assert.NotEmpty(returnString);
    }

    [Fact]
    public async Task Get_Should_Return_All_Plants()
    {
        var response = await _plantCatalogClient.GetAllPlants();

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
                {
                    new JsonStringEnumConverter(),
                },
        };

        var returnString = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Service responded with {response.StatusCode} code and {returnString} message");

        var plants = await response.Content.ReadFromJsonAsync<List<PlantViewModel>>(options);

        Assert.NotNull(plants);
        Assert.NotEmpty(plants);

        var testPlant = plants.FirstOrDefault(plants => plants.Name == TEST_PLANT_NAME);

        Assert.NotNull(testPlant);
    }

    #endregion

    #region Plant Grow Instruction

    [Fact]
    public async Task Post_PlantGrowInstruction_MayCreateNew()
    {
        var plantId = await GetPlantIdToWorkWith(TEST_PLANT_NAME);

        var response = await _plantCatalogClient.CreatePlantGrowInstruction(plantId, TEST_GROW_INSTRUCTION_NAME);
        var returnString = await response.Content.ReadAsStringAsync();

        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            Assert.NotEmpty(returnString);
            Assert.True(Guid.TryParse(returnString, out var plantGrowInstructionId));
        }
        else
        {
            Assert.True(response.StatusCode == System.Net.HttpStatusCode.BadRequest);
            returnString = await response.Content.ReadAsStringAsync();
            Assert.NotEmpty(returnString);
            Assert.Contains("Grow Instruction with this name already exists", returnString);
        }
    }

    [Fact]
    public async Task Get_PlantGrowInstruction_All()
    {
        var growInstructions = await GetPlantGrowInstructionsToWorkWith();

      
        Assert.NotNull(growInstructions);
        _output.WriteLine($"Found '{growInstructions.First().Name}' grow instruction");
        Assert.NotEmpty(growInstructions.First().HarvestInstructions);
    }

    [Fact]
    public async Task Get_PlantGrowInstruction_One()
    {
        var growInstructions = await GetPlantGrowInstructionsToWorkWith();

        var original = growInstructions.First(g => g.Name == TEST_GROW_INSTRUCTION_NAME);

        if (growInstructions.Count == 1)
        {
            //create new grow instruction to make sure we only get one back
            var response = await _plantCatalogClient.CreatePlantGrowInstruction(original.PlantId, TEST_DELETE_GROW_INSTRUCTION_NAME);
        }

        var growInstruction = await GetPlantGrowInstructionToWorkWith(original.PlantId, original.PlantGrowInstructionId);
               
        _output.WriteLine($"Found '{growInstruction.Name}' grow instruction");
        Assert.Equal(original.Name, growInstruction.Name);
    }

    [Fact]
    public async Task Put_PlantGrowInstruction_ShouldUpdate()
    {
        var grow = (await GetPlantGrowInstructionsToWorkWith()).First(g => g.Name == TEST_GROW_INSTRUCTION_NAME);

        //Step 3 update grow Instruction

        grow.DaysToSproutMin += 1;

        var response = await _plantCatalogClient.UpdatePlantGrowInstruction(grow);

        var returnString = await response.Content.ReadAsStringAsync();

        _output.WriteLine($"Service responded with {response.StatusCode} code and {returnString} message");

        Assert.True(response.StatusCode == System.Net.HttpStatusCode.OK);
        Assert.NotEmpty(returnString);
    }

    [Fact]
    public async Task Put_PlantGrowInstruction_ShouldDelete()
    {
        var grow = (await GetPlantGrowInstructionsToWorkWith()).FirstOrDefault(g => g.Name == TEST_DELETE_GROW_INSTRUCTION_NAME);

        if(grow == null)
        {
            //oh well. something deleted this grow nstruction already. will skip this round
            return;
        }

        //Step 3 update grow Instruction

        grow.DaysToSproutMin += 1;

        var response = await _plantCatalogClient.DeletePlantGrowInstruction(grow.PlantId, grow.PlantGrowInstructionId);

        var returnString = await response.Content.ReadAsStringAsync();

        _output.WriteLine($"Service to delete plant grow instruction responded with {response.StatusCode} code and {returnString} message");

        Assert.True(response.StatusCode == System.Net.HttpStatusCode.OK);
        Assert.NotEmpty(returnString);
    }

   
    #endregion

    #region Shared Private Functions
    private async Task<string> GetPlantIdToWorkWith(string plantName)
    {
        //Step 1 - Get Plant to work with
        var response = await _plantCatalogClient.GetPlantIdByPlantName(plantName);
        var plantId = await response.Content.ReadAsStringAsync();

        if (response.StatusCode != System.Net.HttpStatusCode.OK || string.IsNullOrEmpty(plantId))
        {
            _output.WriteLine($"Plant is not found. Will create new one");
            response = await _plantCatalogClient.CreatePlant(plantName);

            plantId = await response.Content.ReadAsStringAsync();

            _output.WriteLine($"Service to create plant responded with {response.StatusCode} code and {plantId} message");
        }
        else
        {
            _output.WriteLine($"Plant was found with service responded with {response.StatusCode} code and {plantId} message");
        }

        return plantId;
    }

    private async Task<List<PlantGrowInstructionViewModel>> GetPlantGrowInstructionsToWorkWith()
    {
        var plantId = await GetPlantIdToWorkWith(TEST_PLANT_NAME);

        //Step 2 - Get Grow Instruction to update
        var response = await _plantCatalogClient.GetPlantGrowInstructions(plantId);

        _output.WriteLine($"Service to get all plant grow instructions responded with {response.StatusCode} code");

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
                {
                    new JsonStringEnumConverter(),
                },
        };
        var growInstructions = await response.Content.ReadFromJsonAsync<List<PlantGrowInstructionViewModel>>(options);

        Assert.NotNull(growInstructions);
        Assert.NotEmpty(growInstructions);

        return growInstructions;
    }

    private async Task<PlantGrowInstructionViewModel> GetPlantGrowInstructionToWorkWith(string plantId, string growId)
    {
        var response = await _plantCatalogClient.GetPlantGrowInstruction(plantId, growId);

        _output.WriteLine($"Service to get single plant grow instruction responded with {response.StatusCode} code");

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
                {
                    new JsonStringEnumConverter(),
                },
        };
        var growInstruction = await response.Content.ReadFromJsonAsync<PlantGrowInstructionViewModel>(options);

        Assert.NotNull(growInstruction);

        return growInstruction;
    }
    #endregion
}
