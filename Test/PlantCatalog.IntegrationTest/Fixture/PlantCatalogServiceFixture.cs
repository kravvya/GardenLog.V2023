using PlantCatalog.IntegrationTest.Clients;

namespace PlantCatalog.IntegrationTest.Fixture;

public class PlantCatalogServiceFixture : PlantCatalogApplicationFactory<Program>, IDisposable
{
    private bool _disposedValue;
    private readonly WebApplicationFactory<Program> _factory;
    

    public PlantCatalogServiceFixture()
    {
        _factory= new WebApplicationFactory<Program>();
        _factory.ConfigureAwait(false);

        
        var client = _factory.CreateClient();

        PlantCatalogClient = new PlantCatalogClient(client.BaseAddress, client);
    }

    public PlantCatalogClient PlantCatalogClient { get; init; }

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
