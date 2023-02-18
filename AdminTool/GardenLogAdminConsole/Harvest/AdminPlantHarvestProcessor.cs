using DnsClient.Internal;
using GardenLog.SharedInfrastructure;
using GardenLog.SharedInfrastructure.MongoDB;
using GardenLog.SharedKernel.Interfaces;
using GardenLogAdminConsole.Plants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PlantCatalog.Infrustructure.Data.Repositories;
using PlantHarvest.Domain.HarvestAggregate;
using PlantHarvest.Infrastructure.Data.Repositories;
using System.Reflection;

namespace GardenLogAdminConsole.Harvest;

internal class AdminPlantHarvestProcessor
{
    internal int UpdateAllPlantHarvestWithSpacingInInches(IConfiguration config, Microsoft.Extensions.Logging.ILoggerFactory loggerFactory)
    {
        IConfigurationService configurationService = new ConfigurationService(config, loggerFactory.CreateLogger<ConfigurationService>());

        IUnitOfWork unitOfWork = new MongoDbContext(configurationService, loggerFactory.CreateLogger<MongoDbContext>());

        AdminPlantRepository plantRepository = new AdminPlantRepository(unitOfWork, loggerFactory.CreateLogger<PlantRepository>());
        AdminHarvestRepository harvestRepository = new AdminHarvestRepository(unitOfWork, loggerFactory.CreateLogger<HarvestCycleRepository>());

        ILogger<AdminPlantHarvestProcessor> logger = loggerFactory.CreateLogger<AdminPlantHarvestProcessor>();

		try
		{
            logger.LogInformation("Going to get all plants");
            var plants =  plantRepository.GetAllPlants().GetAwaiter().GetResult();
            logger.LogInformation($"Found {plants.Count} plants");

            logger.LogInformation("Going to get all harvests");
            var harvests = harvestRepository.GetAllHarvests().GetAwaiter().GetResult();
            logger.LogInformation($"Found {harvests.Count} harvests");


            PropertyInfo propertyInfo = typeof(PlantHarvestCycle).GetProperty("SpacingInInches");
         

            foreach (var harvest in harvests)
            {
                foreach(var plant in harvest.Plants)
                {
                    var grow = plants.FirstOrDefault(p => p.Id== plant.PlantId)?.GrowInstructions?.FirstOrDefault(g => g.Id == plant.PlantGrowthInstructionId);
                    if (grow != null)
                    {
                        propertyInfo.SetValue(plant, grow.SpacingInInches, null); 
                        logger.LogInformation($"Set value of {plant.PlantName} to {plant.SpacingInInches}");
                    }
                }
                harvestRepository.Update(harvest);
            }

            var response =  unitOfWork.SaveChangesAsync().GetAwaiter().GetResult();

            return response;
        }
		catch (Exception ex)
		{
            logger.LogCritical($"Exception processing - {ex.Message}", ex);
            throw;
        }
    }
}
