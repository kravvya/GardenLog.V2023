
using FluentValidation;
using FluentValidation.AspNetCore;
using GardenLog.SharedInfrastructure;
using GardenLog.SharedInfrastructure.Extensions;
using GardenLog.SharedInfrastructure.MongoDB;
using Microsoft.AspNetCore.Mvc;
using PlantHarvest.Infrastructure.Data.Repositories;
using Serilog;
using Serilog.Enrichers.Span;
using System.Text.Json.Serialization;
using UserManagement.CommandHandlers;
using UserManagement.QueryHandlers;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithSpan()
    //.WriteTo.Console(new JsonFormatter())
    .WriteTo.Console(Serilog.Events.LogEventLevel.Information)
    .CreateLogger();
// .CreateBootstrapLogger();
Log.Information("Starting up UserProfile.Api");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Host.UseLogging();

    //https://github.com/FluentValidation/FluentValidation/issues/1965
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddFluentValidationClientsideAdapters();
    builder.Services.AddValidatorsFromAssemblyContaining<CreateUserProfileCommandValidator>();

    builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
        options.InvalidModelStateResponseFactory = context =>
        {
            return new BadRequestObjectResult(context.ModelState);
        }
    ).AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddSingleton<IConfigurationService, ConfigurationService>();
    builder.Services.AddSingleton<IUnitOfWork, MongoDbContext>();

    builder.Services.AddSingleton<IUserProfileRepository, UserProfileRepository>();
    builder.Services.AddSingleton<IGardenRepository, GardenRepository>();

    builder.Services.AddScoped<IUserProfileCommandHandler, UserProfileCommandHandler>();
    builder.Services.AddScoped<IUserProfileQueryHandler, UserProfileQueryHandler>();

    builder.Services.AddScoped<IGardenCommandHandler, GardenCommandHandler>();
    builder.Services.AddScoped<IGardenQueryHandler, GardenQueryHandler>();

    builder.Services.AddCors(options =>
    {
        options.AddGlWebPolicy();
    });


    //TODO Add Healthchecks!!!!

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseAuthorization();

    app.UseCors("glWebPolicy");

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception in UserProfile.Api");
}
finally
{
    Log.Information("Shut down complete for UserProfile.Api");
    Log.CloseAndFlush();
}
public partial class Program { }