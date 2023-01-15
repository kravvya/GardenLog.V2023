using Blazored.Toast;
using GardenLogWeb;
using FluentValidation;
using GardenLogWeb.Shared.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PlantCatalog.Contract.Validators;

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

builder.Services.AddBlazoredToast();
builder.Services.AddScoped<IGardenLogToastService, GardenLogToastService>();

string serviceUrl = "";
string imageServiceUrl = "";
if (builder.HostEnvironment.IsProduction())
{
    serviceUrl = "https://plantcatalogapi-containerapp.politecoast-efa2ff8d.eastus.azurecontainerapps.io/";
    imageServiceUrl= serviceUrl;
}
else
{
    serviceUrl = "https://localhost:44304/";
    imageServiceUrl = "https://localhost:44391/";
};

builder.Services.AddHttpClient(GlobalConstants.PLANTCATALOG_API, client => client.BaseAddress = new Uri(serviceUrl));
builder.Services.AddHttpClient(GlobalConstants.IMAGEPLANTCATALOG_API, client => client.BaseAddress = new Uri(imageServiceUrl));


builder.Services.AddValidatorsFromAssemblyContaining<PlantViewModelValidator>();

builder.Services.AddBlazoredToast();




await builder.Build().RunAsync();