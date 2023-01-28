using GardenLog.SharedInfrastructure.MongoDB;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System.Reflection;

namespace PlantHarvest.Infrastructure.Data.Repositories;

public class GardenRepository : BaseRepository<Garden>, IGardenRepository
{
    private const string GARDEN_COLLECTION_NAME = "Garden-Collection";
    private readonly ILogger<GardenRepository> _logger;

    public GardenRepository(IUnitOfWork unitOfWork, ILogger<GardenRepository> logger)
        : base(unitOfWork, logger)
    {
        _logger = logger;
    }

    public async Task<GardenViewModel> GetGarden(string gardenId)
    {
        var data = await Collection
            .Find<Garden>(Builders<Garden>.Filter.Eq("_id", gardenId))
            .As<GardenViewModel>()
            .FirstOrDefaultAsync();

        return data;

    }

    public async Task<IReadOnlyCollection<GardenViewModel>> GetGardens(string userProfileId)
    {
        var data = await Collection
           .Find<Garden>(Builders<Garden>.Filter.Eq("UserProfileId", userProfileId))
           .As<GardenViewModel>()
           .ToListAsync();

        return data;
    }

    #region Garden Bed

    public void AddGardenBed(string gardenBedId, Garden harvestCyclce)
    {
        var gardenFilter = Builders<Garden>.Filter.Eq("_id", harvestCyclce.Id);
        var update = Builders<Garden>.Update.Push<GardenBed>("GardenBeds", harvestCyclce.GardenBeds.First(g => g.Id == gardenBedId));

        AddCommand(() => Collection.UpdateOneAsync(gardenFilter, update));
    }

    public void DeleteGardenBed(string gardenBedId, Garden garden)
    {
        var gardenFilter = Builders<Garden>.Filter.Eq("_id", garden.Id);
        var update = Builders<Garden>.Update.PullFilter(p => p.GardenBeds, Builders<GardenBed>.Filter.Eq(p => p.Id, gardenBedId));

        AddCommand(() => Collection.UpdateOneAsync(gardenFilter, update));
    }

    public async Task<GardenBedViewModel> GetGardenBed(string gardenId, string id)
    {
        var data = await Collection
           .Find<Garden>(Builders<Garden>.Filter.Eq("_id", gardenId))
           .Project(Builders<Garden>.Projection.Include(g => g.GardenBeds))
           .As<GardenBedViewModelProjection>()
           .FirstOrDefaultAsync();

        return data.GardenBeds.FirstOrDefault(b => b.GardenBedId == id);
    }

    public async Task<IReadOnlyCollection<GardenBedViewModel>> GetGardenBeds(string gardenId)
    {
        var data = await Collection
           .Find<Garden>(Builders<Garden>.Filter.Eq("_id", gardenId))
           .Project(Builders<Garden>.Projection.Include(g => g.GardenBeds))
           .As<GardenBedViewModelProjection>()
           .FirstOrDefaultAsync();

        return data.GardenBeds;
    }

    public void UpdateGardenBed(string gardenBedId, Garden garden)
    {
        var gardenFilter = Builders<Garden>.Filter.Eq("_id", garden.Id);
        var update = Builders<Garden>.Update.Set("GardenBeds.$[f]", garden.GardenBeds.First(g => g.Id == gardenBedId));
        var options = new UpdateOptions()
        {
            ArrayFilters = new List<ArrayFilterDefinition<BsonValue>>()
            {
                new BsonDocument("f._id",
                new BsonDocument("$eq", gardenBedId))
            }
        };

        AddCommand(() => Collection.UpdateOneAsync(gardenFilter, update, options));
    }

    #endregion
  
    protected override IMongoCollection<Garden> GetCollection()
    {
        return _unitOfWork.GetCollection<IMongoCollection<Garden>, Garden>(GARDEN_COLLECTION_NAME);
    }

    protected override void OnModelCreating()
    {

        BsonClassMap.RegisterClassMap<Garden>(p =>
        {
            p.AutoMap();
            //ignore elements not int he document 
            p.SetIgnoreExtraElements(true);
            p.SetDiscriminator("garden");

            var nonPublicCtors = p.ClassType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
            var longestCtor = nonPublicCtors.OrderByDescending(ctor => ctor.GetParameters().Length).FirstOrDefault();
            p.MapConstructor(longestCtor, p.ClassType.GetProperties().Where(c => c.Name != "Id").Select(c => c.Name).ToArray());
        });
             

        BsonClassMap.RegisterClassMap<GardenBase>(p =>
        {
            p.AutoMap();
            //ignore elements not in the document 
            p.SetIgnoreExtraElements(true);

        });

        BsonClassMap.RegisterClassMap<GardenViewModel>(p =>
        {
            p.AutoMap();
            //ignore elements not in the document 
            p.SetIgnoreExtraElements(true);
            p.MapMember(m => m.GardenId).SetElementName("_id");
        });

        BsonClassMap.RegisterClassMap<GardenBed>(p =>
        {
            p.AutoMap();
            //ignore elements not int he document 
            p.SetIgnoreExtraElements(true);
            p.MapMember(m => m.Type).SetSerializer(new EnumSerializer<GardenBedTypeEnum>(BsonType.String));
        });


        BsonClassMap.RegisterClassMap<GardenBedBase>(p =>
        {
            p.AutoMap();
            //ignore elements not in the document 
            p.SetIgnoreExtraElements(true);
            p.MapMember(m => m.Type).SetSerializer(new EnumSerializer<GardenBedTypeEnum>(BsonType.String));
        });

        BsonClassMap.RegisterClassMap<GardenBedViewModel>(p =>
        {
            p.AutoMap();
            //ignore elements not in the document 
            p.SetIgnoreExtraElements(true);
            p.MapMember(m => m.GardenBedId).SetElementName("_id");
        });

    }

}

public record GardenBedViewModelProjection(string _id, List<GardenBedViewModel> GardenBeds);