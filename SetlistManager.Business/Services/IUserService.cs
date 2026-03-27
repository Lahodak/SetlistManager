using SetlistManager.Common.Genius.Models;
using SetlistManager.Common.Models;
using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Services;

/// <summary>
/// Provides operations for managing users, tokens, and friendships.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Updates the specified user's profile.
    /// </summary>
    /// <param name="model">The user data to apply.</param>
    Task UpdateUserAsync(UserModel model);

    /// <summary>
    /// Gets the profile of the currently authenticated user.
    /// </summary>
    /// <returns>The current user's model, or <see langword="null"/> if not found.</returns>
    Task<UserModel?> GetCurrentUserAsync();

    /// <summary>
    /// Exchanges a Genius OAuth authorization code for an access token and stores it for the user.
    /// </summary>
    /// <param name="grantResultModel">The authorization code and state from the Genius OAuth callback.</param>
    Task AddGeniusTokenToUserAsync(GrantAccessTokenResultModel grantResultModel);

    /// <summary>
    /// Revokes an external provider token for the specified user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="tokenId">The token identifier to revoke.</param>
    Task RevokeTokenAsync(int userId, int tokenId);

    /// <summary>
    /// Retrieves a user entity by a temporary OAuth authentication secret.
    /// </summary>
    /// <param name="secret">The temporary secret used during the OAuth flow.</param>
    /// <returns>The matching user entity</returns>
    Task<User> GetUserByTempAuthSecret(string secret);

    /// <summary>
    /// Retrieves a user entity by its identifier.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>The user entity, or <see langword="null"/> if not found.</returns>
    Task<User?> GetUserEntityByIdAsync(int userId);

    /// <summary>
    /// Gets a paginated list of users.
    /// </summary>
    /// <param name="request">Pagination and search parameters.</param>
    Task<PagedResponse<UserViewModel>> GetUsersAsync(PagedRequest request);

    /// <summary>
    /// Sends or processes a friendship request on behalf of the initiator.
    /// </summary>
    /// <param name="initiatorId">The identifier of the user initiating the request.</param>
    /// <param name="friendshipRequest">The friendship request details.</param>
    Task HandleFriendshipRequestAsync(int initiatorId, FriendshipRequestModel friendshipRequest);

    /// <summary>
    /// Accepts a pending friendship request.
    /// </summary>
    /// <param name="id">The user identifier accepting the request.</param>
    /// <param name="friendshipId">The friendship record identifier.</param>
    Task AcceptFriendshipAsync(int id, int friendshipId);

    /// <summary>
    /// Removes an existing friendship or declines a pending request.
    /// </summary>
    /// <param name="id">The user identifier performing the removal.</param>
    /// <param name="friendshipId">The friendship record identifier.</param>
    Task RemoveFriendshipAsync(int id, int friendshipId);

    /// <summary>
    /// Gets a paginated list of friends for the specified user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="request">Pagination and search parameters.</param>
    Task<PagedResponse<FriendModel>> GetUserFriendsAsync(int userId, PagedRequest request);
}