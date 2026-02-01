using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SetlistManager.Business.Options;
using SetlistManager.Business.Services;
using SetlistManager.Common.Genius.Models;
using SetlistManager.Common.Models;

namespace SetlistManager.Api.Controllers;

public class UsersController : BaseController
{
    private readonly IUserService _userService;
    private readonly AppOptions _appOptions;

    public UsersController(IUserService userService, IOptions<AppOptions> appOptions)
    {
        _appOptions = appOptions.Value;
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<UserViewModel>>> GetUsers([FromQuery] PagedRequest pagedRequest)
    {
        return Ok(await _userService.GetUsersAsync(pagedRequest));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserModel>> GetUserById(int id)
    {
        return Ok(await _userService.GetCurrentUserAsync(id));
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(UserModel model)
    {
        await _userService.UpdateUserAsync(model);

        return NoContent();
    }

    [AllowAnonymous]
    [HttpGet("genius-tokens-callback")]
    public async Task<ActionResult> AddGeniusTokenToUser([FromQuery] GrantAccessTokenResultModel grantResultModel)
    {
        await _userService.TryAddGeniusTokenToUserAsync(grantResultModel);       

        return Redirect(_appOptions.UserPortalUrl);
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
    public async Task<ActionResult<PagedResponse<FriendModel>>> GetUserFriends(int id, [FromQuery] PagedRequest pagedRequest)
    {
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