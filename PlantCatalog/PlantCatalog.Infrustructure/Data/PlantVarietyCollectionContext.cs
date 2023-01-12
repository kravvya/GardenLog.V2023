using GardenLog.SharedInfrastructure.MongoDB;
using GardenLog.SharedKernel.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using PlantCatalog.Contract.Base;
using PlantCatalog.Contract.ViewModels;
using PlantCatalog.Domain.PlantAggregate;

namespace PlantCatalog.Infrustructure.Data;


public class PlantVarietyCollectionContext : IMongoCollectionContext<PlantVariety>
{
    private const string PLANT_VARIETY_COLLECTION_NAME = "PlantCatalog-Collection";

    private ILogger<PlantVarietyCollectionContext> _logger { get; }

    public IMongoCollection<PlantVariety> Collection { get; }

    public IUnitOfWork UnitOfWork { get; }

    public PlantVarietyCollectionContext(IUnitOfWork unitOfWork, ILogger<PlantVarietyCollectionContext> logger)
    {
        _logger = logger;
        UnitOfWork = unitOfWork;

        _logger.LogInformation("Setting up Mongo Context");
        OnModelCreating();

        _logger.LogInformation("Models are setup for Mongo Context");
        Collection = unitOfWork.GetCollection<IMongoCollection<PlantVariety>, PlantVariety>(PLANT_VARIETY_COLLECTION_NAME);
    }


    private void OnModelCreating()
    {
        BsonClassMap.RegisterClassMap<PlantVariety>(p =>
        {
            p.AutoMap();
            //ignore elements not int he document 
            p.SetIgnoreExtraElements(true);
            p.SetDiscriminator("plantVariety");

            p.MapMember(m => m.MoistureRequirement).SetSerializer(new EnumSerializer<MoistureRequirementEnum>(BsonType.String));
            p.MapMember(m => m.LightRequirement).SetSerializer(new EnumSerializer<LightRequirementEnum>(BsonType.String));
            p.MapMember(m => m.GrowTolerance).SetSerializer(new EnumToStringArraySerializer<GrowToleranceEnum>());
            p.MapProperty(m => m.Tags).SetDefaultValue(new List<string>());
            p.MapProperty(m => m.Colors).SetDefaultValue(new List<string>());
        });


        BsonClassMap.RegisterClassMap<PlantVarietyViewModel>(p =>
        {
            p.AutoMap();
            //ignore elements not in the document 
            p.SetIgnoreExtraElements(true);
            p.MapMember(m => m.PlantVarietyId).SetElementName("_id");

        });

        BsonClassMap.RegisterClassMap<PlantVarietyBase>(p =>
        {
            p.AutoMap();
            //ignore elements not int he document 
            p.SetIgnoreExtraElements(true);

            p.MapMember(m => m.MoistureRequirement).SetSerializer(new EnumSerializer<MoistureRequirementEnum>(BsonType.String));
            p.MapMember(m => m.LightRequirement).SetSerializer(new EnumSerializer<LightRequirementEnum>(BsonType.String));
            p.MapMember(m => m.GrowTolerance).SetSerializer(new EnumToStringArraySerializer<GrowToleranceEnum>());
            p.MapProperty(m => m.Tags).SetDefaultValue(new List<string>());
            p.MapProperty(m => m.Colors).SetDefaultValue(new List<string>());
        });
    }


}
