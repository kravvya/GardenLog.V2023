using System.Net.Http;
using System;

namespace GardenLogWeb.Services;

public interface IHarvestCycleService
{
    Task<IList<HarvestCycleModel>> GetHarvestList(bool forceRefresh);
    Task<HarvestCycleModel> GetHarvestCycle(string harvestCycleId, bool useCache);
    Task<ApiObjectResponse<string>> CreateHarvest(HarvestCycleModel harvest);
    Task<ApiResponse> UpdateHarvest(HarvestCycleModel harvest);
    Task<ApiResponse> DeleteHarvest(string id);
}

public class HarvestCycleService : IHarvestCycleService
{
    private readonly ILogger<PlantService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ICacheService _cacheService;
    private readonly IGardenLogToastService _toastService;
    private readonly IImageService _imageService;

    private const int CACHE_DURATION = 10;
    private const string HARVESTS_KEY = "HarvestCycles";

    public HarvestCycleService(ILogger<PlantService> logger, IHttpClientFactory clientFactory, ICacheService cacheService, IGardenLogToastService toastService, IImageService imageService)
    {
        _logger = logger;
        _httpClientFactory = clientFactory;
        _cacheService = cacheService;
        _toastService = toastService;
        _imageService = imageService;
    }

    #region Public Harvest Cycle Functions

    public async Task<IList<HarvestCycleModel>> GetHarvestList(bool forceRefresh)
    {
        IList<HarvestCycleModel> harvests;

        if (forceRefresh || !_cacheService.TryGetValue<IList<HarvestCycleModel>>(HARVESTS_KEY, out harvests))
        {
            _logger.LogInformation("Harvests not in cache or forceRefresh");
            harvests = await GetAllHarvests();
            if (harvests.Count > 0)
            {
                // Save data in cache.
                _cacheService.Set(HARVESTS_KEY, harvests, DateTime.Now.AddMinutes(CACHE_DURATION));
            }
        }
        else
        {
           _logger.LogInformation($"Harvests are in cache. Found {harvests.Count()}");
        }

        return harvests;
    }

    public async Task<HarvestCycleModel> GetHarvestCycle(string harvestCycleId, bool useCache)
    {
        HarvestCycleModel? harvest = null;


        if (useCache)
        {
            if (_cacheService.TryGetValue<IList<HarvestCycleModel>>(HARVESTS_KEY, out var harvests))
            {
                harvest = harvests?.FirstOrDefault(p => p.HarvestCycleId == harvestCycleId);
            }
        }

        if (harvest == null)
        {
            var httpClient = _httpClientFactory.CreateClient(GlobalConstants.PLANTHARVEST_API);

            var response = await httpClient.ApiGetAsync<HarvestCycleModel>(HarvestRoutes.GetHarvestCycleById.Replace("{id}", harvestCycleId));

            if (!response.IsSuccess)
            {
                _toastService.ShowToast("Unable to get Harvest Cycle details", GardenLogToastLevel.Error);
                return new HarvestCycleModel();
            }

            harvest = response.Response;

            AddOrUpdateToHarvestCycleList(harvest);
        }

        return harvest;
    }

    public async Task<ApiObjectResponse<string>> CreateHarvest(HarvestCycleModel harvest)
    {
        var httpClient = _httpClientFactory.CreateClient(GlobalConstants.PLANTHARVEST_API);

        var response = await httpClient.ApiPostAsync(HarvestRoutes.CreateHarvestCycle, harvest);

        if (response.ValidationProblems != null)
        {
            _toastService.ShowToast($"Unable to create a Garden Plan. Please resolve validatione errors and try again.", GardenLogToastLevel.Error);
        }
        else if (!response.IsSuccess)
        {
            _toastService.ShowToast($"Received an invalid response from Garden Plan Post: {response.ErrorMessage}", GardenLogToastLevel.Error);
        }
        else
        {
            harvest.HarvestCycleId = response.Response;

            AddOrUpdateToHarvestCycleList(harvest);

            _toastService.ShowToast($"Garden Plan {harvest.HarvestCycleName} is created", GardenLogToastLevel.Success);
        }

        return response;
    }

    public async Task<ApiResponse> UpdateHarvest(HarvestCycleModel harvest)
    {
        var httpClient = _httpClientFactory.CreateClient(GlobalConstants.PLANTHARVEST_API);

        var response = await httpClient.ApiPutAsync(HarvestRoutes.UpdateHarvestCycle.Replace("{id}", harvest.HarvestCycleId), harvest);

        if (response.ValidationProblems != null)
        {
            _toastService.ShowToast($"Unable to update a Garden Plan. Please resolve validatione errors and try again.", GardenLogToastLevel.Error);
        }
        else if (!response.IsSuccess)
        {
            _toastService.ShowToast($"Received an invalid response: {response.ErrorMessage}", GardenLogToastLevel.Error);
        }
        else
        {
            AddOrUpdateToHarvestCycleList(harvest);

            _toastService.ShowToast($"{harvest.HarvestCycleName} is successfully updated.", GardenLogToastLevel.Success);
        }

        return response;
    }

    public async Task<ApiResponse> DeleteHarvest(string id)
    {
        var httpClient = _httpClientFactory.CreateClient(GlobalConstants.PLANTHARVEST_API);

        var response = await httpClient.ApiDeleteAsync(HarvestRoutes.UpdateHarvestCycle.Replace("{id}", id));

        if (response.ValidationProblems != null)
        {
            _toastService.ShowToast($"Unable to delete a Garden Plan. Please resolve validatione errors and try again.", GardenLogToastLevel.Error);
        }
        else if (!response.IsSuccess)
        {
            _toastService.ShowToast($"Received an invalid response: {response.ErrorMessage}", GardenLogToastLevel.Error);
        }
        else
        {
             RemoveFromHarvestCycleList(id);

            _toastService.ShowToast($"Garden Plan deleted.", GardenLogToastLevel.Success);
        }
        return response;
    }
    #endregion


    #region Private Harvest Cycle Functions
    private async Task<List<HarvestCycleModel>> GetAllHarvests()
    {
        var httpClient = _httpClientFactory.CreateClient(GlobalConstants.PLANTHARVEST_API);

        var response = await httpClient.ApiGetAsync<List<HarvestCycleModel>>(HarvestRoutes.GetAllHarvestCycles);

        if (!response.IsSuccess)
        {
            _toastService.ShowToast("Unable to get Garden Plans", GardenLogToastLevel.Error);
            return new List<HarvestCycleModel>();
        }

        return response.Response;
    }

    private void AddOrUpdateToHarvestCycleList(HarvestCycleModel harvest)
    {
        List<HarvestCycleModel>? harvests = null;

        if (_cacheService.TryGetValue<List<HarvestCycleModel>>(HARVESTS_KEY, out harvests))
        {
            var index = harvests.FindIndex(p => p.HarvestCycleId == harvest.HarvestCycleId);
            if (index > -1)
            {
                harvests[index] = harvest;
            }
            return;
        }
        else
        {
            harvests = new List<HarvestCycleModel>();
        }
        harvests.Add(harvest);

        _cacheService.Set(HARVESTS_KEY, harvests, DateTime.Now.AddMinutes(CACHE_DURATION));
    }

    private void RemoveFromHarvestCycleList(string harvestId)
    {
        if (_cacheService.TryGetValue<List<HarvestCycleModel>>(HARVESTS_KEY, out var harvests))
        {
            var index = harvests.FindIndex(p => p.HarvestCycleId == harvestId);
            if (index > -1)
            {
                harvests.RemoveAt(index);
            }
        }
    }
    #endregion
}
