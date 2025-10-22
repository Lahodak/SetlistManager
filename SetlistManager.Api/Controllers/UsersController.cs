using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SetlistManager. Api.Services;
using SetlistManager.Business.Services;
using SetlistManager.Common.Genius.Models;
using SetlistManager.Common.Models;
using System.Net.Http;

namespace SetlistManager. Api.Controllers;

[Route("api/users")]
public class UsersController : BaseController
{
    private readonly IUserService _userService;
    private readonly ISetlistsService _setlistsService;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IHttpClientFactory _httpClientFactory;

    public UsersController(IUserService userService, ISetlistsService setlistsService, ICurrentUserContext currentUserContext, IHttpClientFactory httpClientFactory)
    {
        _userService = userService;
        _setlistsService = setlistsService;
        _currentUserContext = currentUserContext;
        _httpClientFactory = httpClientFactory;
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
        
        return Ok(await _userService.GetCurrentUserAsync((int)userId));
    }

    [HttpGet("{id:int}/setlists")]
    public async Task<ActionResult<List<SetlistModel>>> GetUserSetlists(int id)
    {
        return Ok(await _setlistsService.GetAllSetlistsOfUserAsync(id));
    }

    [AllowAnonymous]
    [HttpGet("tokens")]
    public async Task<ActionResult> AddUserToken([FromQuery] GrantAccessTokenResultModel grantResultModel)
    {
        var userId = int.Parse(grantResultModel.State);


        return Ok();
    }
}