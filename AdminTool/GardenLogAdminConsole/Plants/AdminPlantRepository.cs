using GardenLog.SharedKernel.Interfaces;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using PlantCatalog.Domain.PlantAggregate;
using PlantCatalog.Infrustructure.Data.Repositories;

namespace GardenLogAdminConsole.Plants
{
    internal class AdminPlantRepository : PlantRepository
    {
        public AdminPlantRepository(IUnitOfWork unitOfWork, ILogger<PlantRepository> logger) 
            : base(unitOfWork, logger)
        {
        }

        internal new async Task<List<Plant>> GetAllPlants()
        {
            return await Collection.Find(new BsonDocument()).ToListAsync();
        }
    }
}
