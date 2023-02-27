using Microsoft.AspNetCore.Mvc;
using System.Net;
using UserManagement.CommandHandlers;
using UserManagement.Contract;
using UserManagement.QueryHandlers;

namespace UserManagement.Controllers;


[Route(UserProfileRoutes.UserProfileBase)]
[ApiController]
public class UserProfileController : Controller
{
    private readonly IUserProfileCommandHandler _commadnHandler;
    private readonly IUserProfileQueryHandler _queryHandler;
    private readonly ILogger<UserProfileController> _logger;

    public UserProfileController(IUserProfileCommandHandler commadnHandler, IUserProfileQueryHandler queryHandler, ILogger<UserProfileController> logger)
    {
        _commadnHandler = commadnHandler;
        _queryHandler = queryHandler;
        _logger = logger;
    }

    [HttpGet()]
    [ActionName("GetUserProfile")]
    [Route(UserProfileRoutes.GetUserProfile)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(UserProfileViewModel), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<UserProfileViewModel>> GetUserProfile()
    {
        try
        {
            var results = await _queryHandler.GetUserProfile();

            if (results == null) return NotFound();

            return Ok(results);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpGet()]
    [ActionName("GetUserProfileById")]
    [Route(UserProfileRoutes.GetUserProfileById)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(UserProfileViewModel), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<UserProfileViewModel>> GetUserProfileById(string userProfileId)
    {
        try
        {
            var results = await _queryHandler.GetUserProfile(userProfileId);

            if (results == null) return NotFound();

            return Ok(results);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpPost()]
    [ActionName("SearchUserProfile")]
    [Route(UserProfileRoutes.SearchUserProfile)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(UserProfileViewModel), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<UserProfileViewModel>> SearchUserProfile([FromBody] SearchUserProfiles search)
    {
        try
        {
            var results = await _queryHandler.SearchForUserProfile(search);

            if (results == null) return NotFound();

            return Ok(results);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpPost()]
    [ActionName("CreateUserProfile")]
    [Route(UserProfileRoutes.CreateUserProfile)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<UserProfileViewModel>> CreateUserProfile([FromBody] CreateUserProfileCommand command)
    {
        try
        {
            var results = await _commadnHandler.CreateUserProfile(command);

            var url = Url.Action("GetUserProfileById", "UserProfileController", new { userProfileId = results });

            return Ok(results);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(ex.ParamName!, ex.Message);
            return BadRequest(ModelState);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpPut()]
    [ActionName("UpdateUserProfile")]
    [Route(UserProfileRoutes.UpdateUserProfile)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<UserProfileViewModel>> UpdateUserProfile([FromBody] UpdateUserProfileCommand command)
    {
        try
        {
            var results = await _commadnHandler.UpdateUserProfile(command);

            return results == 0 ? NotFound() : Ok(results);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(ex.ParamName!, ex.Message);
            return BadRequest(ModelState);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpDelete()]
    [ActionName("DeleteUserProfile")]
    [Route(UserProfileRoutes.DeleteUserProfile)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<UserProfileViewModel>> DeleteUserProfile(string userProfileId)
    {
        try
        {
            var results = await _commadnHandler.DeleteUserProfile(userProfileId);

            return results == 0 ? NotFound() : Ok(results);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
}
