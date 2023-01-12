
using GardenLog.SharedKernel.Interfaces;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver.Linq;

namespace GardenLog.SharedInfrastructure.MongoDB;

public class MongoDbContext : IUnitOfWork
{
    private readonly ILogger<MongoDbContext> _logger;

    protected MongoClient _mongoClient { get; set; }
    protected IMongoDatabase _database { get; set; }
    protected MongoSettings _settings { get; set; }
    protected IClientSessionHandle Session { get; set; }

    private readonly List<Func<Task>> _commands;

    public MongoDbContext(IConfigurationService configurationService, ILogger<MongoDbContext> logger)
    {
        _logger = logger;
        _settings = configurationService.GetPlantCatalogMongoSettings();

        // Every command will be stored and it'll be processed at SaveChanges
        _commands = new List<Func<Task>>();

        OnConfiguring();
    }

    public void OnConfiguring()
    {
        _logger.LogInformation("Got connection string. Start with {server}", _settings.Server);

        MongoUrlBuilder bldr = new MongoUrlBuilder();
        bldr.Scheme = ConnectionStringScheme.MongoDBPlusSrv;
        bldr.UseTls = true;
        bldr.Server = new MongoServerAddress(_settings.Server);
        bldr.Username = _settings.UserName;
        bldr.Password = _settings.Password;

        var settings = MongoClientSettings.FromUrl(bldr.ToMongoUrl());
        settings.RetryWrites = true;
        settings.WriteConcern = WriteConcern.WMajority;
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);
        settings.LinqProvider = LinqProvider.V3;

        _mongoClient = new MongoClient(settings);
        _logger.LogInformation("Mongo Client is set up");

        _database = _mongoClient.GetDatabase(_settings.DatabaseName);
        _logger.LogInformation("Mongo database is set up {db}", _settings.DatabaseName);

        BsonDefaults.GuidRepresentationMode = GuidRepresentationMode.V3;
    }

    public T GetCollection<T, Y>(string collectionName)
    {
        return (T)_database.GetCollection<Y>(collectionName);
    }

    public async Task<int> SaveChangesAsync()
    {
        int numberOfChanges = await ApplyChangesAsync(_commands);
        _commands.Clear();
        return numberOfChanges;
    }

    public void AddCommand(Func<Task> func)
    {
        _commands.Add(func);
    }

    private async Task<int> ApplyChangesAsync(List<Func<Task>> commands)
    {

        using (Session = await _mongoClient.StartSessionAsync())
        {
            Session.StartTransaction();

            var commandTasks = commands.Select(c => c());

            await Task.WhenAll(commandTasks);

            await Session.CommitTransactionAsync();
        }

        return commands.Count; ;
    }
}