using UserManagement.IntegrationTest.Clients;


namespace UserManagement.IntegrationTest.Fixture;

public class UserManagementServiceFixture : UserManagementApplicationFactory<Program>, IDisposable
{
    private bool _disposedValue;
    private readonly UserManagementApplicationFactory<Program> _factory;
    

    public UserManagementServiceFixture()
    {
        _factory= new UserManagementApplicationFactory<Program>();
        _factory.ConfigureAwait(true);

        FixtureId = Guid.NewGuid().ToString();
        
        var client = _factory.CreateClient();

        GardenClient = new GardenClient(client.BaseAddress, client);
        UserProfileClient = new UserProfileClient(client.BaseAddress, client);
    }

    public GardenClient GardenClient { get; init; }
    public UserProfileClient UserProfileClient { get; init; }
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
