

namespace GardenLogWeb.Services
{
    public interface IGardenService
    {
        Task<List<GardenModel>> GetGardens(bool forceRefresh);
        Task<GardenModel> GetGarden(string gardenId, bool useCache);
        Task<ApiObjectResponse<string>> CreateGarden(GardenModel garden);
        Task<ApiResponse> UpdateGarden(GardenModel garden);
        Task<ApiResponse> DeleteGarden(string gardenId);
    }

    public class GardenService : IGardenService
    {
        private readonly ILogger<GardenService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ICacheService _cacheService;
        private readonly IGardenLogToastService _toastService;
        private readonly IImageService _imageService;

        private const int CACHE_DURATION = 10;
        private const string GARDENS_KEY = "Gardens";

        public GardenService(ILogger<GardenService> logger, IHttpClientFactory clientFactory, ICacheService cacheService, IGardenLogToastService toastService, IImageService imageService)
        {
            _logger = logger;
            _httpClientFactory = clientFactory;
            _cacheService = cacheService;
            _toastService = toastService;
            _imageService = imageService;
        }

        #region Garden Functions

        public async Task<List<GardenModel>> GetGardens(bool forceRefresh)
        {
            List<GardenModel> gardens = null;

            if (forceRefresh || !_cacheService.TryGetValue<List<GardenModel>>(GARDENS_KEY, out gardens))
            {
                _logger.LogInformation("Gardens not in cache or forceRefresh");

                var gardenTast = GetAllGardens();
                var imagesTask = _imageService.GetImages(RelatedEntityTypEnum.Plant, false);

                await Task.WhenAll(gardenTast, imagesTask);

                gardens = gardenTast.Result;
                var images = imagesTask.Result;

                if (gardens.Count > 0)
                {

                    foreach (var garden in gardens)
                    {
                        garden.Images = images.Where(i => i.RelatedEntityId == garden.GardenId).ToList();

                        var image = garden.Images.FirstOrDefault();
                        if (image != null)
                        {
                            garden.ImageFileName = image.FileName;
                            garden.ImageLabel = image.Label;
                        }
                        else
                        {
                            garden.ImageFileName = ImageService.NO_IMAGE;
                            garden.ImageLabel = "Add image";
                        }
                    }

                    // Save data in cache.
                    _cacheService.Set(GARDENS_KEY, gardens, DateTime.Now.AddMinutes(CACHE_DURATION));
                }

            }
            else
            {
                _logger.LogInformation($"Gardens are in cache. Found {gardens.Count()}");
            }

            return gardens;
        }

        public async Task<GardenModel> GetGarden(string gardenId, bool useCache)
        {
            GardenModel garden = null;

            var plants = (await GetGardens(false));

            if (useCache)
            {
                garden = plants.FirstOrDefault(p => p.GardenId == gardenId);
            }

            if (garden == null)
            {
                var httpClient = _httpClientFactory.CreateClient(GlobalConstants.PLANTCATALOG_API);

                var response = await httpClient.ApiGetAsync<GardenModel>(GardenRoutes.GetGarden.Replace("{gardenId}", gardenId));

                if (!response.IsSuccess)
                {
                    _toastService.ShowToast("Unable to get Garden details", GardenLogToastLevel.Error);
                    return null;
                }

                garden = response.Response;

               await AddOrUpdateToGardenList(garden);
            }

            return garden;
        }

        public async Task<ApiObjectResponse<string>> CreateGarden(GardenModel garden)
        {
            var httpClient = _httpClientFactory.CreateClient(GlobalConstants.USERMANAGEMENT_API);

            var response = await httpClient.ApiPostAsync(GardenRoutes.CreateGarden, garden);

            if (response.ValidationProblems != null)
            {
                _toastService.ShowToast($"Unable to create a Garden. Please resolve validatione errors and try again.", GardenLogToastLevel.Error);
            }
            else if (!response.IsSuccess)
            {
                _toastService.ShowToast($"Received an invalid response from Garden post: {response.ErrorMessage}", GardenLogToastLevel.Error);
            }
            else
            {
                garden.GardenId = response.Response;
                garden.Images = new();
                garden.ImageFileName = ImageService.NO_IMAGE;
                garden.ImageLabel = string.Empty;

                await AddOrUpdateToGardenList(garden);

                _toastService.ShowToast($"Garden saved", GardenLogToastLevel.Success);
            }

            return response;
        }

