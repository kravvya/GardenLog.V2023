using GardenLog.SharedInfrastructure.MongoDB;
using Microsoft.AspNetCore.Mvc;

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
                   return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

       
    }
}