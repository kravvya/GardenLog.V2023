using GardenLog.SharedInfrastructure;
using GardenLog.SharedInfrastructure.MongoDB;
using GardenLog.SharedKernel;
using GardenLog.SharedKernel.Enum;
using GardenLog.SharedKernel.Interfaces;
using GardenLogAdminConsole.Plants;
using ImageCatalog.Api.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PlantCatalog.Infrustructure.Data.Repositories;

namespace GardenLogAdminConsole.Images;


internal class AdminImageProcessor
{
    internal  async Task<int> UpdateAllImagesWithRelatedEntities(IConfiguration config, ILoggerFactory loggerFactory)
    {
        IConfigurationService configurationService = new ConfigurationService(config, loggerFactory.CreateLogger<ConfigurationService>());

        IUnitOfWork unitOfWork = new MongoDbContext(configurationService, loggerFactory.CreateLogger<MongoDbContext>());

        AdminImageRepository imageRepository = new AdminImageRepository(unitOfWork, loggerFactory.CreateLogger<ImageRepository>());
        AdminPlantRepository plantRepository = new AdminPlantRepository(unitOfWork, loggerFactory.CreateLogger<PlantRepository>());
        AdminPlantVarietyRepository plantVarietyRepository = new AdminPlantVarietyRepository(unitOfWork, loggerFactory.CreateLogger<PlantVarietyRepository>());

        ILogger<AdminImageProcessor> logger = loggerFactory.CreateLogger<AdminImageProcessor>();

        try
        {
            logger.LogInformation("Going to get all images");
            var images = await imageRepository.GetAllImages();
            logger.LogInformation($"Found {images.Count} imags");

            logger.LogInformation("Going to get all plants");
            var plants = await plantRepository.GetAllPlants();
            logger.LogInformation($"Found {plants.Count} plants");

            logger.LogInformation("Going to get all varieties");
            var varieties = await plantVarietyRepository.GetAllPlantVarieties();
            logger.LogInformation($"Found {varieties.Count} varieties");

            foreach (var image in images)
            {
                logger.LogInformation($"Image: {image.FileName} is for {image.RelatedEntityType}");
                if (image.RelatedEntities==null || !image.RelatedEntities.Any())
                {
                   List<RelatedEntity> relatedEntities = new List<RelatedEntity>();

                    logger.LogInformation($"Image: {image.FileName} does not have related entities");
                    switch (image.RelatedEntityType)
                    {
                        case RelatedEntityTypEnum.Plant:
                            var plant = plants.FirstOrDefault(p => p.Id == image.RelatedEntityId);
                            if (plant != null)
                            {
                                relatedEntities.Add(new RelatedEntity(RelatedEntityTypEnum.Plant, image.RelatedEntityId, plant.Name));
                                logger.LogInformation($"Image: {image.FileName} added {relatedEntities[0]}");
                            }
                            else
                            {
                                logger.LogInformation($"Image: {image.FileName} is missing valid plant {image.RelatedEntityId}");
                                continue;
                            }
                            image.SetRelatedEntities(relatedEntities);
                            imageRepository.Update(image);
                            break;
                        case RelatedEntityTypEnum.PlantVariety:
                            var variety = varieties.FirstOrDefault(p => p.Id == image.RelatedEntityId);
                            if (variety != null)
                            {
                                relatedEntities.Add(new RelatedEntity(RelatedEntityTypEnum.Plant, variety.PlantId, variety.PlantName));
                                logger.LogInformation($"Image: {image.FileName} added {relatedEntities[0]}");
                                relatedEntities.Add(new RelatedEntity(RelatedEntityTypEnum.PlantVariety, image.RelatedEntityId, variety.Name));
                                logger.LogInformation($"Image: {image.FileName} added {relatedEntities[1]}");
                            }
                            else
                            {
                                logger.LogInformation($"Image: {image.FileName} is missing valid variety {image.RelatedEntityId}");
                                continue;
                            }
                            image.SetRelatedEntities(relatedEntities);
                            imageRepository.Update(image);
                            break;
                    }
                }
            }

            logger.LogWarning("About to commit all this work to MongoDB. Are you sure you want to continue?");
            Console.ReadLine();
            return await unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogCritical($"Exception processing - {ex.Message}", ex);
            throw;
        
        }
    }
}
