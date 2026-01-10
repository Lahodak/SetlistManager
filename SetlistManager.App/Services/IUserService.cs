using SetlistManager.Common.Models;

namespace SetlistManager.App.Services;

public interface IUserService
{
    Task AddNewProviderToken(TokenCreateModel tokenModel);
    Task<UserModel?> GetUserAsync();
    Task<List<SetlistModel>?> GetAllUserSetlists();
    Task RegisterAsync(RegisterRequestModel model);
    Task LogOutAsync();
    Task<string?> GetUserToken();
    Task<bool> IsUserLoggedInAsync();
    Task<bool> TryUpdateUser(UserModel user);
    Task LogInAsync(LoginRequestModel model);
    Task<bool> VerifyEmailAsync(string token, string email);
    Task<bool> RequestPasswordResetAsync(string email);
    Task<bool> ResetPasswordAsync(string email, string newPassword, string token);
    Task<PagedResponse<FriendModel>?> GetUserFriendshipsAsync(PagedRequest request);
    Task HandleFriendshipRequestAsync(FriendshipRequestModel friendshipRequest);
    Task<bool> TryRemoveFriendshipAsync(int friendshipId);
    Task<bool> TryAcceptFriendshipAsync(int friendshipId);
    Task<PagedResponse<UserViewModel>?> GetPagedUsersAsync(PagedRequest request);
    Task<int?> GetCurrentUserIdAsync();
}