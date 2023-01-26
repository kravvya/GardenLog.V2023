using Microsoft.AspNetCore.Mvc;
using PlantCatalog.Contract;
using System.Net;

namespace PlantHarvest.Api.Controllers;

[Route(HarvestRoutes.PlantHarvestBase)]
[ApiController]

public class HarvestCycleController : Controller
{
    private readonly IHarvestCommandHandler _handler;
    private readonly IHarvestQueryHandler _queryHandler;
    private readonly ILogger<HarvestCycleController> _logger;

    public HarvestCycleController(IHarvestCommandHandler handler, IHarvestQueryHandler queryHandler, ILogger<HarvestCycleController> logger)
	{
        _handler = handler;
        _queryHandler = queryHandler;
        _logger = logger;
    }

    #region Harvest Cycle
    [HttpGet()]
    [ActionName("GetHarvestCycleById")]
    [Route(HarvestRoutes.GetHarvestCycleById)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(HarvestCycleViewModel), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<HarvestCycleViewModel>> GetHarvestCycleById(string id)
    {
        try
        {
            var harvest = await _queryHandler.GetHarvestCycleByHarvestCycleId(id);

            if (harvest != null)
            {
                return Ok(harvest);
            }
            else
            {
                return NotFound();
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"Exceptin getting harvest {id}", ex);
            throw;
        }
    }

    [HttpGet()]
    [ActionName("GetIdByHarvestCycleName")]
    [Route(HarvestRoutes.GetIdByHarvestCycleName)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<string>> GetIdByHarvestCycleName(string name)
    {
        var id = await _queryHandler.GetHarvestCycleIdByHarvestCycleName(name);

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
    [ActionName("GetAllHarvestCycles")]
    [Route(HarvestRoutes.GetAllHarvestCycles)]
    [ProducesResponseType(typeof(IReadOnlyCollection<HarvestCycleViewModel>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyCollection<HarvestCycleViewModel>>> GetAllHarvestCycles()
    {
        return Ok(await _queryHandler.GetAllHarvestCycles());

    }


    [HttpPost]
    [Route(HarvestRoutes.CreateHarvestCycle)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> PostPlantAsync([FromBody] CreateHarvestCycleCommand command)
    {
        try
        {
            string result = await _handler.CreateHarvestCycle(command);

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
    [Route(HarvestRoutes.UpdateHarvestCycle)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> PutPlantAsync([FromBody] UpdateHarvestCycleCommand command)
    {
        try
        {
            string result = await _handler.UpdateHarvestCycle(command);

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
    [Route(HarvestRoutes.DeleteHarvestCycle)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> DeleteHarvestCycleAsync(string id)
    {

        string result = await _handler.DeleteHarvestCycle(id);

        if (!string.IsNullOrWhiteSpace(result))
        {
            return Ok(true);
        }


        return BadRequest();
    }
    #endregion

    #region Plant Harvest Cycle
    [HttpGet()]
    [ActionName("GetPlantHarvestCycleByHarvestCycleId")]
    [Route(HarvestRoutes.GetPlantHarvestCycles)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(IReadOnlyCollection<PlantHarvestCycleViewModel>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyCollection<PlantHarvestCycleViewModel>>> GetPlantHarvestCycleByHarvestCycleIdAsync(string harvestId)
    {
        return Ok(await _queryHandler.GetPlantHarvestCycles(harvestId));
    }

    [HttpGet()]
    [ActionName("GetPlantHarvestCyclesByPlant")]
    [Route(HarvestRoutes.GetPlantHarvestCyclesByPlant)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(IReadOnlyCollection<PlantHarvestCycleViewModel>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyCollection<PlantHarvestCycleViewModel>>> GetPlantHarvestCyclesByPlant(string plantId)
    {
        return Ok(await _queryHandler.GetPlantHarvestCyclesByPlantId(plantId));
    }

    [HttpGet()]
    [Route(HarvestRoutes.GetPlantHarvestCycle)]
    [ActionName("GetPlantHarvestCycleById")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(PlantHarvestCycleViewModel), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<PlantHarvestCycle>> GetPlantHarvestCycleByIdAsync(string harvestId, string id)
    {
        var plan = await _queryHandler.GetPlantHarvestCycle(harvestId, id);

        if (plan != null)
        {
            return Ok(plan);
        }
        else
        {
            return NotFound();
        }
    }

    [HttpPost()]
    [Route(HarvestRoutes.CreatePlantHarvestCycle)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> PostPlantHarvestCycleAsync([FromBody] CreatePlantHarvestCycleCommand command)
    {
        try
        {
            string result = await _handler.AddPlantHarvestCycle(command);

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
    [Route(HarvestRoutes.UpdatePlantHarvestCycle)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> PutPlantHarvestCycleAsync([FromBody] UpdatePlantHarvestCycleCommand command)
    {
        try
        {
            string result = await _handler.UpdatePlantHarvestCycle(command);

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
    [Route(HarvestRoutes.DeletePlantHarvestCycle)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> DeletePlantHarvestCycleAsync(string harvestId, string id)
    {

        string result = await _handler.DeletePlantHarvestCycle(harvestId, id);

        if (!string.IsNullOrWhiteSpace(result))
        {
            return Ok(true);
        }

        return BadRequest();
    }
    #endregion
}
