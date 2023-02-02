using PlantHarvest.IntegrationTest.Clients;
using System.Net.Http;

namespace PlantHarvest.IntegrationTest.Fixture;

public class PlantHarvestServiceFixture : PlantHarvestApplicationFactory<Program>, IDisposable
{
    private bool _disposedValue;
    private readonly PlantHarvestApplicationFactory<Program> _factory;
    

    public PlantHarvestServiceFixture()
    {
        _factory= new PlantHarvestApplicationFactory<Program>();
        _factory.ConfigureAwait(true);

        FixtureId = Guid.NewGuid().ToString();
        
        var client = _factory.CreateClient();

        client.DefaultRequestHeaders.Add("RequestUser", "86377291-980f-4af2-8608-39dbbf7e09e1");

        PlantHarvestClient = new PlantHarvestClient(client.BaseAddress, client);
        WorkLogClient = new WorkLogClient(client.BaseAddress, client);
        PlantTaskClient = new PlantTaskClient(client.BaseAddress, client);
    }

    public PlantHarvestClient PlantHarvestClient { get; init; }
    public WorkLogClient WorkLogClient { get; init; }
    public PlantTaskClient PlantTaskClient { get; init; }
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
