﻿using GardenLog.SharedInfrastructure.Extensions;
using PlantCatalog.Contract;
using PlantHarvest.Contract.Commands;
using PlantHarvest.Contract.ViewModels;

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
            var url = $"{this._baseUrl.OriginalString}{Routes.GetIdByHarvestCycleName}";
            return await this._httpClient.GetAsync(url.Replace("{name}", name));
        }

        public async Task<HttpResponseMessage> CreateHarvestCycle(string name)
        {
            var url = $"{this._baseUrl.OriginalString}{Routes.CreateHarvestCycle}/";

            var createHarvestCycleCommand = PopulateCreateHarvestCycleCommand(name);

            using var requestContent = createHarvestCycleCommand.ToJsonStringContent();

            return await this._httpClient.PostAsync(url, requestContent);

        }

        public async Task<HttpResponseMessage> UpdateHarvestCycle(HarvestCycleViewModel harvest)
        {
            var url = $"{this._baseUrl.OriginalString}{Routes.UpdateHarvestCycle}";

            using var requestContent = harvest.ToJsonStringContent();

            return await this._httpClient.PutAsync(url.Replace("{id}", harvest.HarvestCycleId), requestContent);
        }

        public async Task<HttpResponseMessage> DeleteHarvestCycle(string id)
        {
            var url = $"{this._baseUrl.OriginalString}{Routes.DeleteHarvestCycle}";

            return await this._httpClient.DeleteAsync (url.Replace("{id}",id));
        }

        public async Task<HttpResponseMessage> GetAllHarvestCycles()
        {
            var url = $"{this._baseUrl.OriginalString}{Routes.GetAllHarvestCycles}/";
           return await this._httpClient.GetAsync(url);           
        }

        public async Task<HttpResponseMessage> GetHarvestCycle(string id)
        {
            var url = $"{this._baseUrl.OriginalString}{Routes.GetHarvestCycleById}";
            return await this._httpClient.GetAsync(url.Replace("{id}", id));
        }

        private static CreateHarvestCycleCommand PopulateCreateHarvestCycleCommand(string name)
        {
            return new CreateHarvestCycleCommand()
            {
               HarvestCycleName= name,
               StartDate = DateTime.Now,
               GardenId = Guid.NewGuid().ToString(),
               Notes="Integration test Harvest Cycle"               
            };
        }
        #endregion

        #region Plan Harvest Cycle 

        public async Task<HttpResponseMessage> CreatePlanHarvestCycle(string harvestId, string plantId)
        {
            var url = $"{this._baseUrl.OriginalString}{Routes.CreatePlanHarvestCycle}";

            var createPlanHarvestCycleCommand = PopulateCreatePlanHarvestCycleCommand(harvestId, plantId);

            using var requestContent = createPlanHarvestCycleCommand.ToJsonStringContent();

            return await this._httpClient.PostAsync(url, requestContent);

        }

        public async Task<HttpResponseMessage> UpdatePlanHarvestCycle(PlanHarvestCycleViewModel plan)
        {
            var url = $"{this._baseUrl.OriginalString}{Routes.UpdatePlanHarvestCycle}";

            using var requestContent = plan.ToJsonStringContent();

            return await this._httpClient.PutAsync(url.Replace("{harvestId}", plan.HarvestCycleId).Replace("{id}", plan.PlanHarvestCycleId), requestContent);
        }

        public async Task<HttpResponseMessage> DeletePlanHarvestCycle(string harvestId,string id)
        {
            var url = $"{this._baseUrl.OriginalString}{Routes.DeletePlanHarvestCycle}";

            return await this._httpClient.DeleteAsync(url.Replace("{harvestId}", harvestId).Replace("{id}", id));
        }

        public async Task<HttpResponseMessage> GetPlanHarvestCycles(string harvestId)
        {
            var url = $"{this._baseUrl.OriginalString}{Routes.GetPlanHarvestCycles}";
            return await this._httpClient.GetAsync(url.Replace("{harvestId}", harvestId));
        }

        public async Task<HttpResponseMessage> GetPlanHarvestCycle(string harvestId, string id)
        {
            var url = $"{this._baseUrl.OriginalString}{Routes.GetPlanHarvestCycle}";
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
            var url = $"{this._baseUrl.OriginalString}{Routes.CreatePlantHarvestCycle}";

            var createPlantHarvestCycleCommand = PopulateCreatePlantHarvestCycleCommand(harvestId, plantId, plantVarietyId);

            using var requestContent = createPlantHarvestCycleCommand.ToJsonStringContent();

            return await this._httpClient.PostAsync(url, requestContent);

        }

        public async Task<HttpResponseMessage> UpdatePlantHarvestCycle(PlantHarvestCycleViewModel HarvestCycle)
        {
            var url = $"{this._baseUrl.OriginalString}{Routes.UpdatePlantHarvestCycle}";

            using var requestContent = HarvestCycle.ToJsonStringContent();

            return await this._httpClient.PutAsync(url.Replace("{harvestId}", HarvestCycle.HarvestCycleId).Replace("{id}", HarvestCycle.PlantHarvestCycleId), requestContent);
        }

        public async Task<HttpResponseMessage> DeletePlantHarvestCycle(string harvestId, string id)
        {
            var url = $"{this._baseUrl.OriginalString}{Routes.DeletePlantHarvestCycle}";

            return await this._httpClient.DeleteAsync(url.Replace("{harvestId}", harvestId).Replace("{id}", id));
        }

        public async Task<HttpResponseMessage> GetPlantHarvestCycles(string harvestId)
        {
            var url = $"{this._baseUrl.OriginalString}{Routes.GetPlantHarvestCycles}";
            return await this._httpClient.GetAsync(url.Replace("{harvestId}", harvestId));
        }


        public async Task<HttpResponseMessage> GetPlantHarvestCycle(string harvestId, string id)
        {
            var url = $"{this._baseUrl.OriginalString}{Routes.GetPlantHarvestCycle}";
            return await this._httpClient.GetAsync(url.Replace("{harvestId}", harvestId).Replace("{id}", id));
        }

        private static CreatePlantHarvestCycleCommand PopulateCreatePlantHarvestCycleCommand(string harvestId, string plantId, string plantVarietyId)
        {
            return new CreatePlantHarvestCycleCommand()
            {
                HarvestCycleId= harvestId,
                PlantId= plantId,
                PlantVarietyId= plantVarietyId,
                SeedCompanyName= null,
                SeedCompanyId= null,
                PlantGrowthInstructionId = "Fake Growth Instruction",
                Notes="Created by Integration test",
                SeedingDateTime=DateTime.Now,
                IsDirectSeed=false
            };
        }
        #endregion
    }
}
