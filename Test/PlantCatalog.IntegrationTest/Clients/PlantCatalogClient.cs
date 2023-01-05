using GardenLog.SharedInfrastructure.Extensions;
using PlantCatalog.Contract;
using PlantCatalog.Contract.Commands;

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

        public async Task<HttpResponseMessage> GetPlantIdByPlantName(string name)
        {
            var url = $"{this._baseUrl.OriginalString}{Routes.GetIdByPlantName}";
            return await this._httpClient.GetAsync(url.Replace("{name}", name));
        }

        public async Task<HttpResponseMessage> CreatePlant(string name)
        {
            var url = $"{this._baseUrl.OriginalString}{Routes.CreatePlant}/";

            var createPlantCommand = PopulateCreatePlantCommand(name);

            using var requestContent = createPlantCommand.ToJsonStringContent();

            return await this._httpClient.PostAsync(url, requestContent);

        }

        public async Task<HttpResponseMessage> UpdatePlant(PlantViewModel plant)
        {
            var url = $"{this._baseUrl.OriginalString}{Routes.UpdatePlant}";

            using var requestContent = plant.ToJsonStringContent();

            return await this._httpClient.PutAsync(url.Replace("{id}", plant.PlantId), requestContent);

        }

        public async Task<HttpResponseMessage> DeletePLant(string id)
        {
            var url = $"{this._baseUrl.OriginalString}{Routes.DeletePlant}";

            return await this._httpClient.DeleteAsync (url.Replace("{id}",id));
        }

        public async Task<HttpResponseMessage> GetAllPlants()
        {
            var url = $"{this._baseUrl.OriginalString}{Routes.GetAllPlants}/";
           return await this._httpClient.GetAsync(url);           
        }

        public async Task<HttpResponseMessage> GetPlant(string id)
        {
            var url = $"{this._baseUrl.OriginalString}{Routes.GetPlantById}";
            return await this._httpClient.GetAsync(url.Replace("{id}", id));
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
