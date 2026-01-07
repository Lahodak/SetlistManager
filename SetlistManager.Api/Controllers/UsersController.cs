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
        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<UserModel>>> GetUsers(PagedRequest pagedRequest)
    {
        return Ok(await _userService.GetUsersAsync(pagedRequest));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserModel>> GetUserById(int id)
    {
        var userId = _currentUserContext.GetCurrentUserId();

        if (userId is null)
            return Unauthorized();

        return Ok(await _userService.GetCurrentUserAsync(userId.Value));
    }

    [HttpGet("{id}/setlists")]
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

        if (user is null)
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

        var result = await _userService.TryAddUserTokenAsync(user.Id, tokenModel);
        
        if(!result)
            return BadRequest("Could not add token to user, provider not found");

        return Redirect(_appOptions.Value.UserPortalUrl);
    }

    [HttpPost("{id}/friendships")]
    public async Task<ActionResult> InitiateFriendship(int id, [FromBody] FriendshipRequestModel requestModel)
    {
        await _userService.HandleFriendshipRequestAsync(id, requestModel);
        
        return NoContent();
    }

    [HttpDelete("{id}/friendships/{friendshipId}")]
    public async Task<ActionResult> RemoveFriendship(int id, int friendshipId)
    {
        await _userService.RemoveFriendshipAsync(id, friendshipId);
        
        return NoContent();
    }

    [HttpGet("{id}/friendships")]
    public async Task<ActionResult<PagedResponse<FriendModel>>> GetUserFriends(int id, PagedRequest pagedRequest)
    {
        var currentUserId = _currentUserContext.GetCurrentUserId();
        
        if (currentUserId is null)
            return Unauthorized();
        
        var result = await _userService.GetUserFriendsAsync(id, pagedRequest);
                
        return Ok(result);
    }

    [HttpPut("{id}/friendships/{friendshipId}")]
    public async Task<ActionResult> AcceptFriendship(int id, int friendshipId)
    {        
        await _userService.AcceptFriendshipAsync(id, friendshipId);
        
        return NoContent();
    }
}