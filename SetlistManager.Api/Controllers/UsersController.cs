using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SetlistManager.Api.Services;
using SetlistManager.Business.Options;
using SetlistManager.Business.Services;
using SetlistManager.Common.Genius.Models;
using SetlistManager.Common.Models;

namespace SetlistManager. Api.Controllers;

[Route("api/users")]
public class UsersController : BaseController
{
    private readonly IUserService _userService;
    private readonly ISetlistsService _setlistsService;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IGeniusAuthService _geniusAuthService;
    private readonly IOptions<AppOptions> _appOptions;

    public UsersController(IUserService userService, ISetlistsService setlistsService, ICurrentUserContext currentUserContext,
        IGeniusAuthService geniusAuthService, IOptions<AppOptions> appOptions)
    {
        _appOptions = appOptions;
        _userService = userService;
        _setlistsService = setlistsService;
        _currentUserContext = currentUserContext;
        _geniusAuthService = geniusAuthService;
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

        return Ok(await _userService.GetCurrentUserAsync(userId.Value));
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
        if (grantResultModel is null)
            return BadRequest();
        
        var user = await _userService.GetUserByTempSalt(grantResultModel.State);       

        if(user is null)
            return NotFound("User not found");

        var resultAccessTokenModel = await _geniusAuthService.ExchangeGeniusCode(grantResultModel.Code);

        if (resultAccessTokenModel is null || resultAccessTokenModel.AccessToken is null)
            return BadRequest();

        TokenCreateModel tokenModel = new()
        {
            Provider = ProviderEnum.Genius,
            AccessToken = resultAccessTokenModel.AccessToken,
            RefreshToken = null
        };

        await _userService.AddUserTokenAsync(user.Id, tokenModel);

        return Redirect(_appOptions.Value.UserPortalUrl);
    }
}