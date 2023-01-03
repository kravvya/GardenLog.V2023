namespace PlantCatalog.IntegrationTest;

public class PlantCatalogTests : IClassFixture<PlantCatalogServiceFixture>
{
    private readonly ITestOutputHelper _output;
    private readonly PlantCatalogClient _plantCatalogClient;
    private const string TEST_PLANT_NAME = "Blackmelon";

    public PlantCatalogTests(PlantCatalogServiceFixture fixture, ITestOutputHelper output)
    {
        _plantCatalogClient = fixture.PlantCatalogClient;
        _output = output;
        _output.WriteLine($"Service id {fixture.FixtureId} @ {DateTime.Now.ToString("F")}");
    }

    [Fact]
    public async Task Post_Plant_ShouldCreateNewProduct()
    {
        var response = await _plantCatalogClient.CreatePlantCommand(TEST_PLANT_NAME);

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
        var response = await _plantCatalogClient.CreatePlantCommand("");

        var returnString = await response.Content.ReadAsStringAsync();

        _output.WriteLine($"Service responded with {response.StatusCode} code and {returnString} message");

        Assert.True(response.StatusCode == System.Net.HttpStatusCode.BadRequest);
        Assert.NotEmpty(returnString);
        Assert.Contains("'Name' must not be empty.", returnString);
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
