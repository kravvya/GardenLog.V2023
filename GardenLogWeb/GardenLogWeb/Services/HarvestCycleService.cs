using System.Net.Http;
using System;

namespace GardenLogWeb.Services;

public interface IHarvestCycleService
{
    Task<IList<HarvestCycleModel>> GetHarvestList(bool forceRefresh);
    Task<HarvestCycleModel> GetHarvest(string harvestCycleId, bool useCache);
    Task<ApiObjectResponse<string>> CreateHarvest(HarvestCycleModel harvest);
    Task<ApiResponse> UpdateHarvest(HarvestCycleModel harvest);
    Task<ApiResponse> DeleteHarvest(string id);
    Task<List<PlantHarvestCycleModel>> GetPlantHarvests(string harvestCycleId, bool forceRefresh);
    Task<ApiObjectResponse<string>> CreatePlantHarvest(PlantHarvestCycleModel plant);
    Task<ApiResponse> UpdatePlantHarvest(PlantHarvestCycleModel plant);
    Task<ApiResponse> DeletePlantHarvest(string harvestId, string id);
}

public class HarvestCycleService : IHarvestCycleService
{
    private readonly ILogger<PlantService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ICacheService _cacheService;
    private readonly IGardenLogToastService _toastService;
    private readonly IImageService _imageService;
    private readonly IGardenService _gardenService;
    private const int CACHE_DURATION = 10;
    private const string HARVESTS_KEY = "HarvestCycles";
    private const string PLANT_HARVESTS_KEY = "PlantHarvestCycles_{0}";

    public HarvestCycleService(ILogger<PlantService> logger, IHttpClientFactory clientFactory, ICacheService cacheService, IGardenLogToastService toastService, IImageService imageService, IGardenService gardenService)
    {
        _logger = logger;
        _httpClientFactory = clientFactory;
        _cacheService = cacheService;
        _toastService = toastService;
        _imageService = imageService;
        _gardenService = gardenService;
    }

    #region Public Harvest Cycle Functions

