using GardenLog.SharedKernel.Interfaces;

namespace GardenLog.SharedInfrastructure.MongoDB;

public class MongoDBUnitOfWork : IUnitOfWork
{
    private readonly List<Func<Task>> _commands;
    private readonly IMongoDBContext _context;
    private string? _rootHandler = null;

    public MongoDBUnitOfWork(IMongoDBContext context)
    {
        _context = context;

        // Every command will be stored and it'll be processed at SaveChanges
        _commands = new List<Func<Task>>();
    }

    public string Initialize(string handlerName)
    {
        if (string.IsNullOrWhiteSpace(_rootHandler))
        {
            _rootHandler = handlerName;
        }

        return _rootHandler;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await SaveChangesAsync(null);
    }

    public async Task<int> SaveChangesAsync(string? handlerName)
    {
        if (!string.IsNullOrWhiteSpace(_rootHandler))
        {
            if (!string.IsNullOrWhiteSpace(handlerName) && handlerName == _rootHandler)
            {
                int numberOfChanges = await _context.ApplyChangesAsync(_commands);
                _commands.Clear();
                return numberOfChanges;
            }
        }

        return 0;
    }

    public void AddCommand(Func<Task> func)
    {
        _commands.Add(func);
    }

    public T GetCollection<T, Y>(string collectionName)
    {
        return _context.GetCollection<T, Y>(collectionName);
    }

  
}

