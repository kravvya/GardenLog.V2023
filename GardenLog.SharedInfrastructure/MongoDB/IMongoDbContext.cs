using GardenLog.SharedKernel.Interfaces;
using MongoDB.Driver;

namespace GardenLog.SharedInfrastructure.MongoDB
{
    public interface IMongoDBContext<T> where T : class, IAggregateRoot
    {
        public IMongoCollection<T> Collection { get; }

        Task<int> ApplyChangesAsync(List<Func<Task>> commands);
    }
}
