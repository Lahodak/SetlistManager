using SetlistManager.App.Models;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Services;

/// <summary>
/// Client-side service for user account management, authentication state, and preferences.
/// </summary>
public interface IUserService
{
    /// <summary>Gets the current user's profile from the API.</summary>
    Task<UserModel?> GetUserAsync();

    /// <summary>Registers a new user account.</summary>
    Task RegisterAsync(RegisterRequestModel model);

    /// <summary>Logs out the current user and clears stored tokens.</summary>
    Task LogOutAsync();

    /// <summary>Retrieves the stored JWT token for the current user.</summary>
    Task<string?> GetUserTokenAsync();

    /// <summary>Verifies whether the stored JWT token is still valid.</summary>
    Task<bool> VerifyStoredToken();

    /// <summary>Updates the current user's profile. Returns <see langword="true"/> on success.</summary>
    Task<bool> TryUpdateUser(UserModel user);

    /// <summary>Authenticates with credentials and stores the JWT token. Returns <see langword="true"/> on success.</summary>
    Task<bool> LogInAsync(LoginRequestModel model);

    /// <summary>Verifies a user's email address.</summary>
    Task<bool> VerifyEmailAsync(string token, string email);

    /// <summary>Requests a password-reset email for the specified address.</summary>
    Task<bool> RequestPasswordResetAsync(string email);

    /// <summary>Resets the user's password using a reset token.</summary>
    Task<bool> ResetPasswordAsync(string email, string newPassword, string token);

    /// <summary>Gets a paginated list of the current user's friendships.</summary>
    Task<PagedResponse<FriendModel>?> GetUserFriendshipsAsync(PagedRequest request);

    /// <summary>Sends a friendship request to another user.</summary>
    Task HandleFriendshipRequestAsync(FriendshipRequestModel friendshipRequest);

    /// <summary>Removes a friendship. Returns <see langword="true"/> on success.</summary>
    Task<bool> TryRemoveFriendshipAsync(int friendshipId);

    /// <summary>Accepts a pending friendship request. Returns <see langword="true"/> on success.</summary>
    Task<bool> TryAcceptFriendshipAsync(int friendshipId);

    /// <summary>Revokes an external provider token. Returns <see langword="true"/> on success.</summary>
    Task<bool> TryRevokeTokenAsync(int tokenId);

    /// <summary>Gets a paginated list of all users.</summary>
    Task<PagedResponse<UserViewModel>?> GetPagedUsersAsync(PagedRequest request);

    /// <summary>Gets the current user's identifier from local storage.</summary>
    Task<int?> GetCurrentUserIdAsync();

    /// <summary>Gets the user's dark mode preference from local storage.</summary>
    Task<bool> GetUserDarkModeSettings();

    /// <summary>Persists the user's dark mode preference to local storage.</summary>
    Task UpdateUserDarkModeSettingsAsync(bool newValue);

    /// <summary>Gets the user's room panel layout configuration from local storage.</summary>
    Task<List<PanelType>?> GetPanelConfigAsync();

    /// <summary>Saves the user's room panel layout configuration to local storage.</summary>
    Task SavePanelConfigAsync(List<PanelType> panels);
}