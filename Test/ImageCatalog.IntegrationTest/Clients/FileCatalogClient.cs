using GardenLog.SharedInfrastructure.Extensions;
using ImageCatalog.Contract;
using ImageCatalog.Contract.Commands;
using ImageCatalog.Contract.Queries;

namespace ImageCatalog.IntegrationTest.Clients
{
    public class FileCatalogClient
    {
        private readonly Uri _baseUrl;
        private readonly HttpClient _httpClient;

        public FileCatalogClient(Uri baseUrl, HttpClient httpClient)
        {
            _baseUrl = baseUrl;
            _httpClient = httpClient;
        }

        #region Image
        public async Task<HttpResponseMessage> GenerateSasUri(string fileName)
        {
            var url = $"{this._baseUrl.OriginalString}{Routes.GenerateSasToken}";

            return await this._httpClient.GetAsync(url.Replace("{fileName}", fileName));
        }

        public async Task<HttpResponseMessage> ResizeImageToThumbnail(string fileName)
        {
            var url = $"{this._baseUrl.OriginalString}{Routes.ResizeImageToThumbnail}";

            return await this._httpClient.GetAsync(url.Replace("{fileName}", fileName));

        }

        public async Task<HttpResponseMessage> DeletePLant(string id)
        {
            var url = $"{this._baseUrl.OriginalString}{Routes.DeleteImage}";

            return await this._httpClient.DeleteAsync(url.Replace("{imageId}", id));
        }

        public async Task<HttpResponseMessage> SearchImages(GetImagesByRelatedEntity searchQuery)
        {
            var url = $"{this._baseUrl.OriginalString}{Routes.Search}";

            using var requestContent = searchQuery.ToJsonStringContent();

            return await this._httpClient.PostAsync(url, requestContent);
        }

        public async Task<HttpResponseMessage> SearchImages(GetImagesByRelatedEntities searchQuery)
        {
            var url = $"{this._baseUrl.OriginalString}{Routes.SearchBatch}";

            using var requestContent = searchQuery.ToJsonStringContent();

            return await this._httpClient.PostAsync(url, requestContent);
        }

        private static CreateImageCommand PopulateCreateImageCommand(string name)
        {
            return new CreateImageCommand()
            {
                FileName = "TestFile.test",
                FileType = "*.test",
                ImageName = "TestFile",
                Label = "Test Label",
                RelatedEntityId = "TestEntity1",
                RelatedEntityType = Contract.Enum.ImageEntityEnum.Plant
            };
        }

        #endregion

    }
}
