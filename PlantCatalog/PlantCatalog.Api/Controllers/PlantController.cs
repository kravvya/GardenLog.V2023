using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;

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
        try
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
        catch (Exception ex)
        {
            _logger.LogCritical($"Exceptin getting plant {id}", ex);
            throw;
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

    #region Grow Instructions
    [HttpGet()]
    [ActionName("GetPlantGrowInstructionsByPlantId")]
    [Route(Routes.GetPlantGrowInstructions)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(IReadOnlyCollection<PlantGrowInstructionViewModel>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyCollection<PlantGrowInstructionViewModel>>> GetPlantGrowInstructionsByPlantIdAsync(string plantId)
    {
        return Ok(await _queryHandler.GetPlantGrowInstructions(plantId));
    }

    [HttpGet()]
    [Route(Routes.GetPlantGrowInstruction)]
    [ActionName("GetPlantGrowInstructionById")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(PlantGrowInstructionViewModel), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<PlantGrowInstruction>> GetPlantGrowInstructionByIdAsync(string plantId, string id)
    {
        var grow = await _queryHandler.GetPlantGrowInstruction(plantId, id);

        if (grow != null)
        {
            return Ok(grow);
        }
        else
        {
            return NotFound();
        }
    }

    [HttpPost()]
    [Route(Routes.CreatePlantGrowInstruction)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> PostPlantGrowInstructionAsync([FromBody] CreatePlantGrowInstructionCommand command)
    {
        try
        {
            string result = await _handler.AddPlantGrowInstruction(command);

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

    // PUT: api/Plants/{id}/GrowInstruction/{plantGrowInstructionId}
    [HttpPut()]
    [Route(Routes.UpdatePlantGrowInstructions)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> PutPlantGrowInstructionsAsync([FromBody] UpdatePlantGrowInstructionCommand command)
    {
        try
        {
            string result = await _handler.UpdatePlantGrowInstruction(command);

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
    [Route(Routes.DeletePlantGrowInstructions)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> DeletePlantGrowInstructionAsync(string plantId, string id)
    {

        string result = await _handler.DeletePlantGrowInstruction(plantId, id);

        if (!string.IsNullOrWhiteSpace(result))
        {
            return Ok(true);
        }

        return BadRequest();
    }
    #endregion

    #region Plant Variety
    [HttpGet()]
    [ActionName("GetAllPlantVarieties")]
    [Route(Routes.GetAllPlantVarieties)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(IReadOnlyCollection<PlantVarietyViewModel>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IList<PlantVarietyViewModel>>> GetAllPlantVarieties()
    {
        return Ok(await _queryHandler.GetPlantVarieties());
    }

    [HttpGet()]
    [ActionName("GetPlantVarietiesByPlantId")]
    [Route(Routes.GetPlantVarieties)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(IReadOnlyCollection<PlantVarietyViewModel>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IList<PlantVarietyViewModel>>> GetPlantVarietiesByPlantIdAsync(string plantId)
    {
        return Ok(await _queryHandler.GetPlantVarieties(plantId));
    }

    [HttpGet()]
    [ActionName("GetPlantVarietyById")]
    [Route(Routes.GetPlantVariety)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(IReadOnlyCollection<PlantVarietyViewModel>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<PlantVarietyViewModel>> GetPlantVarietyByIdAsync(string plantId, string id)
    {
        var variety = await _queryHandler.GetPlantVariety(plantId, id);

        if (variety != null)
        {
            return Ok(variety);
        }
        else
        {
            return NotFound();
        }
    }

    [HttpPost()]
    [Route(Routes.CreatePlantVariety)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> PostPlantVarietyAsync([FromBody] CreatePlantVarietyCommand command)
    {
        try
        {
            string result = await _handler.AddPlantVariety(command);

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
    [Route(Routes.UpdatePlantVariety)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> PutPlantVarietyAsync([FromBody] UpdatePlantVarietyCommand command)
    {
        try
        {
            string result = await _handler.UpdatePlantVariety(command);

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
    [Route(Routes.DeletePlantVariety)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> DeletelantAsync(string plantId, string id)
    {

        string result = await _handler.DeletePlantVariety(plantId, id);

        if (!string.IsNullOrWhiteSpace(result))
        {
            return Ok(true);
        }

        return BadRequest();
    }
    #endregion
}
