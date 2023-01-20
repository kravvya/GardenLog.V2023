using Microsoft.AspNetCore.Mvc;
using PlantCatalog.Contract;
using System.Net;

namespace PlantHarvest.Api.Controllers;

[Route(Routes.PlantHarvestBase)]
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
    [Route(Routes.GetHarvestCycleById)]
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
    [Route(Routes.GetIdByHarvestCycleName)]
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
    [Route(Routes.GetAllHarvestCycles)]
    [ProducesResponseType(typeof(IReadOnlyCollection<HarvestCycleViewModel>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyCollection<HarvestCycleViewModel>>> GetAllHarvestCycles()
    {
        return Ok(await _queryHandler.GetAllHarvestCycles());

    }


    [HttpPost]
    [Route(Routes.CreateHarvestCycle)]
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
    [Route(Routes.UpdateHarvestCycle)]
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
    [Route(Routes.DeleteHarvestCycle)]
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

    #region Plan Harvest Cycle
    [HttpGet()]
    [ActionName("GetPlanHarvestCyclesByHarvestCycleId")]
    [Route(Routes.GetPlanHarvestCycles)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(IReadOnlyCollection<PlanHarvestCycleViewModel>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyCollection<PlanHarvestCycleViewModel>>> GetPlanHarvestCyclesByHarvestCycleIdAsync(string harvestId)
    {
        return Ok(await _queryHandler.GetPlanHarvestCycles(harvestId));
    }

    [HttpGet()]
    [Route(Routes.GetPlanHarvestCycle)]
    [ActionName("GetPlanHarvestCycleById")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(PlanHarvestCycleViewModel), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<PlanHarvestCycle>> GetPlanHarvestCycleByIdAsync(string harvestId, string id)
    {
        var plan = await _queryHandler.GetPlanHarvestCycle(harvestId, id);

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
    [Route(Routes.CreatePlanHarvestCycle)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> PostPlanHarvestCycleAsync([FromBody] CreatePlanHarvestCycleCommand command)
    {
        try
        {
            string result = await _handler.AddPlanHarvestCycle(command);

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
    [Route(Routes.UpdatePlanHarvestCycle)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> PutPlanHarvestCycleAsync([FromBody] UpdatePlanHarvestCycleCommand command)
    {
        try
        {
            string result = await _handler.UpdatePlanHarvestCycle(command);

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
    [Route(Routes.DeletePlanHarvestCycle)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> DeletePlanHarvestCycleAsync(string harvestId, string id)
    {

        string result = await _handler.DeletePlanHarvestCycle(harvestId, id);

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
    [Route(Routes.GetPlantHarvestCycles)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(IReadOnlyCollection<PlantHarvestCycleViewModel>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyCollection<PlantHarvestCycleViewModel>>> GetPlantHarvestCycleByHarvestCycleIdAsync(string harvestId)
    {
        return Ok(await _queryHandler.GetPlantHarvestCycles(harvestId));
    }

    [HttpGet()]
    [Route(Routes.GetPlantHarvestCycle)]
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
    [Route(Routes.CreatePlantHarvestCycle)]
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
    [Route(Routes.UpdatePlantHarvestCycle)]
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
    [Route(Routes.DeletePlantHarvestCycle)]
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
