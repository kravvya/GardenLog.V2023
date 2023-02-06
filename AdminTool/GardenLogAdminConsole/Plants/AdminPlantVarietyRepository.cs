using GardenLog.SharedKernel.Interfaces;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using PlantCatalog.Domain.PlantAggregate;
using PlantCatalog.Infrustructure.Data.Repositories;

namespace GardenLogAdminConsole.Plants
{
    internal class AdminPlantVarietyRepository : PlantVarietyRepository
    {
        public AdminPlantVarietyRepository(IUnitOfWork unitOfWork, ILogger<PlantVarietyRepository> logger) 
            : base(unitOfWork, logger)
        {
        }

        internal async Task<List<PlantVariety>> GetAllPlantVarieties()
        {
            return await Collection.Find(new BsonDocument()).ToListAsync();
        }
    }
}
