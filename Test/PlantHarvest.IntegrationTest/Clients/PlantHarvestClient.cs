using GardenLog.SharedInfrastructure.Extensions;
using PlantCatalog.Contract;
using PlantHarvest.Contract.Commands;
using PlantHarvest.Contract.ViewModels;
using Xunit.Abstractions;

namespace PlantHarvest.IntegrationTest.Clients
{
    public class PlantHarvestClient
    {
        private readonly Uri _baseUrl;
        private readonly HttpClient _httpClient;

        public PlantHarvestClient(Uri baseUrl, HttpClient httpClient)
        {
            _baseUrl = baseUrl;
            _httpClient = httpClient;
        }

        #region Harvest Cycle
        public async Task<HttpResponseMessage> GetHarvestCycleIdByHarvestCycleName(string name)
        {
            var url = $"{this._baseUrl.OriginalString}{HarvestRoutes.GetIdByHarvestCycleName}";
            return await this._httpClient.GetAsync(url.Replace("{name}", name));
        }

        public async Task<HttpResponseMessage> CreateHarvestCycle(string name)
        {
            var url = $"{this._baseUrl.OriginalString}{HarvestRoutes.CreateHarvestCycle}/";

            var createHarvestCycleCommand = PopulateCreateHarvestCycleCommand(name);

            using var requestContent = createHarvestCycleCommand.ToJsonStringContent();

            return await this._httpClient.PostAsync(url, requestContent);

        }

        public async Task<HttpResponseMessage> UpdateHarvestCycle(HarvestCycleViewModel harvest)
        {
            var url = $"{this._baseUrl.OriginalString}{HarvestRoutes.UpdateHarvestCycle}";

            using var requestContent = harvest.ToJsonStringContent();

            return await this._httpClient.PutAsync(url.Replace("{id}", harvest.HarvestCycleId), requestContent);
        }

        public async Task<HttpResponseMessage> DeleteHarvestCycle(string id)
        {
            var url = $"{this._baseUrl.OriginalString}{HarvestRoutes.DeleteHarvestCycle}";

            return await this._httpClient.DeleteAsync (url.Replace("{id}",id));
        }

        public async Task<HttpResponseMessage> GetAllHarvestCycles()
        {
            var url = $"{this._baseUrl.OriginalString}{HarvestRoutes.GetAllHarvestCycles}/";
           return await this._httpClient.GetAsync(url);           
        }

        public async Task<HttpResponseMessage> GetHarvestCycle(string id)
        {
            var url = $"{this._baseUrl.OriginalString}{HarvestRoutes.GetHarvestCycleById}";
            return await this._httpClient.GetAsync(url.Replace("{id}", id));
        }

        private static CreateHarvestCycleCommand PopulateCreateHarvestCycleCommand(string name)
        {
            return new CreateHarvestCycleCommand()
            {
               HarvestCycleName= name,
               StartDate = DateTime.Now,
               GardenId = "garden1",
               Notes ="Integration test Harvest Cycle"               
            };
        }
        #endregion

        #region Plan Harvest Cycle 

        public async Task<HttpResponseMessage> CreatePlanHarvestCycle(string harvestId, string plantId)
        {
            var url = $"{this._baseUrl.OriginalString}{HarvestRoutes.CreatePlanHarvestCycle}";

            var createPlanHarvestCycleCommand = PopulateCreatePlanHarvestCycleCommand(harvestId, plantId);

            using var requestContent = createPlanHarvestCycleCommand.ToJsonStringContent();

            var response =  await this._httpClient.PostAsync(url, requestContent);
                        
            return response;

        }

        public async Task<HttpResponseMessage> UpdatePlanHarvestCycle(PlanHarvestCycleViewModel plan)
        {
            var url = $"{this._baseUrl.OriginalString}{HarvestRoutes.UpdatePlanHarvestCycle}";

            using var requestContent = plan.ToJsonStringContent();

            return await this._httpClient.PutAsync(url.Replace("{harvestId}", plan.HarvestCycleId).Replace("{id}", plan.PlanHarvestCycleId), requestContent);
        }

        public async Task<HttpResponseMessage> DeletePlanHarvestCycle(string harvestId,string id)
        {
            var url = $"{this._baseUrl.OriginalString}{HarvestRoutes.DeletePlanHarvestCycle}";

            return await this._httpClient.DeleteAsync(url.Replace("{harvestId}", harvestId).Replace("{id}", id));
        }

        public async Task<HttpResponseMessage> GetPlanHarvestCycles(string harvestId)
        {
            var url = $"{this._baseUrl.OriginalString}{HarvestRoutes.GetPlanHarvestCycles}";
            return await this._httpClient.GetAsync(url.Replace("{harvestId}", harvestId));
        }

        public async Task<HttpResponseMessage> GetPlanHarvestCycle(string harvestId, string id)
        {
            var url = $"{this._baseUrl.OriginalString}{HarvestRoutes.GetPlanHarvestCycle}";
            return await this._httpClient.GetAsync(url.Replace("{harvestId}", harvestId).Replace("{id}", id));
        }

