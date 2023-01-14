using PlantCatalog.IntegrationTest.Fixture;

namespace ImageCatalog.IntegrationTest.Fixture;

public class ImageCatalogServiceFixture : ImageCatalogApplicationFactory<Program>, IDisposable
{
    private bool _disposedValue;
    private readonly ImageCatalogApplicationFactory<Program> _factory;
    

    public ImageCatalogServiceFixture()
    {
        _factory= new ImageCatalogApplicationFactory<Program>();
        _factory.ConfigureAwait(true);

        FixtureId = Guid.NewGuid().ToString();
        
        var client = _factory.CreateClient();

        ImageCatalogClient = new ImageCatalogClient(client.BaseAddress, client);
        FileCatalogClient = new FileCatalogClient(client.BaseAddress, client);
    }

    public ImageCatalogClient ImageCatalogClient { get; init; }
    public FileCatalogClient FileCatalogClient { get; init; }
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
