﻿using GardenLogWeb.Models;
using ImageCatalog.Contract.Enum;
using ImageCatalog.Contract.Queries;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;

namespace GardenLogWeb.Services;

public interface IPlantService
{
    Task<List<PlantModel>> GetPlants(bool forceRefresh);
    Task<PlantModel> GetPlant(string plantId, bool useCache);
    Task<ApiObjectResponse<string>> CreatePlant(PlantModel plant);
    Task<ApiResponse> UpdatePlant(PlantModel plant);
    Task<ApiResponse> DeletePlant(string id);


    Task<List<PlantVarietyModel>> GetAllPlantVarieties(bool useCache);
    Task<List<PlantVarietyModel>> GetPlantVarieties(string plantId, bool useCache);
    Task<PlantVarietyModel> GetPlantVariety(string plantId, string plantVarietyId);
    Task<ApiObjectResponse<string>> CreatePlantVariety(PlantVarietyModel variety);
    Task<ApiResponse> UpdatePlantVariety(PlantVarietyModel variety);
    Task<ApiResponse> DeletePlantVariety(string plantId, string id);

    Task<List<PlantGrowInstructionViewModel>> GetPlantGrowInstructions(string plantId, bool useCache);
    Task<PlantGrowInstructionViewModel> GetPlantGrowInstruction(string plantId, string growInstructionId);
    Task<ApiObjectResponse<string>> CreatePlantGrowInstruction(PlantGrowInstructionViewModel plantGrowInstruction);
    Task<ApiResponse> UpdatePlantGrowInstruction(PlantGrowInstructionViewModel plantGrowInstruction);
    Task<ApiResponse> DeletePlantGrowInstruction(string plantId, string id);

    string GetRandomPlantColor();
}

public class PlantService : IPlantService
{
    private readonly ILogger<PlantService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ICacheService _cacheService;
    private readonly IGardenLogToastService _toastService;
    private readonly IImageService _imageService;

    private Random _random = new Random();
    private const string PLANTS_KEY = "Plants";
    private const string PLANT_VARIETY_KEY = "Plant_{0}_Variety";
    private const string PLANT_GROW_INSTRUCTION_KEY = "Plant_{0}_GrowInstruction";


    public PlantService(ILogger<PlantService> logger, IHttpClientFactory clientFactory, ICacheService cacheService, IGardenLogToastService _toastService, IImageService imageService)
    {
        _logger = logger;
        _httpClientFactory = clientFactory;
        _cacheService = cacheService;
        this._toastService = _toastService;
        _imageService = imageService;
    }

    #region Public Plant Functions
    public string GetRandomPlantColor()
    {
        var color = System.Drawing.Color.FromArgb(_random.Next(256), _random.Next(256), _random.Next(256));
        return $"#{color.R.ToString("X2")}{color.G.ToString("X2")}{color.B.ToString("X2")}";
    }

    public async Task<List<PlantModel>> GetPlants(bool forceRefresh)
    {
        List<PlantModel> plants = null;

        if (forceRefresh || !_cacheService.TryGetValue<List<PlantModel>>(PLANTS_KEY, out plants))
        {
            _logger.LogInformation("Plants not in cache or forceRefresh");

            var plantsTask = GetAllPlants();

            var imagesTask = _imageService.GetImages(ImageEntityEnum.Plant, false);

            await Task.WhenAll(plantsTask, imagesTask);

            plants = plantsTask.Result;
            var images = imagesTask.Result;

            if (plants.Count > 0)
            {

                foreach (var plant in plants)
                {
                    plant.Images = images.Where(i => i.RelatedEntityId == plant.PlantId).ToList();

                    var image = plant.Images.FirstOrDefault();
                    if (image != null)
                    {
                        plant.ImageFileName = image.FileName;
                        plant.ImageLabel = image.Label;
                    }
                    else
                    {
                        plant.ImageFileName = ImageService.NO_IMAGE;
                        plant.ImageLabel = "Add image";
                    }
                }

                // Save data in cache.
                _cacheService.Set(PLANTS_KEY, plants, DateTime.Now.AddMinutes(10));
            }
        }

        else
        {
            _logger.LogInformation($"Plants are in cache. Found {plants.Count()}");
        }

        return plants;
    }

