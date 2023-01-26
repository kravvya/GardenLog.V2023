using GardenLog.SharedInfrastructure.MongoDB;
using GardenLog.SharedKernel;
using GardenLog.SharedKernel.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using PlantHarvest.Contract.Enum;
using PlantHarvest.Domain.WorkLogAggregate;

namespace PlantHarvest.Infrastructure.Data.Repositories;

public class WorkLogRepository : BaseRepository<WorkLog>, IWorkLogRepository
{
    private const string WORKLOG_COLLECTION_NAME = "WorkLog-Collection";
    private readonly ILogger<WorkLogRepository> _logger;

    public WorkLogRepository(IUnitOfWork unitOfWork, ILogger<WorkLogRepository> logger)
        : base(unitOfWork, logger)
    {
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<WorkLogViewModel>> GetWorkLogsByEntity(WorkLogEntityEnum entityType, string entityId, string userProfileId)
    {
        List<FilterDefinition<WorkLog>> filters = new();
        filters.Add(Builders<WorkLog>.Filter.Eq("RelatedEntity", entityType));
        filters.Add(Builders<WorkLog>.Filter.Eq("RelatedEntityid", entityId));
        filters.Add(Builders<WorkLog>.Filter.Eq("UserProfileId", userProfileId));

        var data = await Collection
          .Find<WorkLog>(Builders<WorkLog>.Filter.And(filters))
          .As<WorkLogViewModel>()
          .ToListAsync();

        return data;
    }

    protected override IMongoCollection<WorkLog> GetCollection()
    {
        return _unitOfWork.GetCollection<IMongoCollection<WorkLog>, WorkLog>(WORKLOG_COLLECTION_NAME);
    }

    protected override void OnModelCreating()
    {

        BsonClassMap.RegisterClassMap<WorkLog>(p =>
        {
            p.AutoMap();
            //ignore elements not int he document 
            p.SetIgnoreExtraElements(true);
            p.SetDiscriminator("work-log");

            p.MapMember(m => m.RelatedEntity).SetSerializer(new EnumSerializer<WorkLogEntityEnum>(BsonType.String));
            p.MapMember(m => m.Reason).SetSerializer(new EnumSerializer<WorkLogReasonEnum>(BsonType.String));
        });

        //BsonClassMap.RegisterClassMap<BaseEntity>(p =>
        //{
        //    p.AutoMap();
        //    //p.MapIdMember(c => c.Id).SetIdGenerator(MongoDB.Bson.Serialization.IdGenerators.StringObjectIdGenerator.Instance);
        //    //p.IdMemberMap.SetSerializer(new StringSerializer(BsonType.ObjectId));
        //    p.UnmapMember(m => m.DomainEvents);
        //});

        BsonClassMap.RegisterClassMap<WorkLogBase>(p =>
        {
            p.AutoMap();
            //ignore elements not in the document 
            p.SetIgnoreExtraElements(true);
            p.MapMember(m => m.RelatedEntity).SetSerializer(new EnumSerializer<WorkLogEntityEnum>(BsonType.String));
            p.MapMember(m => m.Reason).SetSerializer(new EnumSerializer<WorkLogReasonEnum>(BsonType.String));
        });

        BsonClassMap.RegisterClassMap<WorkLogViewModel>(p =>
        {
            p.AutoMap();
            //ignore elements not in the document 
            p.SetIgnoreExtraElements(true);
            p.MapMember(m => m.WorkLogId).SetElementName("_id");
        });
    }

}