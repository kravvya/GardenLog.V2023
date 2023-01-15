using GardenLog.SharedInfrastructure.MongoDB;
using ImageCatalog.Api.CommandHandlers;
using ImageCatalog.Api.QueryHandlers;
using Serilog;
using Serilog.Enrichers.Span;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithSpan()
    .WriteTo.Console()
    .CreateLogger();

Log.Information("Starting up Image Catalog.Api");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseLogging();
    // Add services to the container.
    builder.Services.AddHttpClient();

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddSingleton<IConfigurationService, ConfigurationService>();
    builder.Services.AddSingleton<IUnitOfWork, MongoDbContext>();

    builder.Services.AddSingleton<IFileRepository, BlobRepository>();
    builder.Services.AddSingleton<IImageRepository, ImageRepository>();

    builder.Services.AddScoped<IImageQueryHandler, ImageQueryHandler>();
    builder.Services.AddScoped<IImageCommandHandler, ImageCommandHandler>();

    builder.Services.AddScoped<IFileCommandHandler, FileCommandHandler>();


    builder.Services.AddCors(options =>
    {
        options.AddGlWebPolicy();
    });

    //TODO Add Healthchecks!!!!

    var app = builder.Build();

    Log.Information("Completed builder.Build");

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    elese{
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    //Aapp Container ingress is EntityHandling HTTPs redirects. This is not needed.
    //app.UseHttpsRedirection();

  //  app.UseAuthorization();

    app.UseCors("glWebPolicy");

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    string type = ex.GetType().Name;
    if (type.Equals("StopTheHostException", StringComparison.Ordinal))
    {
        throw;
    }

    Log.Fatal(ex, "Unhandled exception in ImageCatalog.Api");
}
finally
{
    Log.Information("Shut down complete for ImageCatalog.Api");
    Log.CloseAndFlush();
}

public partial class Program { }