    public async Task<IList<HarvestCycleModel>> GetHarvestList(bool forceRefresh)
    {
        IList<HarvestCycleModel> harvests;

        if (forceRefresh || !_cacheService.TryGetValue<IList<HarvestCycleModel>>(HARVESTS_KEY, out harvests))
        {
            _logger.LogInformation("Harvests not in cache or forceRefresh");

            var harvestTask = GetAllHarvests();
            var gardenTask = _gardenService.GetGardens(false);

            await Task.WhenAll(harvestTask, gardenTask);

            harvests = harvestTask.Result;
            var gardens = gardenTask.Result;

            if (harvests.Count > 0)
            {
                foreach (var harvest in harvests)
                {
                    harvest.GardenName = gardens.FirstOrDefault(g => g.GardenId == harvest.GardenId)?.GardenName;
                }
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

    public async Task<HarvestCycleModel> GetHarvest(string harvestCycleId, bool useCache)
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
            harvest.GardenName = (await _gardenService.GetGarden(harvest.GardenId, true))?.GardenName;

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

    #region Public Plant Harvest Cycle Functions
    public async Task<List<PlantHarvestCycleModel>> GetPlantHarvests(string harvestCycleId, bool forceRefresh)
    {
        List<PlantHarvestCycleModel> plants;
        string key = string.Format(PLANT_HARVESTS_KEY, harvestCycleId);

        if (forceRefresh || !_cacheService.TryGetValue<List<PlantHarvestCycleModel>>(key, out plants))
        {
            System.Console.WriteLine($"PlantHarvestCycles for {harvestCycleId} not in cache or forceRefresh");

            var httpClient = _httpClientFactory.CreateClient(GlobalConstants.PLANTHARVEST_API);

            var response = await httpClient.ApiGetAsync<List<PlantHarvestCycleModel>>(HarvestRoutes.GetPlanHarvestCycles.Replace("{harvestId}", harvestCycleId));

            if (!response.IsSuccess)
            {
                _toastService.ShowToast("Unable to get Garden Plan deatils ", GardenLogToastLevel.Error);
                return new List<PlantHarvestCycleModel>();
            }

            plants = response.Response;

            if (plants.Count > 0)
            {
                // Save data in cache.
                _cacheService.Set(key, plants, DateTime.Now.AddMinutes(CACHE_DURATION));
            }
        }
        else
        {
            System.Console.WriteLine($"PlantHarvestCycles for {harvestCycleId} are in cache. Found {plants.Count()}");
        }

        return plants;
    }

    public async Task<ApiObjectResponse<string>> CreatePlantHarvest(PlantHarvestCycleModel plant)
    {
        var httpClient = _httpClientFactory.CreateClient(GlobalConstants.PLANTHARVEST_API);

        var response = await httpClient.ApiPostAsync(HarvestRoutes.CreatePlantHarvestCycle, plant);

        if (response.ValidationProblems != null)
        {
            _toastService.ShowToast($"Unable to add Plant to Garden Plan. Please resolve validatione errors and try again.", GardenLogToastLevel.Error);
        }
        else if (!response.IsSuccess)
        {
            _toastService.ShowToast($"Received an invalid response from Garden Plan Post: {response.ErrorMessage}", GardenLogToastLevel.Error);
        }
        else
        {
            plant.PlantHarvestCycleId = response.Response;

            AddOrUpdateToPlantHarvestCycleList(plant);

            _toastService.ShowToast($"Plant is added to the Garden Plan", GardenLogToastLevel.Success);
        }

        return response;
    }

    public async Task<ApiResponse> UpdatePlantHarvest(PlantHarvestCycleModel plant)
    {
        var httpClient = _httpClientFactory.CreateClient(GlobalConstants.PLANTHARVEST_API);

        var response = await httpClient.ApiPutAsync(HarvestRoutes.UpdatePlanHarvestCycle.Replace("{harvestId}", plant.HarvestCycleId).Replace("{id}", plant.PlantHarvestCycleId), plant);

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
            AddOrUpdateToPlantHarvestCycleList(plant);

            _toastService.ShowToast($"Garden Plan is successfully updated.", GardenLogToastLevel.Success);
        }

        return response;
    }

    public async Task<ApiResponse> DeletePlantHarvest(string harvestId, string id)
    {
        var httpClient = _httpClientFactory.CreateClient(GlobalConstants.PLANTHARVEST_API);

        var response = await httpClient.ApiDeleteAsync(HarvestRoutes.DeletePlantHarvestCycle.Replace("{harvestId}", harvestId).Replace("{id}", id));

        if (response.ValidationProblems != null)
        {
            _toastService.ShowToast($"Unable to change Garden Plan. Please resolve validatione errors and try again.", GardenLogToastLevel.Error);
        }
        else if (!response.IsSuccess)
        {
            _toastService.ShowToast($"Received an invalid response: {response.ErrorMessage}", GardenLogToastLevel.Error);
        }
        else
        {
            RemoveFromPlantHarvestCycleList(harvestId, id);

            _toastService.ShowToast($"Garden Plan changed.", GardenLogToastLevel.Success);
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
                return;
            }
        }
        else
        {
            harvests = new List<HarvestCycleModel>();
            _cacheService.Set(HARVESTS_KEY, harvests, DateTime.Now.AddMinutes(CACHE_DURATION));
        }
        harvests.Add(harvest);

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

    #region Private Plant Harvest Cycle
    private void AddOrUpdateToPlantHarvestCycleList(PlantHarvestCycleModel plant)
    {
        List<PlantHarvestCycleModel>? plants = null;
        string key = string.Format(PLANT_HARVESTS_KEY, plant.HarvestCycleId);

        if (_cacheService.TryGetValue<List<PlantHarvestCycleModel>>(key, out plants))
        {
            var index = plants.FindIndex(p => p.PlantHarvestCycleId == plant.PlantHarvestCycleId);
            if (index > -1)
            {
                plants[index] = plant;
                return;
            }
        }
        else
        {
            plants = new List<PlantHarvestCycleModel>();
            _cacheService.Set(key, plants, DateTime.Now.AddMinutes(CACHE_DURATION));
        }
        plants.Add(plant);

    }
    private void RemoveFromPlantHarvestCycleList(string harvestId, string id)
    {
        string key = string.Format(PLANT_HARVESTS_KEY, harvestId);
        if (_cacheService.TryGetValue<List<PlantHarvestCycleModel>>(key, out var plants))
        {
            var index = plants.FindIndex(p => p.PlantHarvestCycleId == id);
            if (index > -1)
            {
                plants.RemoveAt(index);
            }
        }
    }
    #endregion

}
