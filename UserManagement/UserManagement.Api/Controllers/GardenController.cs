﻿using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Reflection.Metadata;
using UserManagement.CommandHandlers;
using UserManagement.Contract;
using UserManagement.QueryHandlers;

namespace UserManagement.Controllers;

[Route(GardenRoutes.GardenBase)]
[ApiController]
public class GardenController : Controller
{
    private readonly IGardenCommandHandler _commadnHandler;
    private readonly IGardenQueryHandler _queryHandler;
    private readonly ILogger<GardenController> _logger;

    public GardenController(IGardenCommandHandler commadnHandler, IGardenQueryHandler queryHandler, ILogger<GardenController> logger)
    {
        _commadnHandler = commadnHandler;
        _queryHandler = queryHandler;
        _logger = logger;
    }

    #region Garden

    [HttpGet()]
    [ActionName("GetGardens")]
    [Route(GardenRoutes.GetGardens)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(GardenViewModel), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<GardenViewModel>> GetGardens()
    {

        var results = await _queryHandler.GetGardens();

        if (results == null) return NotFound();

        return Ok(results);

    }

    [HttpGet()]
    [ActionName("GetGarden")]
    [Route(GardenRoutes.GetGarden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(GardenViewModel), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<GardenViewModel>> GetGarden(string gardenId)
    {
        try
        {
            var results = await _queryHandler.GetGarden(gardenId);

            if (results == null) return NotFound();

            return Ok(results);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }


    [HttpGet()]
    [ActionName("GetGardenByName")]
    [Route(GardenRoutes.GetGardenByName)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(GardenViewModel), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<GardenViewModel>> GetGardenByName(string gardenName)
    {
        try
        {
            var results = await _queryHandler.GetGardenByName(gardenName);

            if (results == null) return NotFound();

            return Ok(results);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }


    [HttpPost()]
    [ActionName("CreateGarden")]
    [Route(GardenRoutes.CreateGarden)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(GardenViewModel), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<GardenViewModel>> CreateGarden([FromBody] CreateGardenCommand command)
    {           
        try
        {
            var result = await _commadnHandler.CreateGarden(command);

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
    [ActionName("UpdateGarden")]
    [Route(GardenRoutes.UpdateGarden)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<GardenViewModel>> UpdateGarden([FromBody] UpdateGardenCommand command)
    {
        try
        {
            var results = await _commadnHandler.UpdateGarden(command);

            return results == 0 ? NotFound() : Ok(results);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpDelete()]
    [ActionName("DeleteGarden")]
    [Route(GardenRoutes.DeleteGarden)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<GardenViewModel>> DeleteGarden(string gardenId)
    {
        try
        {
            var results = await _commadnHandler.DeleteGarden(gardenId);

            return results == 0 ? NotFound() : Ok(results);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
    #endregion

    #region Garden Bed
    [HttpGet()]
    [ActionName("GetGardenBeds")]
    [Route(GardenRoutes.GetGardenBeds)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(GardenViewModel), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<GardenViewModel>> GetGardenBeds(string gardenId)
    {
        try
        {
            var results = await _queryHandler.GetGardenBeds(gardenId);

            if (results == null) return NotFound();

            return Ok(results);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpGet()]
    [ActionName("GetGardenBed")]
    [Route(GardenRoutes.GetGardenBed)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(GardenViewModel), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<GardenViewModel>> GetGardenBeds(string gardenId, string id)
    {
        try
        {
            var results = await _queryHandler.GetGardenBed(gardenId, id);

            if (results == null) return NotFound();

            return Ok(results);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }


    [HttpPost()]
    [ActionName("CreateGardenBed")]
    [Route(GardenRoutes.CreateGardenBed)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<GardenViewModel>> CreateGardenBed([FromBody] CreateGardenBedCommand command)
    {
        try
        {
            var results = await _commadnHandler.CreateGardenBed(command);

            return Ok(results);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(ex.ParamName, ex.Message);
            return BadRequest(ModelState);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpPut()]
    [ActionName("UpdateGardenBed")]
    [Route(GardenRoutes.UpdateGardenBed)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<GardenBedViewModel>> UpdateGardenBed([FromBody] UpdateGardenBedCommand command)
    {
        try
        {
            var results = await _commadnHandler.UpdateGardenBed(command);

            return results == 0 ? NotFound() : Ok(results);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(ex.ParamName, ex.Message);
            return BadRequest(ModelState);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpDelete()]
    [ActionName("DeleteGardenBed")]
    [Route(GardenRoutes.DeleteGardenBed)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<GardenBedViewModel>> DeleteGardenBed(string gardenId, string id)
    {
        try
        {
            var results = await _commadnHandler.DeleteGardenBed(gardenId, id);

            return results == 0 ? NotFound() : Ok(results);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
    #endregion
}
