using GardenLog.SharedInfrastructure.Extensions;
using PlantHarvest.Contract;
using PlantHarvest.Contract.Commands;
using PlantHarvest.Contract.ViewModels;

namespace PlantHarvest.IntegrationTest.Clients
{
    public class PlantTaskClient
    {
        private readonly Uri _baseUrl;
        private readonly HttpClient _httpClient;

        public PlantTaskClient(Uri baseUrl, HttpClient httpClient)
        {
            _baseUrl = baseUrl;
            _httpClient = httpClient;
        }

        #region Plant Task

        public async Task<HttpResponseMessage> CreatePlantTask(string harvestCycleId, string plantHarvestCycelId)
        {
            var url = $"{this._baseUrl.OriginalString}{HarvestRoutes.CreateTask}/";

            var createPlantTaskCommand = PopulateCreatePlantTaskCommand(harvestCycleId, plantHarvestCycelId);

            using var requestContent = createPlantTaskCommand.ToJsonStringContent();

            return await this._httpClient.PostAsync(url, requestContent);

        }

        public async Task<HttpResponseMessage> UpdatePlantTask(PlantTaskViewModel task)
        {
            var url = $"{this._baseUrl.OriginalString}{HarvestRoutes.UpdateTask}";

            using var requestContent = task.ToJsonStringContent();

            return await this._httpClient.PutAsync(url.Replace("{id}", task.PlantTaskId), requestContent);
        }

        public async Task<HttpResponseMessage> CompletePlantTask(PlantTaskViewModel task)
        {
            var url = $"{this._baseUrl.OriginalString}{HarvestRoutes.CompleteTask}";

            using var requestContent = task.ToJsonStringContent();

            return await this._httpClient.PutAsync(url.Replace("{id}", task.PlantTaskId), requestContent);
        }

        public async Task<HttpResponseMessage> GetPlantTasks()
        {
            var url = $"{this._baseUrl.OriginalString}{HarvestRoutes.GetTasks}/";
            return await this._httpClient.GetAsync(url);
        }

        public async Task<HttpResponseMessage> GetActivePlantTasks()
        {
            var url = $"{this._baseUrl.OriginalString}{HarvestRoutes.GetTasks}/";
            return await this._httpClient.GetAsync(url);
        }

        private static CreatePlantTaskCommand PopulateCreatePlantTaskCommand(string harvestCycleId, string plantHarvestCycelId)
        {
            return new CreatePlantTaskCommand()
            {
                PlantName = "Test Plant Name",
                Description = "Created by Intergration Test",
                HarvestCycleId = harvestCycleId,
                IsSystemGenerated = true,
                Notes = "Created by Integration Test",
                PlantHarvestCycleId = plantHarvestCycelId,
                CreatedDateTime= DateTime.Now,
                TargetDateStart = DateTime.Now.AddDays(1),
                TargetDateEnd = DateTime.Now.AddDays(2),
                Type = Contract.Enum.WorkLogReasonEnum.Information
            };
        }
        #endregion
    }
}
