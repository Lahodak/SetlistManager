using Microsoft.AspNetCore.Mvc;
using SetlistManager. Api.Services;
using SetlistManager.Common.Models;
using SetlistManager.Business.Services;

namespace SetlistManager. Api.Controllers;

[Route("api/users")]
public class UsersController : BaseController
{
    private readonly IUserService _userService;
    private readonly ISetlistsService _setlistsService;
    private readonly ICurrentUserContext _currentUserContext;

    public UsersController(IUserService userService, ISetlistsService setlistsService, ICurrentUserContext currentUserContext)
    {
        _userService = userService;
        _setlistsService = setlistsService;
        _currentUserContext = currentUserContext;
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(UserModel model)
    {
        await _userService.UpdateUserAsync(model);
        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<UserModel>> GetUser()
    {        
        var userId = _currentUserContext.GetCurrentUserId();
        
        if (userId is null)        
            return Unauthorized();
        
        return Ok(await _userService.GetCurrentUserAsync((int)userId));
    }

    [HttpGet("{id:int}/setlists")]
    public async Task<ActionResult<List<SetlistModel>>> GetUserSetlists(int id)
    {
        return Ok(await _setlistsService.GetAllSetlistsOfUserAsync(id));
    }
}