using GardenLog.SharedInfrastructure.MongoDB;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Driver;
using PlantCatalog.Contract.ViewModels;
using PlantCatalog.Domain.PlantAggregate;
using System.Collections.Generic;
using System.Net.Http.Headers;
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

        public async Task<IReadOnlyCollection<PlantGrowInstructionViewModel>> GetPlantGrowInstractions(string plantId)
        {
            var data = await _context.Collection
               .Find<Plant>(Builders<Plant>.Filter.Eq("_id", plantId))
               .Project(Builders<Plant>.Projection.Include(p => p.GrowInstructions))
               .As<PlantGrowInstructionViewModelProjection>()
               .FirstAsync();

            data.GrowInstructions.ForEach(g => g.PlantId = data._id);
           
            return data.GrowInstructions;
        }

        public async Task<PlantGrowInstructionViewModel> GetPlantGrowInstraction(string plantId, string id)
        {
            //var filter = Builders<Plant>.Filter.Eq(x => x.Id, plantId)  
            //    & Builders<Plant>.Filter.ElemMatch(x => x.GrowInstructions, Builders<PlantGrowInstruction>.Filter.Eq(g => g.Id, id));


            var data = await _context.Collection
               .Find<Plant>(Builders<Plant>.Filter.Eq("_id", plantId))
              .Project(Builders<Plant>.Projection.Include(p => p.GrowInstructions))
              .As<PlantGrowInstructionViewModelProjection>()
              .FirstAsync();

            data.GrowInstructions.ForEach(g => g.PlantId = data._id);

            return data.GrowInstructions.First(g => g.PlantGrowInstructionId==id);
        }

        public void AddPlantGrowInstruction(string plantId, PlantGrowInstruction growInstruction, int growInstructionsCount)
        {
            var plantFilter = Builders<Plant>.Filter.Eq("_id", plantId);
            var update = Builders<Plant>.Update.Push<PlantGrowInstruction>("GrowInstructions", growInstruction)
                .Set(p => p.GrowInstructionsCount, growInstructionsCount);

            AddCommand(() => _context.Collection.UpdateOneAsync(plantFilter, update));
        }

        public void UpdatePlantGrowInstruction(string plantId, PlantGrowInstruction growInstruction)
        {
            var filter = Builders<Plant>.Filter.Eq(p => p.Id, plantId);
            var update = Builders<Plant>.Update.Set("GrowInstructions.$[f]", growInstruction);
            var options = new UpdateOptions()
            {
                ArrayFilters = new List<ArrayFilterDefinition<BsonValue>>()
                {
                    new BsonDocument("f._id",
                    new BsonDocument("$eq", growInstruction.Id))
                }
            };

            AddCommand(() => _context.Collection.UpdateOneAsync(filter, update, options));
        }

        public void DeletePlantGrowInstruction(string plantId, string plantGrowInstructionid, int growInstructionsCount)
        {
            var filter = Builders<Plant>.Filter.Eq(p => p.Id, plantId);
            var update = Builders<Plant>.Update.Set(p => p.GrowInstructionsCount, growInstructionsCount)
                .PullFilter(p => p.GrowInstructions, Builders<PlantGrowInstruction>.Filter.Eq(p => p.Id, plantGrowInstructionid));

            AddCommand(() => _context.Collection.UpdateOneAsync(filter, update));
        }

    }

}
