
using GrowConditions.Contract.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace GrowConditions.Api.Controllers;

[Route(WeatherRoutes.WeatherBase)]
[ApiController]
public class WeatherController : ControllerBase
{
    private readonly ILogger<WeatherController> _logger;
    private readonly IWeatherCommandHandler _weatherCommandHandler;
    private readonly IWeatherQueryHandler _weatherQueryHandler;

    public WeatherController(ILogger<WeatherController> logger, IWeatherCommandHandler weatherCommandHandler, IWeatherQueryHandler weatherQueryHandler)
    {
        _logger = logger;
        _weatherCommandHandler = weatherCommandHandler;
        _weatherQueryHandler = weatherQueryHandler;
    }

    [HttpGet(WeatherRoutes.GetLastWeatherUpdate)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(WeatherUpdateViewModel), (int)HttpStatusCode.OK)]
    public async Task<ActionResult> GetLastWeatherUpdate(string gardenId)
    {
        var weather = await _weatherQueryHandler.GetLastWeatherUpdate(gardenId);
        if (weather == null)
        {
            return NotFound();
        }
        return Ok(weather);
    }

    [HttpGet(WeatherRoutes.GetHistoryOfWeatherUpdates)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(IList<WeatherUpdateViewModel>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult> GetHistoryOfWeatherUpdates(string gardenId, int numberOfDays)
    {
        var weather = await _weatherQueryHandler.GetHistoryOfWeatherUpdates(gardenId, numberOfDays);
        if (weather == null)
        {
            return NotFound();
        }
        return Ok(weather);
    }


    [HttpGet(WeatherRoutes.Run)]
    [ProducesResponseType((int)HttpStatusCode.Accepted)]
    public async Task<ActionResult> RunAsync()
    {
        try
        {
            _logger.LogInformation("Received request to run weather cycle");
            await _weatherCommandHandler.GetWeatherUpdates();
            return Accepted();
        }
        catch (Exception ex)
        {
            _logger.LogError("Exception running weather cycle time", ex);
            return Problem(ex.Message);
        }
    }
}
