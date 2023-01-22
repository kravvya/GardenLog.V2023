using GardenLog.SharedInfrastructure.Extensions;
using ImageCatalog.Contract;
using ImageCatalog.Contract.Commands;
using ImageCatalog.Contract.Queries;

namespace ImageCatalog.IntegrationTest.Clients
{
    public class ImageCatalogClient
    {
        private readonly Uri _baseUrl;
        private readonly HttpClient _httpClient;

        public ImageCatalogClient(Uri baseUrl, HttpClient httpClient)
        {
            _baseUrl = baseUrl;
            _httpClient = httpClient;
        }

        #region Image
        public async Task<HttpResponseMessage> CreateImage(string name)
        {
            var url = $"{this._baseUrl.OriginalString}{ImageRoutes.CrerateImage}/";

            var createImageCommand = PopulateCreateImageCommand(name);

            using var requestContent = createImageCommand.ToJsonStringContent();

            return await this._httpClient.PostAsync(url, requestContent);

        }

        public async Task<HttpResponseMessage> UpdateImage(ImageViewModel image)
        {
            var url = $"{this._baseUrl.OriginalString}{ImageRoutes.UpdateImage}";

            using var requestContent = image.ToJsonStringContent();

            return await this._httpClient.PutAsync(url.Replace("{imageId}", image.ImageId), requestContent);

        }

        public async Task<HttpResponseMessage> DeleteIamge(string id)
        {
            var url = $"{this._baseUrl.OriginalString}{ImageRoutes.DeleteImage}";

            return await this._httpClient.DeleteAsync(url.Replace("{imageId}", id));
        }

        public async Task<HttpResponseMessage> SearchImages(GetImagesByRelatedEntity searchQuery)
        {
            var url = $"{this._baseUrl.OriginalString}{ImageRoutes.Search}";
                       
            using var requestContent = searchQuery.ToJsonStringContent();

            return await this._httpClient.PostAsync(url, requestContent);           
        }

        public async Task<HttpResponseMessage> SearchImages(GetImagesByRelatedEntities searchQuery)
        {
            var url = $"{this._baseUrl.OriginalString}{ImageRoutes.SearchBatch}";

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
