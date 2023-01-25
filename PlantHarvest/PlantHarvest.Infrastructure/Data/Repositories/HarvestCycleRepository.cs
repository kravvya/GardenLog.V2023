using GardenLog.SharedInfrastructure.MongoDB;
using GardenLog.SharedKernel;
using GardenLog.SharedKernel.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Reflection;

namespace PlantHarvest.Infrastructure.Data.Repositories;

public class HarvestCycleRepository : BaseRepository<HarvestCycle>, IHarvestCycleRepository
{
    private const string HARVEST_COLLECTION_NAME = "PlantHarvest-Collection";
    private readonly ILogger<HarvestCycleRepository> _logger;

    public HarvestCycleRepository(IUnitOfWork unitOfWork, ILogger<HarvestCycleRepository> logger)
        : base(unitOfWork, logger)
    {
        _logger = logger;
    }

    public async Task<string> GetIdByNameAsync(string harvestCycleName, string userProfileId)
    {
        var idOnlyProjection = Builders<HarvestCycle>.Projection.Include(p => p.Id);

        List<FilterDefinition<HarvestCycle>> filters = new();
        filters.Add(Builders<HarvestCycle>.Filter.Eq("HarvestCycleName", harvestCycleName));
        filters.Add(Builders<HarvestCycle>.Filter.Eq("UserProfileId", userProfileId));

        var data = await Collection
            .Find<HarvestCycle>(Builders<HarvestCycle>.Filter.And(filters))
            .Project(idOnlyProjection)
            .FirstOrDefaultAsync();

        if (data != null)
        {
            if (data.TryGetValue("_id", out var id))
                return id.ToString();
        }
        return string.Empty;
    }

    public async Task<IReadOnlyCollection<HarvestCycleViewModel>> GetAllHarvestCycles(string userProfileId)
    {
        var data = await Collection
          .Find<HarvestCycle>(Builders<HarvestCycle>.Filter.Eq("UserProfileId", userProfileId))
          .As<HarvestCycleViewModel>()
          .ToListAsync();

        return data;
    }

    #region Plan Harvest Cycle
    public void AddPlanHarvestCycle(string planHarvestCyclceId, HarvestCycle harvestCyclce)
    {
        var harvestFilter = Builders<HarvestCycle>.Filter.Eq("_id", harvestCyclce.Id);
        var update = Builders<HarvestCycle>.Update.Push<PlanHarvestCycle>("Plans", harvestCyclce.Plans.First(g => g.Id == planHarvestCyclceId));

        AddCommand(() => Collection.UpdateOneAsync(harvestFilter, update));
    }

    public void DeletePlanHarvestCycle(string planHarvestCyclceId, HarvestCycle harvestCyclce)
    {
        var harvestFilter = Builders<HarvestCycle>.Filter.Eq("_id", harvestCyclce.Id);
        var update = Builders<HarvestCycle>.Update.PullFilter(p => p.Plans, Builders<PlanHarvestCycle>.Filter.Eq(p => p.Id, planHarvestCyclceId));

        AddCommand(() => Collection.UpdateOneAsync(harvestFilter, update));
    }

    public void UpdatePlanHarvestCycle(string planHarvestCyclceId, HarvestCycle harvestCyclce)
    {
        var harvestFilter = Builders<HarvestCycle>.Filter.Eq("_id", harvestCyclce.Id);
        var update = Builders<HarvestCycle>.Update.Set("Plans.$[f]", harvestCyclce.Plans.First(g => g.Id == planHarvestCyclceId));
        var options = new UpdateOptions()
        {
            ArrayFilters = new List<ArrayFilterDefinition<BsonValue>>()
            {
                new BsonDocument("f._id",
                new BsonDocument("$eq", planHarvestCyclceId))
            }
        };

        AddCommand(() => Collection.UpdateOneAsync(harvestFilter, update, options));
    }

    public async Task<PlanHarvestCycleViewModel> GetPlanHarvestCycle(string harvestCycleId, string id)
    {
        var data = await Collection
         .Find<HarvestCycle>(Builders<HarvestCycle>.Filter.Eq("_id", harvestCycleId))
         .Project(Builders<HarvestCycle>.Projection.Include(p => p.Plans))
         .As<PlanHarvestCycleViewModelProjection>()
         .FirstAsync();

        data.Plans.ForEach(g => g.HarvestCycleId = data._id);

        return data.Plans.First(g => g.PlanHarvestCycleId == id);
    }
    
