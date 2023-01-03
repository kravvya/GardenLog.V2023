using GardenLog.SharedInfrastructure.MongoDB;
using Microsoft.Extensions.Configuration;

namespace GardenLog.SharedInfrastructure
{
    public interface IConfigurationService
    {
        MongoSettings GetPlantCatalogMongoSettings();
    }

    public class ConfigurationService : IConfigurationService
    {
        private readonly IConfiguration _configuration;

        public ConfigurationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public MongoSettings GetPlantCatalogMongoSettings()
        {
           // var settings = new MongoSettings();
            // _configuration.GetSection(MongoSettings.SECTION).Bind(settings);
            return _configuration.GetSection(MongoSettings.SECTION).Get<MongoSettings>();          
          //  return settings;
        }


    }
}