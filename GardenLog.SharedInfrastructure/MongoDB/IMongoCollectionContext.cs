using GardenLog.SharedKernel.Interfaces;
using MongoDB.Driver;

namespace GardenLog.SharedInfrastructure.MongoDB;

public interface IMongoCollectionContext<T> where T : class, IEntity
{
    public IMongoCollection<T> Collection { get; }

    public IUnitOfWork UnitOfWork { get; }

}