        public async Task<ApiResponse> UpdateGarden(GardenModel harvest)
        {
            var httpClient = _httpClientFactory.CreateClient(GlobalConstants.USERMANAGEMENT_API);

            var response = await httpClient.ApiPutAsync(GardenRoutes.UpdateGarden.Replace("{gardenId}", harvest.GardenId), harvest);

            if (response.ValidationProblems != null)
            {
                _toastService.ShowToast($"Unable to update Garden. Please resolve validatione errors and try again.", GardenLogToastLevel.Error);
            }
            else if (!response.IsSuccess)
            {
                _toastService.ShowToast($"Received an invalid response: {response.ErrorMessage}", GardenLogToastLevel.Error);
            }
            else
            {
                await AddOrUpdateToGardenList(harvest);

                _toastService.ShowToast($"Garden changes successfully saved.", GardenLogToastLevel.Success);
            }

            return response;
        }

        public async Task<ApiResponse> DeleteGarden(string gardenId)
        {
            var httpClient = _httpClientFactory.CreateClient(GlobalConstants.USERMANAGEMENT_API);

            var response = await httpClient.ApiDeleteAsync(GardenRoutes.DeleteGarden.Replace("{gardenId}", gardenId));

            if (response.ValidationProblems != null)
            {
                _toastService.ShowToast($"Unable to delete a Garden. Please resolve validatione errors and try again.", GardenLogToastLevel.Error);
            }
            else if (!response.IsSuccess)
            {
                _toastService.ShowToast($"Received an invalid response: {response.ErrorMessage}", GardenLogToastLevel.Error);
            }
            else
            {
                RemoveFromGardenList(gardenId);

                _toastService.ShowToast($"Garden deleted.", GardenLogToastLevel.Success);
            }
            return response;

        }

        #endregion

        #region Private Garden functions
        //private List<GardenViewModel> GetAllGardens()
        //{
        //    return new List<GardenViewModel>() { new GardenViewModel(){
        //        GardenId = "garden1",
        //        Name = "Kravchenko's Garden",
        //        City="Minnetrista",
        //        StateCode = "MN",
        //        UserProfileId="up1",
        //        Latitude=44.97092m,
        //        Longitude=93.66728m

        //    } };
        //}
        //public Garden GetGarden()
        //{
        //    Garden garden = new Garden();
        //    garden.GardenName = "Steve's Garden";
        //    garden.BorderColor = "#585858";
        //    garden.Length = 180;
        //    garden.Width = 240;

        //    return garden;
        //}
        private async Task<List<GardenModel>> GetAllGardens()
        {
            var httpClient = _httpClientFactory.CreateClient(GlobalConstants.USERMANAGEMENT_API);

            var url = GardenRoutes.GetGardens;

            var response = await httpClient.ApiGetAsync<List<GardenModel>>(url);

            if (!response.IsSuccess)
            {
                _toastService.ShowToast("Unable to get Gardens", GardenLogToastLevel.Error);
                return new List<GardenModel>();
            }

            return response.Response;
        }

        #endregion

        private async Task AddOrUpdateToGardenList(GardenModel garden)
        {
            List<GardenModel> gardens = null;

            if (_cacheService.TryGetValue<List<GardenModel>>(GARDENS_KEY, out gardens))
            {
                var index = gardens.FindIndex(p => p.GardenId == garden.GardenId);
                if (index > -1)
                {                  
                    gardens[index] = garden;
                }
                else
                {
                    gardens.Add(garden);
                }
            }
            else
            {
                await GetGardens(true);
            }           
        }

        private void RemoveFromGardenList(string gardenId)
        {
            List<GardenModel> gardens = null;
          
            if (!_cacheService.TryGetValue<List<GardenModel>>(GARDENS_KEY, out gardens))
            {

                var index = gardens.FindIndex(p => p.GardenId == gardenId);
                if (index > -1)
                {
                    gardens.RemoveAt(index);
                }
            }

        }
    }
}
