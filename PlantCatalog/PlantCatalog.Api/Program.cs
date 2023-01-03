using FluentValidation;
using FluentValidation.AspNetCore;
using GardenLog.SharedInfrastructure;
using GardenLog.SharedInfrastructure.Extensions;
using GardenLog.SharedInfrastructure.MongoDB;
using Microsoft.AspNetCore.Identity;
using PlantCatalog.Contract.Validators;
using PlantCatalog.Domain.PlantAggregate;
using PlantCatalog.Infrustructure.Data;
using PlantCatalog.Infrustructure.Data.Repositories;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Formatting.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithSpan()
    //.WriteTo.Console(new JsonFormatter())
    .WriteTo.Console(Serilog.Events.LogEventLevel.Information)
    .CreateLogger();
   // .CreateBootstrapLogger();
Log.Information("Starting up PlantCatalog.Api");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Host.UseLogging();

    //https://github.com/FluentValidation/FluentValidation/issues/1965
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddFluentValidationClientsideAdapters();
    builder.Services.AddValidatorsFromAssemblyContaining<CreatePlantCommandValidator>();

    builder.Services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
      

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddSingleton<IConfigurationService, ConfigurationService>();
    builder.Services.AddSingleton<IMongoDBContext<Plant>, PlantDBContext>();

    builder.Services.AddScoped<IPlantRepository, PlantRepository>();
    builder.Services.AddScoped<IPlantCommandHandler, PlantCommandHandler>();
    builder.Services.AddScoped<IPlantQueryHandler, PlantQueryHandler>();

    //TODO Add Healthchecks!!!!

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    //Aapp Container ingress is EntityHandling HTTPs redirects. This is not needed.
    //app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception in PlantCatalog.Api");
}
finally
{
    Log.Information("Shut down complete for PlantCatalog.Api");
    Log.CloseAndFlush();
}
public partial class Program { }