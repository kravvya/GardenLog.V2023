namespace PlantCatalog.IntegrationTest.Fixture;

public class PlantCatalogServiceFixture : PlantCatalogApplicationFactory<Program>, IDisposable
{
    private bool _disposedValue;
    private readonly PlantCatalogApplicationFactory<Program> _factory;
    

    public PlantCatalogServiceFixture()
    {
        _factory= new PlantCatalogApplicationFactory<Program>();
        _factory.ConfigureAwait(true);

        FixtureId = Guid.NewGuid().ToString();
        
        var client = _factory.CreateClient();

        PlantCatalogClient = new PlantCatalogClient(client.BaseAddress, client);
    }

    public PlantCatalogClient PlantCatalogClient { get; init; }
    public string FixtureId { get; init; }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _factory.Dispose();
            }
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
