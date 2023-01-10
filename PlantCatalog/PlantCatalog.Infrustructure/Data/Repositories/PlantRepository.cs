using GardenLog.SharedInfrastructure.MongoDB;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Driver;
using PlantCatalog.Contract.ViewModels;
using PlantCatalog.Domain.PlantAggregate;
using static MongoDB.Bson.Serialization.Serializers.SerializerHelper;

namespace PlantCatalog.Infrustructure.Data.Repositories
{
    public class PlantRepository : BaseRepository<Plant>, IPlantRepository
    {
        private readonly IMongoDBContext<Plant> _context;
        private readonly ILogger<PlantRepository> _logger;

        public PlantRepository(IMongoDBContext<Plant> context, ILogger<PlantRepository> logger)
            : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Plant> GetByNameAsync(string plantName)
        {
            var data = await _context.Collection.FindAsync<Plant>(Builders<Plant>.Filter.Eq("Name", plantName));
            return data.FirstOrDefault();
        }

        public async Task<string> GetIdByNameAsync(string plantName)
        {
            var idOnlyProjection = Builders<Plant>.Projection.Include(p => p.Id);

            var data = await _context.Collection
                .Find<Plant>(Builders<Plant>.Filter.Eq("Name", plantName))
                .Project(idOnlyProjection)
                .FirstOrDefaultAsync();

            if (data != null)
            {
                if (data.TryGetValue("_id", out var id))
                    return id.ToString();
            }
            return string.Empty;
        }

        public async Task<IReadOnlyCollection<PlantViewModel>> GetAllPlants()
        {
            var data = await _context.Collection
               .Find<Plant>(Builders<Plant>.Filter.Empty)
               .As<PlantViewModel>()
               .ToListAsync();

            return data;
        }

        public async Task<IReadOnlyCollection<PlantGrowInstruction>> GetPlantGrowInstractions(string plantId)
        {
            //var growInstructionsOnlyProjection = Builders<Plant>.Projection.Include(p => p.GrowInstructions);

            //var data = await _context.Collection
            //   .Find<Plant>(Builders<Plant>.Filter.Eq("_id", plantId))
            //   .Project(growInstructionsOnlyProjection)
            //   .As<PlantGrowInstructionViewModel>()
            //   .ToListAsync();

            var data = _context.Collection.AsQueryable()
                .Where(p => p.Id == plantId)
                .Select(x => x.GrowInstructions)
                .FirstOrDefault();

            return data;
        }

        public async Task<PlantGrowInstruction> GetPlantGrowInstraction(string plantId, string id)
        {
            //var growInstructionsOnlyProjection = Builders<Plant>.Projection.Include(p => p.GrowInstructions);

            //var plantFilter = Builders<Plant>.Filter.Eq("_id", plantId);
            //var growFilter = Builders<Plant>.Filter.Eq("GrowingInstructions.Id", id);
            //var filter = Builders<Plant>.Filter.And(plantFilter, growFilter);

            //var data = await _context.Collection
            //   .Find(filter)
            //   .Project(growInstructionsOnlyProjection)
            //   .As<PlantGrowInstructionViewModel>()
            //   .FirstOrDefaultAsync();

            var data = _context.Collection.AsQueryable()
             .Where(p => p.Id == plantId)
             .Select(x => x.GrowInstructions.Where(g => g.Id == id))
             .FirstOrDefault();

            return data.FirstOrDefault();
        }

        public void AddPlantGrowInstruction(string plantId, PlantGrowInstruction growInstruction)
        {
            var plantFilter = Builders<Plant>.Filter.Eq("_id", plantId);
            var update = Builders<Plant>.Update.Push<PlantGrowInstruction>("GrowInstructions", growInstruction);

            AddCommand(() => _context.Collection.UpdateOneAsync(plantFilter, update));
        }

        public void UpdatePlantGrowInstruction(string plantId, PlantGrowInstruction growInstruction)
        {
            //var growFilter = Builders<Plant>.Filter.Eq(x => x.Id, plantId)
            //     & Builders<Plant>.Filter.ElemMatch(x => x.GrowInstructions, Builders<PlantGrowInstruction>.Filter.Eq(x => x.Id, growInstruction.Id));

            //var update = Builders<Plant>.Update.Set("GrowInstructions", growInstruction);


            //AddCommand(() => _context.Collection.UpdateOneAsync(growFilter, update));

            var filter = Builders<Plant>.Filter.Eq(p => p.Id, plantId);
            var update = Builders<Plant>.Update.Set("GrowInstructions.$[f]", growInstruction);


            AddCommand(() => _context.Collection.UpdateOneAsync(filter, update,
            new UpdateOptions()
            {
                ArrayFilters = new List<ArrayFilterDefinition<BsonValue>>()
                {
                    new BsonDocument("f._id",
                    new BsonDocument("$eq", growInstruction.Id))
                }
            })
            );

        }

        public void DeletePlantGrowInstruction(string plantId, PlantGrowInstruction growInstruction)
        {
            var growFilter = Builders<Plant>.Filter.Eq(x => x.Id, plantId)
                 & Builders<Plant>.Filter.ElemMatch(x => x.GrowInstructions, Builders<PlantGrowInstruction>.Filter.Eq(x => x.Id, growInstruction.Id));

            var update = Builders<Plant>.Update.Pull("GrowInstructions", growInstruction);

            AddCommand(() => _context.Collection.UpdateOneAsync(growFilter, update));
        }

    }
}
