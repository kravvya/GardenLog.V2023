using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace PlantCatalog.Api.Controllers;

[Route(Routes.PlantCatalogBase)]
[ApiController]
public class PlantController : Controller
{
    private readonly IPlantCommandHandler _handler;
    private readonly IPlantQueryHandler _queryHandler;
    private readonly ILogger<PlantController> _logger;

    public PlantController(IPlantCommandHandler handler, IPlantQueryHandler queryHandler, ILogger<PlantController> logger)
    {
        _handler = handler;
        _queryHandler = queryHandler;
        _logger = logger;
    }


    #region Plant

    [HttpGet(Name = "GetAllPlants")]
    [ProducesResponseType(typeof(IReadOnlyCollection<PlantViewModel>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyCollection<PlantViewModel>>> GetAllPlantsAsync()
    {
        return Ok(await _queryHandler.GetAllPlants());

    }
    // POST: api/Plants
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> PostPlantAsync([FromBody] CreatePlantCommand command)
    {
        try
        {
            string result = await _handler.CreatePlant(command);

            if (!string.IsNullOrWhiteSpace(result))
            {
                return Ok(result);
            }
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(ex.ParamName, ex.Message);
            return BadRequest(ModelState);
        }

        return BadRequest();
    }

    // PUT: api/Plants/{id}
    [HttpPut("{id}")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> PutPlantAsync([FromBody] UpdatePlantCommand command)
    {
        try
        {
            string result = await _handler.UpdatePlant(command);

            if (!string.IsNullOrWhiteSpace(result))
            {
                return Ok(result);
            }
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(ex.ParamName, ex.Message);
            return BadRequest(ModelState);
        }

        return BadRequest();
    }
    #endregion
}
