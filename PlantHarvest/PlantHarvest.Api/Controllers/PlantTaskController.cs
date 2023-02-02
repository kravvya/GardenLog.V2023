﻿using Microsoft.AspNetCore.Mvc;

using System.Net;

namespace PlantHarvest.Api.Controllers;

[Route(HarvestRoutes.PlantTaskBase)]
[ApiController]

public class PlantTaskController : Controller
{
    private readonly IPlantTaskCommandHandler _handler;
    private readonly IPlantTaskQueryHandler _queryHandler;
    private readonly ILogger<PlantTaskController> _logger;

    public PlantTaskController(IPlantTaskCommandHandler handler, IPlantTaskQueryHandler queryHandler, ILogger<PlantTaskController> logger)
    {
        _handler = handler;
        _queryHandler = queryHandler;
        _logger = logger;
    }

    #region Plant Task

    [HttpGet()]
    [ActionName("GetTasks")]
    [Route(HarvestRoutes.GetTasks)]
    [ProducesResponseType(typeof(IReadOnlyCollection<PlantTaskViewModel>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyCollection<PlantTaskViewModel>>> GetTasks()
    {
         return Ok(await _queryHandler.GetPlantTasks());
    }

    [HttpGet()]
    [ActionName("GetActiveTasks")]
    [Route(HarvestRoutes.GetActiveTasks)]
    [ProducesResponseType(typeof(IReadOnlyCollection<PlantTaskViewModel>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyCollection<PlantTaskViewModel>>> GetActiveTasks()
    {
        return Ok(await _queryHandler.GetActivePlantTasks());
    }

    [HttpPost]
    [Route(HarvestRoutes.CreateTask)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> CreateTask([FromBody] CreatePlantTaskCommand command)
    {
        try
        {
            string result = await _handler.CreatePlantTask(command);

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
    [Route(HarvestRoutes.UpdateTask)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> UpdateTask([FromBody] UpdatePlantTaskCommand command)
    {
        try
        {
            string result = await _handler.UpdatePlantTask(command);

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
    [Route(HarvestRoutes.CompleteTask)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> CompleteTask([FromBody] UpdatePlantTaskCommand command)
    {

        string result = await _handler.CompletePlantTask(command);

        if (!string.IsNullOrWhiteSpace(result))
        {
            return Ok(true);
        }


        return BadRequest();
    }
    #endregion
}
