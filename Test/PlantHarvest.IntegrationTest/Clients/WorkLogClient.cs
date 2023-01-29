using GardenLog.SharedInfrastructure.Extensions;
using PlantCatalog.Contract;
using PlantHarvest.Contract.Commands;
using PlantHarvest.Contract.Enum;
using PlantHarvest.Contract.ViewModels;

namespace PlantHarvest.IntegrationTest.Clients
{
    public class WorkLogClient
    {
        private readonly Uri _baseUrl;
        private readonly HttpClient _httpClient;

        public WorkLogClient(Uri baseUrl, HttpClient httpClient)
        {
            _baseUrl = baseUrl;
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("RequestUser", "86377291-980f-4af2-8608-39dbbf7e09e1");
        }

        #region Work Log
        
        public async Task<HttpResponseMessage> CreateWorkLog(WorkLogEntityEnum entityType, string entityId)
        {
            var url = $"{this._baseUrl.OriginalString}{HarvestRoutes.CreateWorkLog}/";

            var createWorkLogCommand = PopulateCreateWorkLogCommand(entityType, entityId);

            using var requestContent = createWorkLogCommand.ToJsonStringContent();

            return await this._httpClient.PostAsync(url, requestContent);

        }

        public async Task<HttpResponseMessage> UpdateWorkLog(WorkLogViewModel workLog)
        {
            var url = $"{this._baseUrl.OriginalString}{HarvestRoutes.UpdateWorkLog}";

            using var requestContent = workLog.ToJsonStringContent();

            return await this._httpClient.PutAsync(url.Replace("{id}", workLog.WorkLogId), requestContent);
        }

        public async Task<HttpResponseMessage> DeleteWorkLog(string id)
        {
            var url = $"{this._baseUrl.OriginalString}{HarvestRoutes.DeleteWorkLog}";

            return await this._httpClient.DeleteAsync(url.Replace("{id}", id));
        }

        public async Task<HttpResponseMessage> GetWorkLogs(WorkLogEntityEnum entityType, string entityId)
        {
            var url = $"{this._baseUrl.OriginalString}{HarvestRoutes.GetWorkLogs}/";
            return await this._httpClient.GetAsync(url.Replace("{entityType}", entityType.ToString()).Replace("{entityId}", entityId));
        }

        private static CreateWorkLogCommand PopulateCreateWorkLogCommand(WorkLogEntityEnum entityType, string entityId)
        {
            return new CreateWorkLogCommand()
            {
                Log = "Created by Integration test",
                EventDateTime = DateTime.Now,
                EnteredDateTime = DateTime.Now,
                Reason = Contract.Enum.WorkLogReasonEnum.Information,
                RelatedEntity = entityType,
                RelatedEntityid = entityId
            };
        }
        #endregion
    }
}
