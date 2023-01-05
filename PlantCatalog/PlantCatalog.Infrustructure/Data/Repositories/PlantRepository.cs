using GardenLog.SharedInfrastructure.MongoDB;
using MongoDB.Driver;
using PlantCatalog.Contract.ViewModels;
using PlantCatalog.Domain.PlantAggregate;

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
    }
}
