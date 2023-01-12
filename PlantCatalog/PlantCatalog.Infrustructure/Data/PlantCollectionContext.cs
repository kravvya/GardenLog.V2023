using GardenLog.SharedInfrastructure;
using GardenLog.SharedInfrastructure.MongoDB;
using GardenLog.SharedKernel;
using GardenLog.SharedKernel.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using PlantCatalog.Contract.Base;
using PlantCatalog.Contract.ViewModels;
using PlantCatalog.Domain.PlantAggregate;
using System.Drawing;
using System.Reflection;
using System.Xml.Linq;

namespace PlantCatalog.Infrustructure.Data;


public class PlantCollectionContext : IMongoCollectionContext<Plant>
{
    private const string PLANT_COLLECTION_NAME = "PlantCatalog-Collection";
    
    private ILogger<PlantCollectionContext> _logger { get; }

    public IMongoCollection<Plant> Collection { get; init; }
    public IUnitOfWork UnitOfWork { get; }

    public PlantCollectionContext(IUnitOfWork unitOfWork, ILogger<PlantCollectionContext> logger)
    {
        _logger = logger;
        UnitOfWork = unitOfWork;

        _logger.LogInformation("Setting up Mongo Context");
        OnModelCreating();

        _logger.LogInformation("Models are setup for Mongo Context");
        Collection = unitOfWork.GetCollection<IMongoCollection<Plant>, Plant>(PLANT_COLLECTION_NAME);

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
            p.MapProperty(m => m.Tags).SetDefaultValue(new List<string>());
            p.MapProperty(m => m.VarietyColors).SetDefaultValue(new List<string>());
            p.MapProperty(m => m.GrowInstructions).SetDefaultValue(new List<PlantGrowInstruction>());
            p.MapProperty(m => m.GrowInstructionsCount);

            var nonPublicCtors = p.ClassType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
            var longestCtor = nonPublicCtors.OrderByDescending(ctor => ctor.GetParameters().Length).FirstOrDefault();
            p.MapConstructor(longestCtor, p.ClassType.GetProperties().Where(c => c.Name != "Id" && c.Name != "GrowInstructionsCount").Select(c => c.Name).ToArray());

        });

        BsonClassMap.RegisterClassMap<BaseEntity>(p =>
        {
            p.AutoMap();
            //p.MapIdMember(c => c.Id).SetIdGenerator(MongoDB.Bson.Serialization.IdGenerators.StringObjectIdGenerator.Instance);
            //p.IdMemberMap.SetSerializer(new StringSerializer(BsonType.ObjectId));
            p.UnmapMember(m => m.DomainEvents);
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

        BsonClassMap.RegisterClassMap<PlantGrowInstruction>(g =>
        {
            g.AutoMap();
            g.SetIgnoreExtraElements(true);
            g.MapMember(m => m.PlantingDepthInInches).SetSerializer(new EnumSerializer<PlantingDepthEnum>(BsonType.String));
            g.MapMember(m => m.PlantingMethod).SetSerializer(new EnumSerializer<PlantingMethodEnum>(BsonType.String));
            g.MapMember(m => m.StartSeedAheadOfWeatherCondition).SetSerializer(new EnumSerializer<WeatherConditionEnum>(BsonType.String));
            g.MapMember(m => m.HarvestSeason).SetSerializer(new EnumSerializer<HarvestSeasonEnum>(BsonType.String));
            g.MapMember(m => m.TransplantAheadOfWeatherCondition).SetSerializer(new EnumSerializer<WeatherConditionEnum>(BsonType.String));
            g.MapMember(m => m.FertilizerAtPlanting).SetSerializer(new EnumSerializer<FertilizerEnum>(BsonType.String));
            g.MapMember(m => m.FertilizerForSeedlings).SetSerializer(new EnumSerializer<FertilizerEnum>(BsonType.String));
            g.MapMember(m => m.Fertilizer).SetSerializer(new EnumSerializer<FertilizerEnum>(BsonType.String));
        });


        BsonClassMap.RegisterClassMap<PlantGrowInstructionViewModel>(p =>
        {
            p.AutoMap();
            //ignore elements not in the document 
            p.SetIgnoreExtraElements(true);
            p.MapMember(m => m.PlantGrowInstructionId).SetElementName("_id");
         
        });

        BsonClassMap.RegisterClassMap<PlantGrowInstructionBase>(p =>
        {
            p.AutoMap();
            //ignore elements not in the document 
            p.SetIgnoreExtraElements(true);
           
            p.MapMember(m => m.PlantingDepthInInches).SetSerializer(new EnumSerializer<PlantingDepthEnum>(BsonType.String));
            p.MapMember(m => m.PlantingMethod).SetSerializer(new EnumSerializer<PlantingMethodEnum>(BsonType.String));
            p.MapMember(m => m.StartSeedAheadOfWeatherCondition).SetSerializer(new EnumSerializer<WeatherConditionEnum>(BsonType.String));
            p.MapMember(m => m.HarvestSeason).SetSerializer(new EnumSerializer<HarvestSeasonEnum>(BsonType.String));
            p.MapMember(m => m.TransplantAheadOfWeatherCondition).SetSerializer(new EnumSerializer<WeatherConditionEnum>(BsonType.String));
            p.MapMember(m => m.FertilizerAtPlanting).SetSerializer(new EnumSerializer<FertilizerEnum>(BsonType.String));
            p.MapMember(m => m.FertilizerForSeedlings).SetSerializer(new EnumSerializer<FertilizerEnum>(BsonType.String));
            p.MapMember(m => m.Fertilizer).SetSerializer(new EnumSerializer<FertilizerEnum>(BsonType.String));
        });

        BsonClassMap.RegisterClassMap<PlantGrowInstructionViewModelProjection>(p =>
        {
            p.AutoMap();
            //ignore elements not in the document 
            p.SetIgnoreExtraElements(true);

        });
    }
}


public record PlantGrowInstructionViewModelProjection(string _id, List<PlantGrowInstructionViewModel> GrowInstructions);