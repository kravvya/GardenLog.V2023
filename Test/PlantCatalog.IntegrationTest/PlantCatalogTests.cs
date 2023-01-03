

using PlantCatalog.IntegrationTest.Clients;
using PlantCatalog.IntegrationTest.Fixture;

namespace PlantCatalog.IntegrationTest;

public class PlantCatalogTests : IClassFixture<PlantCatalogServiceFixture>
{
    private readonly PlantCatalogClient _plantCatalogClient;
    private const string TEST_PLANT_NAME = "Blackmelon";

    public PlantCatalogTests(PlantCatalogServiceFixture fixture)
    {
        _plantCatalogClient = fixture.PlantCatalogClient;
    }

    [Fact]
    public async Task Post_Plant_ShouldCreateNewProduct()
    {
        // Act
        var response = await _plantCatalogClient.CreatePlantCommand(TEST_PLANT_NAME);

        var returnString = await response.Content.ReadAsStringAsync();

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

        Assert.True(response.StatusCode == System.Net.HttpStatusCode.BadRequest);
        Assert.NotEmpty(returnString);
        Assert.Contains("'Name' must not be empty.", returnString);
    }

    [Fact]
    public async Task Get_Should_Return_All_Plants()
    {
        var plants = await _plantCatalogClient.GetAllPlants();

        Assert.NotNull(plants);
        Assert.NotEmpty(plants);

        var testPlant = plants.FirstOrDefault(plants => plants.Name == TEST_PLANT_NAME);

        Assert.NotNull(testPlant);
    }

}
