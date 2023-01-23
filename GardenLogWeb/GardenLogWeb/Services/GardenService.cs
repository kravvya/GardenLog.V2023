using GardenLogWeb.Models;
using GardenLogWeb.Shared.Services;
using Microsoft.Extensions.Logging;


namespace GardenLogWeb.Services
{
    public interface IGardenService
    {
        Garden GetGarden();
        Task<GardenViewModel> GetGarden(string gardenId, bool useCache);
        Task<List<GardenViewModel>> GetGardens(bool forceRefresh);
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
        public Task<List<GardenViewModel>> GetGardens(bool forceRefresh)
        {
            List<GardenViewModel> gardens = null;

            if (forceRefresh || !_cacheService.TryGetValue<List<GardenViewModel>>(GARDENS_KEY, out gardens))
            {
                _logger.LogInformation("Plants not in cache or forceRefresh");

                gardens = GetAllGardens();
                // Save data in cache.
                _cacheService.Set(GARDENS_KEY, gardens, DateTime.Now.AddMinutes(CACHE_DURATION));
            }
            else
            {
                _logger.LogInformation($"Plants are in cache. Found {gardens.Count()}");
            }

            return Task.FromResult(gardens);
        }

        public Task<GardenViewModel> GetGarden(string gardenId, bool useCache)
        {
            var garden = GetAllGardens().First();
            garden.GardenId = gardenId;
            
            return Task.FromResult(garden);
        }

        public Garden GetGarden()
        {
            Garden garden = new Garden();
            garden.GardenName = "Steve's Garden";
            garden.BorderColor = "#585858";
            garden.Length = 180;
            garden.Width = 240;

            return  garden;
        }
        #endregion

        #region Private Garden functions
        private List<GardenViewModel> GetAllGardens()
        {
            return new List<GardenViewModel>() { new GardenViewModel(){
                GardenId = "garden1",
                GardenName = "Kravchenko's Garden",
                City="Minnetrista",
                StateCode = "MN",
                UserId="up1",
                Latitude=44.97092m,
                Longitude=93.66728m
                
            } };
        }

       
        #endregion
    }
}
