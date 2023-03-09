using Microsoft.AspNetCore.Mvc;
using System.Net;
using UserManagement.CommandHandlers;
using UserManagement.Contract;

namespace UserManagement.Controllers;

[Route(GardenRoutes.GardenBase)]
[ApiController]
public class Contactontroller : Controller
{
    private readonly IContactCommandHandler _commadnHandler;
    private readonly ILogger<Contactontroller> _logger;

    public Contactontroller(IContactCommandHandler commadnHandler, ILogger<Contactontroller> logger)
    {
        _commadnHandler = commadnHandler;
        _logger = logger;
    }

    #region Garden

    [HttpPost()]
    [ActionName("SendEmail")]
    [Route(UserProfileRoutes.SendEmail)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<ActionResult> SendEmail([FromBody] SendEmailCommand command)
    {
        var result = await _commadnHandler.SendEmail(command);
        if (!result) return BadRequest();

       return Ok();

    }

    #endregion

 
}
