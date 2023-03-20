using Microsoft.AspNetCore.Mvc;
using System.Net;
using UserManagement.CommandHandlers;
using UserManagement.Contract;

namespace UserManagement.Controllers;

[Route(GardenRoutes.GardenBase)]
[ApiController]
public class ContactController : Controller
{
    private readonly IContactCommandHandler _commadnHandler;
    private readonly ILogger<ContactController> _logger;

    public ContactController(IContactCommandHandler commadnHandler, ILogger<ContactController> logger)
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
        var result =  await _commadnHandler.SendEmail(command);
        if (!result) return BadRequest();

       return Ok();

    }

    #endregion

 
}
