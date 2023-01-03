using GardenLog.SharedInfrastructure.Extensions;
using Newtonsoft.Json;
using PlantCatalog.Contract;
using PlantCatalog.Contract.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using PlantCatalog.Contract.ViewModels;
using System.Net.Http.Json;

namespace PlantCatalog.IntegrationTest.Clients
{
    public class PlantCatalogClient
    {
        private readonly Uri _baseUrl;
        private readonly HttpClient _httpClient;

        public PlantCatalogClient(Uri baseUrl, HttpClient httpClient)
        {
            _baseUrl = baseUrl;
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> CreatePlantCommand(string name)
        {
            var url = $"{this._baseUrl}{Routes.CreatePlant}/";

            var createPlantCommand = PopulateCreatePlantCommand(name);

            using var requestContent = createPlantCommand.ToJsonStringContent();

            return await this._httpClient.PostAsync(url, requestContent);

        }

        public async Task<IReadOnlyCollection<PlantViewModel>> GetAllPlants()
        {
            var url = $"{this._baseUrl}{Routes.GetAllPlants}/";
            var response = await this._httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters =
                {
                    new JsonStringEnumConverter(),
                },
            };
            return await response.Content.ReadFromJsonAsync<List<PlantViewModel>>(options);
        }

        private static CreatePlantCommand PopulateCreatePlantCommand(string name)
        {
            return new CreatePlantCommand()
            {
                Color = "Black",
                Description = "Black Vegetable",
                GardenTip = "Only grows at night",
                GrowTolerance = Contract.Enum.GrowToleranceEnum.LightFrost | Contract.Enum.GrowToleranceEnum.HardFrost,
                Lifecycle = Contract.Enum.PlantLifecycleEnum.Cool,
                LightRequirement = Contract.Enum.LightRequirementEnum.FullShade,
                MoistureRequirement = Contract.Enum.MoistureRequirementEnum.DroutTolerant,
                Name = name,
                SeedViableForYears = 10,
                Type = Contract.Enum.PlantTypeEnum.Vegetable

            };
        }
    }
}
