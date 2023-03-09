using GardenLog.SharedInfrastructure;
using GardenLogAdminConsole.Harvest;
using GardenLogAdminConsole.Images;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .AddUserSecrets(typeof(AdminImageProcessor).Assembly)
    .Build();

ILoggerFactory loggerFactory =
    LoggerFactory.Create(builder =>
        builder.AddSimpleConsole(options =>
        {
            options.IncludeScopes = true;
            options.SingleLine = true;
            options.TimestampFormat = "HH:mm:ss ";
        }));

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

//AdminImageProcessor processor = new AdminImageProcessor();
//var changes = await processor.UpdateAllImagesWithRelatedEntities(config, loggerFactory);

AdminPlantHarvestProcessor processor = new();
//var changes = processor.UpdateAllPlantHarvestWithSpacingInInches(config, loggerFactory);

var changes = AdminPlantHarvestProcessor.UpdateAllPlantHarvestWithPlantsPerFoot(config, loggerFactory);

Console.WriteLine($"Performed {changes} changes");



