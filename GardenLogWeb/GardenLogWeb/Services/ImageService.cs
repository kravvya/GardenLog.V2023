using Azure.Storage.Blobs;
using GardenLogWeb.Shared.Extensions;
using GardenLogWeb.Shared.Services;
using Microsoft.AspNetCore.Components.Forms;

namespace GardenLogWeb.Services;

public interface IImageService
{
    Task<ApiObjectResponse<string>> CreateImage(ImageModel image);
    Task<List<ImageModel>> GetImages(string entityType, string entityId, bool FilterUserOnly);
    Task<List<ImageModel>> GetImages(string entityType, bool FilterUserOnly);
    Task<List<ImageModel>> GetImagesInBulk(List<ImageRelatedEntityModel> entities);
    string GetRawImageUrl(string fileName);
    string GetThumbnailImageUrl(string fileName);
    Task UploadFile(IBrowserFile file, Action<long> progress, string fileName);
}

public class ImageService : IImageService
{
    private const string ThumbnailImageUrl = "https://glimagedev.blob.core.windows.net/thumbnails/";
    private const string RawImageUrl = "https://glimagedev.blob.core.windows.net/images/";

    public const string NO_IMAGE = "/images/noimage.png";

    private const string IMAGE_ROUTE = "/image";
    private readonly ILogger<ImageService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IGardenLogToastService _toastService;

    public ImageService(ILogger<ImageService> logger, IHttpClientFactory clientFactory, IGardenLogToastService toastService)
    {
        _logger = logger;
        _httpClientFactory = clientFactory;
        _toastService = toastService;
    }
    public async Task<List<ImageModel>> GetImages(string entityType, bool FilterUserOnly)
    {
        var httpClient = _httpClientFactory.CreateClient(GlobalConstants.GARDENLOG_WEB_SERVER);

        var response = await httpClient.ApiGetAsync<List<ImageModel>>($"{IMAGE_ROUTE}?RelatedEntityType={entityType}&FilterUserOnly={FilterUserOnly}");

        return response.Response;
    }

    public async Task<List<ImageModel>> GetImages(string entityType, string entityId, bool FilterUserOnly)
    {
        var httpClient = _httpClientFactory.CreateClient(GlobalConstants.GARDENLOG_WEB_SERVER);

        var response = await httpClient.ApiGetAsync<List<ImageModel>>($"{IMAGE_ROUTE}?RelatedEntityType={entityType}&RelatedEntityId={entityId}&FilterUserOnly={FilterUserOnly}");

        return response.Response;
    }

    public async Task<List<ImageModel>> GetImagesInBulk(List<ImageRelatedEntityModel> entities)
    {
        var request = new ImageRelatedEntities() { Requests = entities };

        var httpClient = _httpClientFactory.CreateClient(GlobalConstants.GARDENLOG_WEB_SERVER);
        var response = await httpClient.ApiPostAsync<List<ImageModel>>($"{IMAGE_ROUTE}/search/batch", request);

        return response.Response;
    }

    public async Task<ApiObjectResponse<string>> CreateImage(ImageModel image)
    {
        var httpClient = _httpClientFactory.CreateClient(GlobalConstants.GARDENLOG_WEB_SERVER);

        var response = await httpClient.ApiPostAsync(IMAGE_ROUTE, image);


        if (response.ValidationProblems != null)
        {
            _toastService.ShowToast($"Unable to create a Plant. Please resolve validatione errors and try again.", GardenLogToastLevel.Error);
        }
        else if (!response.IsSuccess)
        {
            _toastService.ShowToast($"Received an invalid response from Plant Post: {response.ErrorMessage}", GardenLogToastLevel.Error);
        }
        else
        {
            image.ImageId = response.Response;

            _toastService.ShowToast($"Image uploaded. Image id is {image.ImageId}", GardenLogToastLevel.Success);
        }


        return response;
    }

    public async Task UploadFile(IBrowserFile file, Action<long> progressReport, string fileName)
    {
        int maxAllowedSize = 10 * 1024 * 1024;

        var token = await GetSasToken(fileName);

        var blobClient = new BlobServiceClient(new Uri(token.Replace("\"", "")));
        var container = blobClient.GetBlobContainerClient("images");
        var blob = container.GetBlobClient(fileName);

        await blob.UploadAsync(file.OpenReadStream(maxAllowedSize)
            , new Azure.Storage.Blobs.Models.BlobUploadOptions()
            {
                ProgressHandler = new Progress<long>((progress) =>
                {
                    _logger.LogInformation("Uploading {progress}", progress);
                    progressReport(progress);
                })
            });

    }

    public string GetThumbnailImageUrl(string fileName)
    {
        if (fileName.Equals(ImageService.NO_IMAGE))
        {
            return fileName;
        }
        return $"{ThumbnailImageUrl}{fileName}";
    }

    public string GetRawImageUrl(string fileName)
    {
        if (fileName.Equals(ImageService.NO_IMAGE))
        {
            return fileName;
        }
        return $"{RawImageUrl}{fileName}";
    }

    private async Task<string> GetSasToken(string fileName)
    {
        var httpClient = _httpClientFactory.CreateClient(GlobalConstants.GARDENLOG_WEB_SERVER);
        var httpResponseMessage = await httpClient.GetAsync($"/files/tokens/{fileName}");

        httpResponseMessage.EnsureSuccessStatusCode();

        return await httpResponseMessage.Content.ReadAsStringAsync();

    }
}
