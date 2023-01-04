using Blazored.Toast;
using GardenLogWeb;
using GardenLogWeb.Shared.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddSingleton<ICacheService, CacheService>();
builder.Services.AddSingleton<IMouseService, MouseService>();
builder.Services.AddSingleton<IKeyService, KeyService>();

builder.Services.AddScoped<IPlantService, PlantService>();

builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IVerifyService, VerifyService>();
builder.Services.AddScoped<IGardenService, GardenService>();


builder.Services.AddScoped<IGardenLogToastService, GardenLogToastService>();

string serviceUrl = "";
if (builder.HostEnvironment.IsProduction())
{
    serviceUrl = "https://plantcatalogapi-containerapp.politecoast-efa2ff8d.eastus.azurecontainerapps.io/";
}
else
{
    serviceUrl = "https://localhost:44304/";
};

builder.Services.AddHttpClient("GardenLog.Server", client => client.BaseAddress = new Uri(serviceUrl));

//todo Research validators
//builder.Services.AddValidatorsFromAssemblyContaining<PlantModelValidator>();

builder.Services.AddBlazoredToast();




await builder.Build().RunAsync();
