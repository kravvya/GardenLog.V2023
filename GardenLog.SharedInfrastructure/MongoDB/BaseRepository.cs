using GardenLog.SharedKernel.Interfaces;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System.Windows.Input;

namespace GardenLog.SharedInfrastructure.MongoDB
{
    public abstract class BaseRepository<T> : IRepository<T> where T : class, IAggregateRoot
    {
        private readonly IMongoDBContext<T> _context;
        private readonly ILogger<BaseRepository<T>> _logger;
        private readonly List<Func<Task>> _commands;

        public BaseRepository(IMongoDBContext<T> context, ILogger<BaseRepository<T>> logger)
        {
            _context = context;
            _logger = logger;

            // Every command will be stored and it'll be processed at SaveChanges
            _commands = new List<Func<Task>>();
        }

        public void Add(T entity)
        {
            AddCommand(() => _context.Collection.InsertOneAsync(entity));
        }

        public void Delete(string id)
        {
            AddCommand(() => _context.Collection.DeleteOneAsync(Builders<T>.Filter.Eq("_id", id)));
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task<T> GetByIdAsync(string id)
        {
            var data = await _context.Collection.FindAsync(Builders<T>.Filter.Eq("_id", id));
            return data.SingleOrDefault();
        }

        public async Task<int> SaveChangesAsync()
        {
            int numberOfChanges = await _context.ApplyChangesAsync(_commands);
            _commands.Clear();
            return numberOfChanges;
        }

        public void Update(T entity)
        {
            AddCommand(() => _context.Collection.ReplaceOneAsync(Builders<T>.Filter.Eq("_id", entity.Id), entity));
        }

        private void AddCommand(Func<Task> func)
        {
            _commands.Add(func);
        }
    }
}