    public async Task<IReadOnlyCollection<PlanHarvestCycleViewModel>> GetPlanHarvestCycles(string harvestCycleId)
    {
        var data = await Collection
         .Find<HarvestCycle>(Builders<HarvestCycle>.Filter.Eq("_id", harvestCycleId))
         .Project(Builders<HarvestCycle>.Projection.Include(p => p.Plans))
         .As<PlanHarvestCycleViewModelProjection>()
         .FirstAsync();

        data.Plans.ForEach(g => g.HarvestCycleId = data._id);

        return data.Plans;
    }
    #endregion 

    #region Plant Harvest Cycle
    public void AddPlantHarvestCycle(string plantHarvestCyclceId, HarvestCycle harvestCyclce)
    {
        var harvestFilter = Builders<HarvestCycle>.Filter.Eq("_id", harvestCyclce.Id);
        var update = Builders<HarvestCycle>.Update.Push<PlantHarvestCycle>("Plants", harvestCyclce.Plants.First(g => g.Id == plantHarvestCyclceId));

        AddCommand(() => Collection.UpdateOneAsync(harvestFilter, update));
    }

    public void DeletePlantHarvestCycle(string plantHarvestCyclceId, HarvestCycle harvestCyclce)
    {
        var harvestFilter = Builders<HarvestCycle>.Filter.Eq("_id", harvestCyclce.Id);
        var update = Builders<HarvestCycle>.Update.PullFilter(p => p.Plants, Builders<PlantHarvestCycle>.Filter.Eq(p => p.Id, plantHarvestCyclceId));

        AddCommand(() => Collection.UpdateOneAsync(harvestFilter, update));
    }

    public void UpdatePlantHarvestCycle(string plantHarvestCyclceId, HarvestCycle harvestCyclce)
    {
        var harvestFilter = Builders<HarvestCycle>.Filter.Eq("_id", harvestCyclce.Id);
        var update = Builders<HarvestCycle>.Update.Set("Plants.$[f]", harvestCyclce.Plants.First(g => g.Id == plantHarvestCyclceId));
        var options = new UpdateOptions()
        {
            ArrayFilters = new List<ArrayFilterDefinition<BsonValue>>()
            {
                new BsonDocument("f._id",
                new BsonDocument("$eq", plantHarvestCyclceId))
            }
        };

        AddCommand(() => Collection.UpdateOneAsync(harvestFilter, update, options));
    }

    public async Task<PlantHarvestCycleViewModel> GetPlantHarvestCycle(string harvestCycleId, string id)
    {
        var data = await Collection
          .Find<HarvestCycle>(Builders<HarvestCycle>.Filter.Eq("_id", harvestCycleId))
         .Project(Builders<HarvestCycle>.Projection.Include(p => p.Plants))
         .As<PlantHarvestCycleViewModelProjection>()
         .FirstAsync();

        data.Plants.ForEach(g => g.HarvestCycleId = data._id);

        return data.Plants.First(g => g.PlantHarvestCycleId == id);
    }

    public async Task<IReadOnlyCollection<PlantHarvestCycleIdentityOnlyViewModel>> GetPlantHarvestCyclesByPlantId(string plantId)
    {
        List<PlantHarvestCycleIdentityOnlyViewModel> response = new();

        var filter = Builders<HarvestCycle>.Filter.Eq("Plants.PlantId", plantId);
        var projection = Builders<HarvestCycle>.Projection.Include("Plants._id");

        var data = await Collection
        .Find<HarvestCycle>(filter)
         .Project<HarvestCycle>(projection)
         .As<PlantHarvestCycleViewModelProjection>()
         .ToListAsync();

        foreach(var item in data)
        {
            foreach (var p in item.Plants.Where(p => p.PlantId == plantId))
            {
                response.Add(new PlantHarvestCycleIdentityOnlyViewModel()
                {
                    HarvestCycleId = item._id,
                    PlantHarvestCycleId = p.PlantHarvestCycleId,
                });
            }
        }

        return response;
    }

    public async Task<IReadOnlyCollection<PlantHarvestCycleViewModel>> GetPlantHarvestCycles(string harvestCycleId)
    {
        var data = await Collection
       .Find<HarvestCycle>(Builders<HarvestCycle>.Filter.Eq("_id", harvestCycleId))
       .Project(Builders<HarvestCycle>.Projection.Include(p => p.Plants))
       .As<PlantHarvestCycleViewModelProjection>()
       .FirstAsync();

        data.Plants.ForEach(g => g.HarvestCycleId = data._id);

        return data.Plants;
    }
    #endregion

