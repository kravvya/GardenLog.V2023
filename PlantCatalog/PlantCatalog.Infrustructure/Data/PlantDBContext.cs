using GardenLog.SharedInfrastructure;
using GardenLog.SharedInfrastructure.MongoDB;
using GardenLog.SharedKernel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using PlantCatalog.Contract.Base;
using PlantCatalog.Contract.ViewModels;
using PlantCatalog.Domain.PlantAggregate;

namespace PlantCatalog.Infrustructure.Data;


public class PlantDBContext : IMongoDBContext<Plant>
{
    private const string PLANT_COLLECTION_NAME = "PlantCatalog-Collection";

    private ILogger<PlantDBContext> _logger { get; }

    private MongoSettings _settings { get; set; }

    public IClientSessionHandle Session { get; set; }
    public IMongoCollection<Plant> Collection { get; private set; }
    private IMongoDatabase _database { get; set; }
    private MongoClient _mongoClient { get; set; }

    public PlantDBContext(IConfigurationService configurationService, ILogger<PlantDBContext> logger)
    {
        _settings = configurationService.GetPlantCatalogMongoSettings();
        _logger = logger;

        _logger.LogInformation("Setting up Mongo Context");
        OnModelCreating();

        _logger.LogInformation("Models are setup for Mongo Context");
        OnConfiguring();

    }

    private void OnConfiguring()
    {
        _logger.LogInformation("Got connection string. Start with {server}", _settings.Server);

        MongoUrlBuilder bldr = new MongoUrlBuilder();
        bldr.Scheme = MongoDB.Driver.Core.Configuration.ConnectionStringScheme.MongoDBPlusSrv;
        bldr.UseTls= true;
        bldr.Server = new MongoServerAddress(_settings.Server);
        bldr.Username = _settings.UserName;
        bldr.Password = _settings.Password;

        var settings = MongoClientSettings.FromUrl(bldr.ToMongoUrl());
        settings.RetryWrites = true;
        settings.WriteConcern = WriteConcern.WMajority;
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);
        settings.LinqProvider = LinqProvider.V3;

        _mongoClient = new MongoClient(settings);
        _logger.LogInformation("Mongo Client is set up");

        _database = _mongoClient.GetDatabase(_settings.DatabaseName);
        _logger.LogInformation("Mongo database is set up {db}", _settings.DatabaseName);

        Collection = _database.GetCollection<Plant>(PLANT_COLLECTION_NAME);
        _logger.LogInformation("Mongo collection is set up {collection}", PLANT_COLLECTION_NAME);

        BsonDefaults.GuidRepresentationMode = GuidRepresentationMode.V3;
    }
    
    private void OnModelCreating()
    {
        BsonClassMap.RegisterClassMap<Plant>(p =>
        {
            p.AutoMap();
            //ignore elements not int he document 
            p.SetIgnoreExtraElements(true);
            p.SetDiscriminator("plant");
            //p.MapIdMember(c => c.PlantId).SetIdGenerator(MongoDB.Bson.Serialization.IdGenerators.StringObjectIdGenerator.Instance);
            //p.IdMemberMap.SetSerializer(new StringSerializer(BsonType.ObjectId));

            p.MapMember(m => m.Lifecycle).SetSerializer(new EnumSerializer<PlantLifecycleEnum>(BsonType.String));
            p.MapMember(m => m.Type).SetSerializer(new EnumSerializer<PlantTypeEnum>(BsonType.String));
            p.MapMember(m => m.MoistureRequirement).SetSerializer(new EnumSerializer<MoistureRequirementEnum>(BsonType.String));
            p.MapMember(m => m.LightRequirement).SetSerializer(new EnumSerializer<LightRequirementEnum>(BsonType.String));
           // p.MapMember(m => m.GrowTolerance).SetSerializer(new EnumSerializer<GrowToleranceEnum>(BsonType.String));
            p.MapMember(m => m.GrowTolerance).SetSerializer(new EnumToStringArraySerializer<GrowToleranceEnum>());

        });

        BsonClassMap.RegisterClassMap<BaseEntity>(p =>
        {
            p.AutoMap();
            //p.MapIdMember(c => c.Id).SetIdGenerator(MongoDB.Bson.Serialization.IdGenerators.StringObjectIdGenerator.Instance);
            //p.IdMemberMap.SetSerializer(new StringSerializer(BsonType.ObjectId));
            p.UnmapMember(m => m.DomainEvents);
        });

       BsonClassMap.RegisterClassMap<GrowInstruction>(g =>
        {
            g.AutoMap();
            g.SetIgnoreExtraElements(true);
            g.MapMember(m => m.PlantingDepthInInches).SetSerializer(new EnumSerializer<PlantingDepthEnum>(BsonType.String));
            g.MapMember(m => m.PlantingMethod).SetSerializer(new EnumSerializer<PlantingMethodEnum>(BsonType.String));
            g.MapMember(m => m.StartSeedAheadOfWeatherCondition).SetSerializer(new EnumSerializer<WeatherConditionEnum>(BsonType.String));
            g.MapMember(m => m.HarvestSeason).SetSerializer(new EnumSerializer<HarvestSeasonEnum>(BsonType.String));
            g.MapMember(m => m.TransplantAheadOfWeatherCondition).SetSerializer(new EnumSerializer<WeatherConditionEnum>(BsonType.String));
            g.MapMember(m => m.FertilizerAtPlanting).SetSerializer(new EnumSerializer<FertilizerEnum>(BsonType.String));
            g.MapMember(m => m.Fertilizer).SetSerializer(new EnumSerializer<FertilizerEnum>(BsonType.String));


        });

        BsonClassMap.RegisterClassMap<PlantBase>(p =>
        {
            p.AutoMap();
            //ignore elements not in the document 
            p.SetIgnoreExtraElements(true);            
            p.MapMember(m => m.Lifecycle).SetSerializer(new EnumSerializer<PlantLifecycleEnum>(BsonType.String));
            p.MapMember(m => m.Type).SetSerializer(new EnumSerializer<PlantTypeEnum>(BsonType.String));
            p.MapMember(m => m.MoistureRequirement).SetSerializer(new EnumSerializer<MoistureRequirementEnum>(BsonType.String));
            p.MapMember(m => m.LightRequirement).SetSerializer(new EnumSerializer<LightRequirementEnum>(BsonType.String));
            p.MapMember(m => m.GrowTolerance).SetSerializer(new EnumToStringArraySerializer<GrowToleranceEnum>());

        });

        BsonClassMap.RegisterClassMap<PlantViewModel>(p =>
        {
            p.AutoMap();
            //ignore elements not in the document 
            p.SetIgnoreExtraElements(true);
            p.MapMember(m => m.PlantId).SetElementName("_id");

        });
    }


    public async Task<int> ApplyChangesAsync(List<Func<Task>> commands)
    {

        using (Session = await _mongoClient.StartSessionAsync())
        {
            Session.StartTransaction();

            var commandTasks = commands.Select(c => c());

            await Task.WhenAll(commandTasks);

            await Session.CommitTransactionAsync();
        }

        return commands.Count;;
    }
}