    public async Task<PlantModel> GetPlant(string plantId, bool useCache)
    {
        PlantModel plant = null;

        var plants = (await GetPlants(false));

        if (useCache)
        {
            plant = plants.FirstOrDefault(p => p.PlantId == plantId);
        }

        if (plant == null)
        {
            var httpClient = _httpClientFactory.CreateClient(GlobalConstants.PLANTCATALOG_API);

            var response = await httpClient.ApiGetAsync<PlantModel>(Routes.GetPlantById.Replace("{id}", plantId));

            if (!response.IsSuccess)
            {
                _toastService.ShowToast("Unable to get Plant details", GardenLogToastLevel.Error);
                return null;
            }

            plant = response.Response;

            await AddOrUpdateToPlantList(plant);
        }

        return plant;
    }

    public async Task<ApiObjectResponse<string>> CreatePlant(PlantModel plant)
    {
        var httpClient = _httpClientFactory.CreateClient(GlobalConstants.PLANTCATALOG_API);

        var response = await httpClient.ApiPostAsync(Routes.CreatePlant, plant);


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
            plant.PlantId = response.Response;
            await AddOrUpdateToPlantList(plant);

            _toastService.ShowToast($"Plant created. Plant id is {plant.PlantId}", GardenLogToastLevel.Success);
        }

