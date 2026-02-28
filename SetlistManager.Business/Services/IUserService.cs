using SetlistManager.Common.Genius.Models;
using SetlistManager.Common.Models;
using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Services;

public interface IUserService
{
    Task UpdateUserAsync(UserModel model);
    Task<UserModel?> GetCurrentUserAsync();
    Task AddGeniusTokenToUserAsync(GrantAccessTokenResultModel grantResultModel);
    Task RevokeTokenAsync(int userId, int tokenId);
    Task<User?> GetUserByTempAuthSecret(string salt);
    Task<User?> GetUserEntityByIdAsync(int userId);
    Task<PagedResponse<UserViewModel>> GetUsersAsync(PagedRequest request);
    Task HandleFriendshipRequestAsync(int initiatorId, FriendshipRequestModel friendshipRequest);
    Task AcceptFriendshipAsync(int id, int friendshipId);
    Task RemoveFriendshipAsync(int id, int friendshipId);
    Task<PagedResponse<FriendModel>> GetUserFriendsAsync(int userId, PagedRequest request);
}