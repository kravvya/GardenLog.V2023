using GardenLog.SharedKernel.Interfaces;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System.Windows.Input;

namespace GardenLog.SharedInfrastructure.MongoDB
{
    public abstract class BaseRepository<T> : IRepository<T> where T : class, IAggregateRoot
    {
        private readonly IMongoCollectionContext<T> _context;
        private readonly ILogger<BaseRepository<T>> _logger;
       

        public BaseRepository(IMongoCollectionContext<T> context, ILogger<BaseRepository<T>> logger)
        {
            _context = context;
            _logger = logger;           
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

       public void Update(T entity)
        {
            AddCommand(() => _context.Collection.ReplaceOneAsync(Builders<T>.Filter.Eq("_id", entity.Id), entity));
        }

        protected void AddCommand(Func<Task> func)
        {
            _context.UnitOfWork.AddCommand(func);
        }
    }
}
