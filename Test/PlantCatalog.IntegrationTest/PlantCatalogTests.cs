using Microsoft.Extensions.Options;

namespace PlantCatalog.IntegrationTest;

public class PlantCatalogTests : IClassFixture<PlantCatalogServiceFixture>
{
    private readonly ITestOutputHelper _output;
    private readonly PlantCatalogClient _plantCatalogClient;
    private const string TEST_PLANT_NAME = "Blackmelon";
    private const string TEST_DELETE_PLANT_NAME = "DeletePlantName";

    public PlantCatalogTests(PlantCatalogServiceFixture fixture, ITestOutputHelper output)
    {
        _plantCatalogClient = fixture.PlantCatalogClient;
        _output = output;
        _output.WriteLine($"Service id {fixture.FixtureId} @ {DateTime.Now.ToString("F")}");
    }

    [Fact]
    public async Task Post_Plant_ShouldCreateNewProduct()
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

        plant.SeedViableForYears +=1;

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

}
