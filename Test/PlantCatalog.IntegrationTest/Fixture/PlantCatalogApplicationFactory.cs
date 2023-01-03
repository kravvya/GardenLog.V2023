using Microsoft.AspNetCore.Hosting;

namespace PlantCatalog.IntegrationTest.Fixture;

public class PlantCatalogApplicationFactory<TEntryPoint> : WebApplicationFactory<Program> where TEntryPoint : Program
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        //base.ConfigureWebHost(builder); 
    }
}