        private static CreatePlanHarvestCycleCommand PopulateCreatePlanHarvestCycleCommand (string harvestId, string plantId)
        {
            return new CreatePlanHarvestCycleCommand()
            {
                HarvestCycleId= harvestId,
                PlantId=plantId,
                NumberOfPlants=1,
                PlantGrowthInstructionId="Fake Growth Instruction",
                GardenBedId = null,
                Notes = "Created using Integration test"
            };
        }

        #endregion

        #region Plant Harvest Cycle 

        public async Task<HttpResponseMessage> CreatePlantHarvestCycle(string harvestId, string plantId, string plantVarietyId)
        {
            var url = $"{this._baseUrl.OriginalString}{HarvestRoutes.CreatePlantHarvestCycle}";

            var createPlantHarvestCycleCommand = PopulateCreatePlantHarvestCycleCommand(harvestId, plantId, plantVarietyId);

            using var requestContent = createPlantHarvestCycleCommand.ToJsonStringContent();

            return await this._httpClient.PostAsync(url, requestContent);

        }

        public async Task<HttpResponseMessage> UpdatePlantHarvestCycle(PlantHarvestCycleViewModel HarvestCycle)
        {
            var url = $"{this._baseUrl.OriginalString}{HarvestRoutes.UpdatePlantHarvestCycle}";

            using var requestContent = HarvestCycle.ToJsonStringContent();

            return await this._httpClient.PutAsync(url.Replace("{harvestId}", HarvestCycle.HarvestCycleId).Replace("{id}", HarvestCycle.PlantHarvestCycleId), requestContent);
        }

        public async Task<HttpResponseMessage> DeletePlantHarvestCycle(string harvestId, string id)
        {
            var url = $"{this._baseUrl.OriginalString}{HarvestRoutes.DeletePlantHarvestCycle}";

            return await this._httpClient.DeleteAsync(url.Replace("{harvestId}", harvestId).Replace("{id}", id));
        }

        public async Task<HttpResponseMessage> GetPlantHarvestCycles(string harvestId)
        {
            var url = $"{this._baseUrl.OriginalString}{HarvestRoutes.GetPlantHarvestCycles}";
            return await this._httpClient.GetAsync(url.Replace("{harvestId}", harvestId));
        }

        public async Task<HttpResponseMessage> GetPlantHarvestCyclesByPlantId(string plantId)
        {
            var url = $"{this._baseUrl.OriginalString}{HarvestRoutes.GetPlantHarvestCyclesByPlant}";
            return await this._httpClient.GetAsync(url.Replace("{plantId}", plantId));
        }

        public async Task<HttpResponseMessage> GetPlantHarvestCycle(string harvestId, string id)
        {
            var url = $"{this._baseUrl.OriginalString}{HarvestRoutes.GetPlantHarvestCycle}";
            return await this._httpClient.GetAsync(url.Replace("{harvestId}", harvestId).Replace("{id}", id));
        }

        private static CreatePlantHarvestCycleCommand PopulateCreatePlantHarvestCycleCommand(string harvestId, string plantId, string plantVarietyId)
        {
            return new CreatePlantHarvestCycleCommand()
            {
                HarvestCycleId= harvestId,
                PlantId= plantId,
                PlantName = "Test Plant",
                PlantVarietyId= plantVarietyId,
                PlantVarietyName="Test Variety",
                SeedVendorName= null,
                SeedVendorId= null,
                PlantGrowthInstructionId = "Fake Growth Instruction",
                PlantGrowthInstructionName = "Test Growth Instruction",
                PlantingMethod=Contract.Enum.PlantingMethodEnum.SeedIndoors,
                Notes ="Created by Integration test",
                SeedingDateTime=DateTime.Now
            };
        }
        #endregion

        #region Shared Functions
        public async Task<string> GetHarvestCycleIdToWorkWith(string harvestName)
        {
            var response = await this.GetHarvestCycleIdByHarvestCycleName(harvestName);
            var harvestId = await response.Content.ReadAsStringAsync();

            if (response.StatusCode != System.Net.HttpStatusCode.OK || string.IsNullOrEmpty(harvestId))
            {
                //_output.WriteLine($"GetHarvestCycleIdToWorkWith - Harvest Cycle is not found. Will create new one");
                response = await this.CreateHarvestCycle(harvestName);

                harvestId = await response.Content.ReadAsStringAsync();

                //_output.WriteLine($"GetHarvestCycleIdToWorkWith - Service to create harvest cycle responded with {response.StatusCode} code and {harvestId} message");
            }
            else
            {
                //_output.WriteLine($"GetHarvestCycleIdToWorkWith - Harvest Cycle was found with service responded with {response.StatusCode} code and {harvestId} message");
            }

            //_output.WriteLine($"GetHarvestCycleIdToWorkWith - Found  {harvestId} harvest to work with.");
            return harvestId;
        }
        #endregion
    }
}