        return response;
    }

    public async Task<ApiResponse> UpdatePlant(PlantModel plant)
    {
        var httpClient = _httpClientFactory.CreateClient(GlobalConstants.PLANTCATALOG_API);

        var response = await httpClient.ApiPutAsync(Routes.UpdatePlant.Replace("{id}", plant.PlantId), plant);

        if (response.ValidationProblems != null)
        {
            _toastService.ShowToast($"Unable to update a Plant. Please resolve validatione errors and try again.", GardenLogToastLevel.Error);
        }
        else if (!response.IsSuccess)
        {
            _toastService.ShowToast($"Received an invalid response: {response.ErrorMessage}", GardenLogToastLevel.Error);
        }
        else
        {
            await AddOrUpdateToPlantList(plant);

            _toastService.ShowToast($"{plant.Name} is successfully updated.", GardenLogToastLevel.Success);
        }

        return response;
    }

    public async Task<ApiResponse> DeletePlant(string id)
    {
        var httpClient = _httpClientFactory.CreateClient(GlobalConstants.PLANTCATALOG_API);

        var response = await httpClient.ApiDeleteAsync(Routes.DeletePlant.Replace("{id}", id));

        if (response.ValidationProblems != null)
        {
            _toastService.ShowToast($"Unable to delete a Plant. Please resolve validatione errors and try again.", GardenLogToastLevel.Error);
        }
        else if (!response.IsSuccess)
        {
            _toastService.ShowToast($"Received an invalid response: {response.ErrorMessage}", GardenLogToastLevel.Error);
        }
        else
        {
            await RemoveFromPlantList(id);

            _toastService.ShowToast($"Plant deleted. Plant id was {id}", GardenLogToastLevel.Success);
        }
        return response;
    }

    #endregion

    #region Public Plant Variety Functions
    public async Task<List<PlantVarietyModel>> GetAllPlantVarieties(bool useCache)
    {
        List<PlantVarietyModel> plantVarietyList = null;

        if (useCache)
        {
            plantVarietyList = GetPlantVarietiesFromCache(string.Empty);
        }

        if (plantVarietyList == null)
        {
            plantVarietyList = await GetPlantVarieties();

            if (plantVarietyList.Count > 0)
            {
                List<GetImagesByRelatedEntity> relatedEntities = new();
                foreach (var variety in plantVarietyList)
                {
                    relatedEntities.Add(new GetImagesByRelatedEntity(ImageEntityEnum.PlantVariety, variety.PlantVarietyId, false));
                }
                var images = await _imageService.GetImagesInBulk(relatedEntities);

                foreach (var variety in plantVarietyList)
                {
                    variety.Images = images.Where(i => i.RelatedEntityId == variety.PlantVarietyId).ToList();

                    var image = variety.Images.FirstOrDefault();
                    if (image != null)
                    {
                        variety.ImageFileName = image.FileName;
                        variety.ImageLabel = image.Label;
                    }
                    else
                    {
                        variety.ImageFileName = ImageService.NO_IMAGE;
                        variety.ImageLabel = "Add image";
                    }
                }
            }

            AddPlantVarietiesToCache(string.Empty, plantVarietyList);
        }

        return plantVarietyList;
    }

    public async Task<List<PlantVarietyModel>> GetPlantVarieties(string plantId, bool useCache)
    {
        List<PlantVarietyModel> plantVarietyList = null;

        if (useCache)
        {
            plantVarietyList = GetPlantVarietiesFromCache(plantId);
        }

        if (plantVarietyList == null)
        {
            plantVarietyList = await GetPlantVarieties(plantId);

            if (plantVarietyList.Count > 0)
            {
                List<GetImagesByRelatedEntity> relatedEntities = new();
                foreach (var variety in plantVarietyList)
                {
                    relatedEntities.Add(new GetImagesByRelatedEntity(ImageEntityEnum.PlantVariety, variety.PlantVarietyId, false));
                }
                var images = await _imageService.GetImagesInBulk(relatedEntities);

                foreach (var variety in plantVarietyList)
                {
                    variety.Images = images.Where(i => i.RelatedEntityId == variety.PlantVarietyId).ToList();

                    var image = variety.Images.FirstOrDefault();
                    if (image != null)
                    {
                        variety.ImageFileName = image.FileName;
                        variety.ImageLabel = image.Label;
                    }
                    else
                    {
                        variety.ImageFileName = ImageService.NO_IMAGE;
                        variety.ImageLabel = "Add image";
                    }
                }
            }

            AddPlantVarietiesToCache(plantId, plantVarietyList);
        }

        return plantVarietyList;
    }

    public async Task<PlantVarietyModel> GetPlantVariety(string plantId, string plantVerietyId)
    {
        PlantVarietyModel plantVariety = null;

        var plantVarietyTask = GetPlantVarietyFromServer(plantId, plantVerietyId);
        var imagesTask = _imageService.GetImages(ImageEntityEnum.PlantVariety, plantVerietyId, false);

        await Task.WhenAll(plantVarietyTask, imagesTask);

        plantVariety = plantVarietyTask.Result;
        var images = imagesTask.Result;

        plantVariety.Images = images;
        var image = plantVariety.Images.FirstOrDefault();
        if (image != null)
        {
            plantVariety.ImageFileName = image.FileName;
            plantVariety.ImageLabel = image.Label;
        }
        else
        {
            plantVariety.ImageFileName = ImageService.NO_IMAGE;
            plantVariety.ImageLabel = "Add image";
            plantVariety.Images = new();
        }

        AddOrUpdateToPlantVarietyList(plantVariety);

        return plantVariety;
    }

    public async Task<ApiObjectResponse<string>> CreatePlantVariety(PlantVarietyModel variety)
    {
        var httpClient = _httpClientFactory.CreateClient(GlobalConstants.PLANTCATALOG_API);

        var response = await httpClient.ApiPostAsync(Routes.CreatePlantVariety.Replace("{plantId}", variety.PlantId), variety);

        if (response.ValidationProblems != null)
        {
            _toastService.ShowToast($"Unable to create a Plant Variety. Please resolve validatione errors and try again.", GardenLogToastLevel.Error);
        }
        else if (!response.IsSuccess)
        {
            _toastService.ShowToast($"Received an invalid response from Plant Variety Post: {response.ErrorMessage}", GardenLogToastLevel.Error);
        }
        else
        {
            variety.PlantVarietyId = response.Response;
            AddOrUpdateToPlantVarietyList(variety);
            IncrementVarietyCountInCache(variety.PlantId);
            _toastService.ShowToast($"Plant Variety created. Plant Variety id is {variety.PlantVarietyId}", GardenLogToastLevel.Success);
        }

        return response;
    }

    public async Task<ApiResponse> UpdatePlantVariety(PlantVarietyModel variety)
    {
        var httpClient = _httpClientFactory.CreateClient(GlobalConstants.PLANTCATALOG_API);

        string route = Routes.UpdatePlantVariety.Replace("{plantId}", variety.PlantId).Replace("{id}", variety.PlantVarietyId);

        var response = await httpClient.ApiPutAsync(route, variety);

        if (response.ValidationProblems != null)
        {
            _toastService.ShowToast($"Unable to update a Plant Variety. Please resolve validation errors and try again.", GardenLogToastLevel.Error);
        }
        else if (!response.IsSuccess)
        {
            _toastService.ShowToast($"Received an invalid response: {response.ErrorMessage}", GardenLogToastLevel.Error);
        }
        else
        {
            AddOrUpdateToPlantVarietyList(variety);

            _toastService.ShowToast($"{variety.Name} is successfully updated.", GardenLogToastLevel.Success);
        }

        return response;
    }

    public async Task<ApiResponse> DeletePlantVariety(string plantId, string id)
    {
        var httpClient = _httpClientFactory.CreateClient(GlobalConstants.PLANTCATALOG_API);

        string route = Routes.DeletePlantVariety.Replace("{plantId}", plantId).Replace("{id}", id);

        var response = await httpClient.ApiDeleteAsync(route);

        if (response.ValidationProblems != null)
        {
            _toastService.ShowToast($"Unable to delete a Variety. Please resolve validatione errors and try again.", GardenLogToastLevel.Error);
        }
        else if (!response.IsSuccess)
        {
            _toastService.ShowToast($"Received an invalid response: {response.ErrorMessage}", GardenLogToastLevel.Error);
        }
        else
        {
            RemoveFromPlantVarietyList(plantId, id);

            _toastService.ShowToast($"Variety deleted. Variety id was {id}", GardenLogToastLevel.Success);
        }
        return response;
    }
    #endregion

    #region Public Plant Grow Instruction Functions

    public async Task<List<PlantGrowInstructionViewModel>> GetPlantGrowInstructions(string plantId, bool useCache)
    {
        List<PlantGrowInstructionViewModel> plantGrowInstructionsList = null;

        string cacheKey = string.Format(PLANT_GROW_INSTRUCTION_KEY, plantId);

        if (useCache)
        {
            _cacheService.TryGetValue<List<PlantGrowInstructionViewModel>>(cacheKey, out plantGrowInstructionsList);
        }

        if (plantGrowInstructionsList == null)
        {
            var httpClient = _httpClientFactory.CreateClient(GlobalConstants.PLANTCATALOG_API);
            var url = Routes.GetPlantGrowInstructions.Replace("{plantId}", plantId);

            var response = await httpClient.ApiGetAsync<List<PlantGrowInstructionViewModel>>(url);

            if (!response.IsSuccess)
            {
                _toastService.ShowToast("Unable to get Plant Grow Instructions", GardenLogToastLevel.Error);
                return null;
            }

            plantGrowInstructionsList = response.Response;

            AddPlanGrowInstructionsToCache(plantId, plantGrowInstructionsList);
        }

        return plantGrowInstructionsList;
    }

    public async Task<PlantGrowInstructionViewModel> GetPlantGrowInstruction(string plantId, string growInstructionId)
    {
        PlantGrowInstructionViewModel growInstruction = null;

        string route = Routes.GetPlantGrowInstruction.Replace("{plantId}", plantId).Replace("{id}", growInstructionId);

        var httpClient = _httpClientFactory.CreateClient(GlobalConstants.PLANTCATALOG_API);

        var response = await httpClient.ApiGetAsync<PlantGrowInstructionViewModel>(route);

        if (!response.IsSuccess)
        {
            _toastService.ShowToast("Unable to get Plant Grow Instruction", GardenLogToastLevel.Error);
            return null;
        }

        growInstruction = response.Response;

        await AddOrUpdateToPlantGrowInstructionList(growInstruction);

        return growInstruction;
    }

    public async Task<ApiObjectResponse<string>> CreatePlantGrowInstruction(PlantGrowInstructionViewModel plantGrowInstruction)
    {
        var httpClient = _httpClientFactory.CreateClient(GlobalConstants.PLANTCATALOG_API);

        var route = Routes.CreatePlantGrowInstruction.Replace("{plantId}", plantGrowInstruction.PlantId);
        var response = await httpClient.ApiPostAsync(route, plantGrowInstruction);


        if (response.ValidationProblems != null)
        {
            _toastService.ShowToast($"Unable to add Grow Instructions. Please resolve validatione errors and try again.", GardenLogToastLevel.Error);
        }
        else if (!response.IsSuccess)
        {
            _toastService.ShowToast($"Received an invalid response from Grow Instruction Post: {response.ErrorMessage}", GardenLogToastLevel.Error);
        }
        else
        {
            plantGrowInstruction.PlantGrowInstructionId = response.Response;
            await AddOrUpdateToPlantGrowInstructionList(plantGrowInstruction);
            _toastService.ShowToast($"Grow Instructions added. Instruction id is {plantGrowInstruction.PlantGrowInstructionId}", GardenLogToastLevel.Success);
        }

        return response;
    }

    public async Task<ApiResponse> UpdatePlantGrowInstruction(PlantGrowInstructionViewModel plantGrowInstruction)
    {
        var httpClient = _httpClientFactory.CreateClient(GlobalConstants.PLANTCATALOG_API);

        string route = Routes.UpdatePlantGrowInstructions.Replace("{plantId}", plantGrowInstruction.PlantId).Replace("{id}", plantGrowInstruction.PlantGrowInstructionId);

        var response = await httpClient.ApiPutAsync(route, plantGrowInstruction);

        if (response.ValidationProblems != null)
        {
            _toastService.ShowToast($"Unable to update a Plant Grow Instruction. Please resolve validation errors and try again.", GardenLogToastLevel.Error);
        }
        else if (!response.IsSuccess)
        {
            _toastService.ShowToast($"Received an invalid response: {response.ErrorMessage}", GardenLogToastLevel.Error);
        }
        else
        {
            await AddOrUpdateToPlantGrowInstructionList(plantGrowInstruction);
            IncrementGrowCountInCache(plantGrowInstruction.PlantId);
            _toastService.ShowToast($"{plantGrowInstruction.Name} is successfully updated.", GardenLogToastLevel.Success);
        }

        return response;
    }

    public async Task<ApiResponse> DeletePlantGrowInstruction(string plantId, string id)
    {
        var httpClient = _httpClientFactory.CreateClient(GlobalConstants.PLANTCATALOG_API);

        string route = Routes.DeletePlantGrowInstructions.Replace("{plantId}", plantId).Replace("{id}", id);

        var response = await httpClient.ApiDeleteAsync(route);

        if (response.ValidationProblems != null)
        {
            _toastService.ShowToast($"Unable to delete a Instructions. Please resolve validation errors and try again.", GardenLogToastLevel.Error);
        }
        else if (!response.IsSuccess)
        {
            _toastService.ShowToast($"Received an invalid response: {response.ErrorMessage}", GardenLogToastLevel.Error);
        }
        else
        {
            RemoveFromPlanGrowInstructionList(plantId, id);

            _toastService.ShowToast($"Instructions removed. Instruction id was {id}", GardenLogToastLevel.Success);
        }
        return response;
    }
    #endregion

    #region Private Plant Functions
    private async Task<List<PlantModel>> GetAllPlants()
    {
        var httpClient = _httpClientFactory.CreateClient(GlobalConstants.PLANTCATALOG_API);

        var response = await httpClient.ApiGetAsync<List<PlantModel>>(Routes.GetAllPlants);

        if (!response.IsSuccess)
        {
            _toastService.ShowToast("Unable to get Plants", GardenLogToastLevel.Error);
            return null;
        }

        return response.Response;
    }

    private async Task AddOrUpdateToPlantList(PlantModel plant)
    {
        var plants = (await GetPlants(false));

        var index = plants.FindIndex(p => p.PlantId == plant.PlantId);
        if (index > -1)
        {
            plant.Images = plants[index].Images;
            plant.ImageFileName = plants[index].ImageFileName;
            plant.ImageLabel = plants[index].ImageLabel;
            plants[index] = plant;
        }
        else
        {
            plants.Add(plant);
        }
    }

    private async Task RemoveFromPlantList(string plantId)
    {
        var plants = (await GetPlants(false));

        var index = plants.FindIndex(p => p.PlantId == plantId);
        if (index > -1)
        {
            plants.RemoveAt(index);
        }
    }

    private void IncrementVarietyCountInCache(string plantId)
    {
        List<PlantModel> plants = null;

        if (_cacheService.TryGetValue<List<PlantModel>>(PLANTS_KEY, out plants))
        {
            var plant = plants.Where(p => p.PlantId == plantId).FirstOrDefault();
            if (plant != null) plant.VarietyCount++;
        }
    }

    private void IncrementGrowCountInCache(string plantId)
    {
        List<PlantModel> plants = null;

        if (_cacheService.TryGetValue<List<PlantModel>>(PLANTS_KEY, out plants))
        {
            var plant = plants.Where(p => p.PlantId == plantId).FirstOrDefault();
            if (plant != null) plant.GrowInstructionsCount++;
        }
    }
    #endregion

    #region Private Plant Variety Fucntions
    private async Task<List<PlantVarietyModel>> GetPlantVarieties()
    {
        var httpClient = _httpClientFactory.CreateClient(GlobalConstants.PLANTCATALOG_API);

        var response = await httpClient.ApiGetAsync<List<PlantVarietyModel>>(Routes.GetAllPlantVarieties);

        if (!response.IsSuccess)
        {
            _toastService.ShowToast("Unable to get Plant Varieties", GardenLogToastLevel.Error);
            return null;
        }

        return response.Response;
    }

    private async Task<List<PlantVarietyModel>> GetPlantVarieties(string plantId)
    {
        var httpClient = _httpClientFactory.CreateClient(GlobalConstants.PLANTCATALOG_API);

        var response = await httpClient.ApiGetAsync<List<PlantVarietyModel>>(Routes.GetAllPlantVarieties.Replace("{plantId}", plantId));

        if (!response.IsSuccess)
        {
            _toastService.ShowToast("Unable to get Plant Varieties", GardenLogToastLevel.Error);
            return null;
        }

        return response.Response;
    }

    private async Task<PlantVarietyModel> GetPlantVarietyFromServer(string plantId, string plantVerietyId)
    {
        var httpClient = _httpClientFactory.CreateClient(GlobalConstants.PLANTCATALOG_API);

        var response = await httpClient.ApiGetAsync<PlantVarietyModel>(Routes.GetPlantVariety.Replace("{plantId}", plantId).Replace("{id}", plantVerietyId));

        if (!response.IsSuccess)
        {
            _toastService.ShowToast("Unable to get Plant variety", GardenLogToastLevel.Error);
            return null;
        }

        return response.Response;
    }


    private void AddOrUpdateToPlantVarietyList(PlantVarietyModel variety)
    {
        var varieties = GetPlantVarietiesFromCache(variety.PlantId);

        if (varieties == null) return;

        var index = varieties.FindIndex(p => p.PlantVarietyId == variety.PlantVarietyId);
        if (index > -1)
        {
            varieties[index] = variety;
        }
        else
        {
            varieties.Add(variety);
        }
    }

    private List<PlantVarietyModel> GetPlantVarietiesFromCache(string plantId)
    {
        string cacheKey = string.Format(PLANT_VARIETY_KEY, plantId);

        List<PlantVarietyModel> plantVarietyList = null;

        _cacheService.TryGetValue<List<PlantVarietyModel>>(cacheKey, out plantVarietyList);

        return plantVarietyList;
    }

    private void AddPlantVarietiesToCache(string plantId, List<PlantVarietyModel> plantVarietyList)
    {
        string cacheKey = string.Format(PLANT_VARIETY_KEY, plantId);

        _cacheService.Set(cacheKey, plantVarietyList, DateTime.Now.AddMinutes(10));
    }

    private void RemoveFromPlantVarietyList(string plantId, string plantVarietyId)
    {
        var plantVarieties = GetPlantVarietiesFromCache(plantId);

        var index = plantVarieties.FindIndex(p => p.PlantVarietyId == plantVarietyId);
        if (index > -1)
        {
            plantVarieties.RemoveAt(index);
        }
    }
    #endregion

    #region Private Plant Grow Instructions

    private async Task AddOrUpdateToPlantGrowInstructionList(PlantGrowInstructionViewModel growInstructionModel)
    {
        var plantGrowInstructions = await GetPlantGrowInstructions(growInstructionModel.PlantId, true);

        if (plantGrowInstructions == null) return;

        var index = plantGrowInstructions.FindIndex(p => p.PlantGrowInstructionId == growInstructionModel.PlantGrowInstructionId);
        if (index > -1)
        {
            plantGrowInstructions[index] = growInstructionModel;
        }
        else
        {
            plantGrowInstructions.Add(growInstructionModel);
        }
    }

    private List<PlantGrowInstructionViewModel> GetPlantGrowInstructionsFromCache(string plantId)
    {
        string cacheKey = string.Format(PLANT_GROW_INSTRUCTION_KEY, plantId);

        List<PlantGrowInstructionViewModel> plantGrowInstructions = null;

        _cacheService.TryGetValue<List<PlantGrowInstructionViewModel>>(cacheKey, out plantGrowInstructions);

        return plantGrowInstructions;
    }

    private void AddPlanGrowInstructionsToCache(string plantId, List<PlantGrowInstructionViewModel> plantGrowInstructions)
    {
        string cacheKey = string.Format(PLANT_GROW_INSTRUCTION_KEY, plantId);

        _cacheService.Set(cacheKey, plantGrowInstructions, DateTime.Now.AddMinutes(10));
    }

    private void RemoveFromPlanGrowInstructionList(string plantId, string plantGrowInstructionId)
    {
        var growInstructions = GetPlantGrowInstructionsFromCache(plantId);

        var index = growInstructions.FindIndex(p => p.GrowingInstructions == plantGrowInstructionId);
        if (index > -1)
        {
            growInstructions.RemoveAt(index);
        }
    }

    #endregion

}