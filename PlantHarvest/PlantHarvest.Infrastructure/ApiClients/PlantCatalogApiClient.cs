﻿using Microsoft.Extensions.Caching.Memory;

namespace PlantHarvest.Infrastructure.ApiClients;

public interface IPlantCatalogApiClient
{
    Task<PlantGrowInstructionViewModel> GetPlantGrowInstruction(string plantId, string growInstructionId);
}

public class PlantCatalogApiClient : IPlantCatalogApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PlantCatalogApiClient> _logger;
    private readonly IMemoryCache _cache;

    private const string GROW_CACHE_KEY = "Plant:{0}Grow:{1}";
    private const int CACHE_DURATION = 60;

    public PlantCatalogApiClient(HttpClient httpClient, IConfiguration confguration, ILogger<PlantCatalogApiClient> logger, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _logger = logger;
        _cache = cache;
        var plantUrl = confguration["Services:PlantCatalog.Api"];
        _logger.LogInformation($"Plant URL @ {plantUrl}");

        _httpClient.BaseAddress = new Uri(plantUrl);
    }

    public async Task<PlantGrowInstructionViewModel> GetPlantGrowInstruction(string plantId, string growInstructionId)
    {
        PlantGrowInstructionViewModel growInstruction = null;
        string key = string.Format(GROW_CACHE_KEY, plantId, growInstructionId);

        if (!_cache.TryGetValue(key, out growInstruction))
        {
            string route = Routes.GetPlantGrowInstruction.Replace("{plantId}", plantId).Replace("{id}", growInstructionId);

            var response = await _httpClient.ApiGetAsync<PlantGrowInstructionViewModel>(route);

            if (!response.IsSuccess)
            {
                _logger.LogError($"Unable to get Plant Grow Instruction for plantId: {plantId} and growId {growInstructionId}");
                return null;
            }

            growInstruction = response.Response;
            _cache.Set(key, growInstruction, new MemoryCacheEntryOptions()
            {
                SlidingExpiration = TimeSpan.FromMinutes(CACHE_DURATION)
            });
        }



        return growInstruction;
    }
}
