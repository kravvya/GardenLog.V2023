using GardenLog.SharedKernel.Interfaces;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using PlantHarvest.Domain.HarvestAggregate;
using PlantHarvest.Infrastructure.Data.Repositories;

namespace GardenLogAdminConsole.Harvest;

internal class AdminHarvestRepository: HarvestCycleRepository
{
    public AdminHarvestRepository(IUnitOfWork unitOfWork, ILogger<HarvestCycleRepository> logger)
        : base(unitOfWork, logger)
    {

    }

    internal async Task<List<HarvestCycle>> GetAllHarvests()
    {
        return await Collection.Find<HarvestCycle>(new BsonDocument()).ToListAsync();
    }
}
