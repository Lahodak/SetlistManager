using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SetlistManager.Api.Services;
using SetlistManager.Business.Options;
using SetlistManager.Business.Services;
using SetlistManager.Common.Genius.Models;
using SetlistManager.Common.Models;

namespace SetlistManager.Api.Controllers;

public class UsersController : BaseController
{
    private readonly IUserService _userService;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IGeniusAuthService _geniusAuthService;
    private readonly IOptions<AppOptions> _appOptions;

    public UsersController(IUserService userService, ICurrentUserContext currentUserContext, 
        IGeniusAuthService geniusAuthService, IOptions<AppOptions> appOptions)
    {
        _appOptions = appOptions;
        _userService = userService;
        _currentUserContext = currentUserContext;
        _geniusAuthService = geniusAuthService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<UserViewModel>>> GetUsers([FromQuery] PagedRequest pagedRequest)
    {
        return Ok(await _userService.GetUsersAsync(pagedRequest));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserModel>> GetUserById(int id)
    {
        if (!IsAuthorized(id))
            return Unauthorized();

        return Ok(await _userService.GetCurrentUserAsync(id));
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(UserModel model)
    {
        await _userService.UpdateUserAsync(model);
        return NoContent();
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
        if (!IsAuthorized(id))
            return Unauthorized();

        await _userService.HandleFriendshipRequestAsync(id, requestModel);
        
        return NoContent();
    }

    [HttpDelete("{id}/friendships/{friendshipId}")]
    public async Task<ActionResult> RemoveFriendship(int id, int friendshipId)
    {
        if (!IsAuthorized(id))
            return Unauthorized();

        await _userService.RemoveFriendshipAsync(id, friendshipId);
        
        return NoContent();
    }

    [HttpGet("{id}/friendships")]
    public async Task<ActionResult<PagedResponse<FriendModel>>> GetUserFriends(int id, [FromQuery] PagedRequest pagedRequest)
    {        
        if (!IsAuthorized(id))
            return Unauthorized();
        
        var result = await _userService.GetUserFriendsAsync(id, pagedRequest);
                
        return Ok(result);
    }

    [HttpPut("{id}/friendships/{friendshipId}")]
    public async Task<ActionResult> AcceptFriendship(int id, int friendshipId)
    {
        if (!IsAuthorized(id))
            return Unauthorized();

        await _userService.AcceptFriendshipAsync(id, friendshipId);
        
        return NoContent();
    }

    private bool IsAuthorized(int id)
    {
        var currentUserId = _currentUserContext.GetCurrentUserId();
        return currentUserId is not null && currentUserId.Value == id;
    }
}