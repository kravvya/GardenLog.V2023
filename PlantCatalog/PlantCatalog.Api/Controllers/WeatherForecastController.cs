using GardenLog.SharedInfrastructure.MongoDB;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using PlantCatalog.Domain.PlantAggregate;

namespace PlantCatalog.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IMongoDBContext<Plant> _context;
        private readonly IPlantRepository _repository;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IMongoDBContext<Plant> context, IPlantRepository repository)
        {
            _logger = logger;
            _context = context;
            _repository = repository;
        }

         [HttpGet("/weather")]
        public async Task<IEnumerable<WeatherForecast>> Get2()
        {
            var plant = Plant.Create("Kale", "Good old kale", "Green", PlantTypeEnum.Vegetable, PlantLifecycleEnum.Cool, MoistureRequirementEnum.ConsistentMoisture, LightRequirementEnum.PartShade, 
                GrowToleranceEnum.LightFrost| GrowToleranceEnum.HardFrost, "Give plenty of space", 5);

            _repository.Add(plant);
          
            var response = await _repository.SaveChangesAsync();


            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("/weather3")]
        public async Task<IEnumerable<WeatherForecast>> Get4()
        {
            var plant = Plant.Create("Kale", "Good old kale", "Green", PlantTypeEnum.Vegetable, PlantLifecycleEnum.Cool, MoistureRequirementEnum.ConsistentMoisture, LightRequirementEnum.PartShade,
                GrowToleranceEnum.Unspecified, "Give plenty of space", 5);

            _repository.Add(plant);

            var response = await _repository.SaveChangesAsync();


            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("/plant")]
        public async Task<Plant> Get()
        {

            var plant = await _repository.GetByIdAsync("540ad166-3de0-4c1b-bbdd-a4a86d21daa9");

            return plant;
        }

        [HttpGet("/plant2")]
        public async Task<Plant> Get5()
        {

            var plant = await _repository.GetByIdAsync("7d6508e9-fba8-4c0d-9c03-5ee25df0e465");

            return plant;
        }
        

        //[HttpGet(Name = "GetWeatherForecastV2")]
        //public async Task<WeatherForecast> GetV2()
        //{
        //   // 




        //    return new WeatherForecast();
        //}
    }
}