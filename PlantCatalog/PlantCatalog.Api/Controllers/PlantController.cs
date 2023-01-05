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
    [HttpGet()]
    [ActionName("GetPlantById")]
    [Route(Routes.GetPlantById)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(PlantViewModel), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<PlantViewModel>> GetPlantByIdAsync(string id)
    {
        var plant = await _queryHandler.GetPlantByPlantId(id);

        if (plant != null)
        {
            return Ok(plant);
        }
        else
        {
            return NotFound();
        }
    }

    [HttpGet()]
    [ActionName("GetIdByPlantName")]
    [Route(Routes.GetIdByPlantName)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<PlantViewModel>> GetPlantIdByPlantName(string name)
    {
        var id = await _queryHandler.GetPlantIdByPlantName(name);

        if (!string.IsNullOrWhiteSpace(id))
        {
            return Ok(id);
        }
        else
        {
            return NotFound();
        }
    }

    [HttpGet()]
    [ActionName("GetAllPlants")]
    [Route(Routes.GetAllPlants)]
    [ProducesResponseType(typeof(IReadOnlyCollection<PlantViewModel>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyCollection<PlantViewModel>>> GetAllPlantsAsync()
    {
        return Ok(await _queryHandler.GetAllPlants());

    }
    // POST: api/Plants
    [HttpPost]
    [Route(Routes.CreatePlant)]
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

    [HttpPut()]
    [Route(Routes.UpdatePlant)]
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

    [HttpDelete()]
    [Route(Routes.DeletePlant)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> DeletelantAsync(string id)
    {

        string result = await _handler.DeletePlant(id);

        if (!string.IsNullOrWhiteSpace(result))
        {
            return Ok(true);
        }


        return BadRequest();
    }
    #endregion
}
