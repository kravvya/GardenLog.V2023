using DnsClient.Internal;
using GardenLog.SharedInfrastructure.MongoDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GardenLog.SharedInfrastructure
{
    public interface IConfigurationService
    {
        MongoSettings GetPlantCatalogMongoSettings();
    }

    public class ConfigurationService : IConfigurationService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ConfigurationService> _logger;

        public ConfigurationService(IConfiguration configuration, ILogger<ConfigurationService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public MongoSettings GetPlantCatalogMongoSettings()
        {
            // var settings = new MongoSettings();
            // _configuration.GetSection(MongoSettings.SECTION).Bind(settings);
            var mongoSettings = _configuration.GetSection(MongoSettings.SECTION).Get<MongoSettings>();
            if (string.IsNullOrWhiteSpace(mongoSettings.Password))
                mongoSettings.Password = _configuration.GetValue<string>(MongoSettings.PASSWORD_SECRET);

            if (string.IsNullOrWhiteSpace(mongoSettings.Password))
                _logger.LogCritical("DB passwqord is not found. Do not expect any good things to happen");

            return _configuration.GetSection(MongoSettings.SECTION).Get<MongoSettings>();          
          //  return settings;
        }


    }
}