using Microsoft.AspNetCore.Mvc;
using SetlistManager.Business.Services;
using SetlistManager.Common.Models;

namespace SetlistManager.Api.Controllers;

/// <summary>
/// Manages user profiles, tokens, and friendship operations.
/// </summary>
public class UsersController : BaseController
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>Gets a paginated list of users.</summary>
    /// <param name="pagedRequest">Pagination and search parameters.</param>
    [HttpGet]
    public async Task<ActionResult<PagedResponse<UserViewModel>>> GetUsers([FromQuery] PagedRequest pagedRequest)
    {
        return Ok(await _userService.GetUsersAsync(pagedRequest));
    }

    /// <summary>Gets the profile of the currently authenticated user.</summary>
    [HttpGet("me")]
    public async Task<ActionResult<UserModel>> GetCurrentUser()
    {
        return Ok(await _userService.GetCurrentUserAsync());
    }

    /// <summary>Updates a user's profile.</summary>
    /// <param name="id">The user identifier.</param>
    /// <param name="model">The updated user data.</param>
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateUser(int id, [FromBody] UserModel model)
    {
        await _userService.UpdateUserAsync(model);

        return NoContent();
    }

    /// <summary>Revokes an external provider token for a user.</summary>
    /// <param name="id">The user identifier.</param>
    /// <param name="tokenId">The token identifier to revoke.</param>
    [HttpDelete("{id}/tokens/{tokenId}")]
    public async Task<ActionResult> RevokeUserToken(int id, int tokenId)
    {
        await _userService.RevokeTokenAsync(id, tokenId);
        
        return NoContent();
    }

    /// <summary>Sends a friendship request on behalf of the specified user.</summary>
    /// <param name="id">The initiator's user identifier.</param>
    /// <param name="requestModel">The friendship request details.</param>
    [HttpPost("{id}/friendships")]
    public async Task<ActionResult> InitiateFriendship(int id, [FromBody] FriendshipRequestModel requestModel)
    {
        await _userService.HandleFriendshipRequestAsync(id, requestModel);
        
        return NoContent();
    }

    /// <summary>Removes an existing friendship or declines a pending request.</summary>
    /// <param name="id">The user identifier.</param>
    /// <param name="friendshipId">The friendship record identifier.</param>
    [HttpDelete("{id}/friendships/{friendshipId}")]
    public async Task<ActionResult> RemoveFriendship(int id, int friendshipId)
    {
        await _userService.RemoveFriendshipAsync(id, friendshipId);
        
        return NoContent();
    }

    /// <summary>Gets a paginated list of friends for the specified user.</summary>
    /// <param name="id">The user identifier.</param>
    /// <param name="pagedRequest">Pagination and search parameters.</param>
    [HttpGet("{id}/friendships")]
    public async Task<ActionResult<PagedResponse<FriendModel>>> GetUserFriends(int id, [FromQuery] PagedRequest pagedRequest)
    {
        return Ok(await _userService.GetUserFriendsAsync(id, pagedRequest));
    }

    /// <summary>Accepts a pending friendship request.</summary>
    /// <param name="id">The user identifier accepting the request.</param>
    /// <param name="friendshipId">The friendship record identifier.</param>
    [HttpPut("{id}/friendships/{friendshipId}")]
    public async Task<ActionResult> AcceptFriendship(int id, int friendshipId)
    {
        await _userService.AcceptFriendshipAsync(id, friendshipId);
        
        return NoContent();
    }
}