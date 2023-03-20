using Microsoft.AspNetCore.Mvc;
using System.Net;
using UserManagement.CommandHandlers;
using UserManagement.Contract;

namespace UserManagement.Controllers;

[Route(GardenRoutes.GardenBase)]
[ApiController]
public class NotificationController : Controller
{
    private readonly INotificationCommandHandler _commadnHandler;
    private readonly ILogger<NotificationController> _logger;

    public NotificationController(INotificationCommandHandler commadnHandler, ILogger<NotificationController> logger)
    {
        _commadnHandler = commadnHandler;
        _logger = logger;
    }

    #region Garden

    [HttpGet()]
    [ActionName("WeeklyTasks")]
    [Route(UserProfileRoutes.WeeklyTasks)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<ActionResult> WeeklyTasks()
    {
        var result =  await _commadnHandler.PublishWeeklyTasks();
        if (!result) return BadRequest();

       return Ok();

    }

    [HttpGet()]
    [ActionName("PastDueTasks")]
    [Route(UserProfileRoutes.PastDueTasks)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<ActionResult> PastDueTasks()
    {
        var result = await _commadnHandler.PublishPastDueTasks();
        if (!result) return BadRequest();

        return Ok();

    }

    #endregion


}