    protected override IMongoCollection<HarvestCycle> GetCollection()
    {
        return _unitOfWork.GetCollection<IMongoCollection<HarvestCycle>, HarvestCycle>(HARVEST_COLLECTION_NAME);
    }

    protected override void OnModelCreating()
    {
        #region Harvest Cycle
        BsonClassMap.RegisterClassMap<HarvestCycle>(p =>
        {
            p.AutoMap();
            //ignore elements not int he document 
            p.SetIgnoreExtraElements(true);
            p.SetDiscriminator("harvest-cycle");


            p.MapProperty(m => m.Plans).SetDefaultValue(new List<PlanHarvestCycle>());
            p.MapProperty(m => m.Plants).SetDefaultValue(new List<PlantHarvestCycle>());

            var nonPublicCtors = p.ClassType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
            var longestCtor = nonPublicCtors.OrderByDescending(ctor => ctor.GetParameters().Length).FirstOrDefault();
            p.MapConstructor(longestCtor, p.ClassType.GetProperties().Where(c => c.Name != "Id").Select(c => c.Name).ToArray());

        });

        BsonClassMap.RegisterClassMap<BaseEntity>(p =>
        {
            p.AutoMap();
            //p.MapIdMember(c => c.Id).SetIdGenerator(MongoDB.Bson.Serialization.IdGenerators.StringObjectIdGenerator.Instance);
            //p.IdMemberMap.SetSerializer(new StringSerializer(BsonType.ObjectId));
            p.UnmapMember(m => m.DomainEvents);
        });

        BsonClassMap.RegisterClassMap<HarvestCycleBase>(p =>
        {
            p.AutoMap();
            //ignore elements not in the document 
            p.SetIgnoreExtraElements(true);

        });

        BsonClassMap.RegisterClassMap<HarvestCycleViewModel>(p =>
        {
            p.AutoMap();
            //ignore elements not in the document 
            p.SetIgnoreExtraElements(true);
            p.MapMember(m => m.HarvestCycleId).SetElementName("_id");
        });

        #endregion

        #region Plant Harvest Cycle
        BsonClassMap.RegisterClassMap<PlantHarvestCycle>(g =>
        {
            g.AutoMap();
            g.SetIgnoreExtraElements(true);

        });


        BsonClassMap.RegisterClassMap<PlantHarvestCycleViewModel>(p =>
        {
            p.AutoMap();
            //ignore elements not in the document 
            p.SetIgnoreExtraElements(true);
            p.MapMember(m => m.PlantHarvestCycleId).SetElementName("_id");

        });

        BsonClassMap.RegisterClassMap<PlantHarvestCycleBase>(p =>
        {
            p.AutoMap();
            //ignore elements not in the document 
            p.SetIgnoreExtraElements(true);

        });

        BsonClassMap.RegisterClassMap<PlantHarvestCycleViewModelProjection>(p =>
        {
            p.AutoMap();
            //ignore elements not in the document 
            p.SetIgnoreExtraElements(true);

        });
        #endregion

        #region Plan Harvest Cycle
        BsonClassMap.RegisterClassMap<PlanHarvestCycle>(g =>
        {
            g.AutoMap();
            g.SetIgnoreExtraElements(true);

        });


        BsonClassMap.RegisterClassMap<PlanHarvestCycleViewModel>(p =>
        {
            p.AutoMap();
            //ignore elements not in the document 
            p.SetIgnoreExtraElements(true);
            p.MapMember(m => m.PlanHarvestCycleId).SetElementName("_id");

        });

        BsonClassMap.RegisterClassMap<PlanHarvestCycleBase>(p =>
        {
            p.AutoMap();
            //ignore elements not in the document 
            p.SetIgnoreExtraElements(true);

        });

        BsonClassMap.RegisterClassMap<PlanHarvestCycleViewModelProjection>(p =>
        {
            p.AutoMap();
            //ignore elements not in the document 
            p.SetIgnoreExtraElements(true);

        });
        #endregion 
    }


}

public record PlantHarvestCycleViewModelProjection(string _id, List<PlantHarvestCycleViewModel> Plants);
public record PlanHarvestCycleViewModelProjection(string _id, List<PlanHarvestCycleViewModel> Plans);