using SetlistManager.Common.Models;

namespace SetlistManager.App.Services;

public interface IUserService
{
    Task AddNewProviderToken(AddTokenModel tokenModel);
    Task<UserModel?> GetUserAsync();
    Task<List<SetlistModel>?> GetAllUserSetlists();
    Task RegisterAsync(RegisterRequestModel model);
    Task LogOutAsync();
    Task<string?> GetUserToken();
    Task<bool> IsUserLoggedInAsync();
    Task UpdateUser(UserModel user);
    Task LogInAsync(LoginRequestModel model);
    Task<bool> VerifyEmailAsync(string token, string email);
    Task<bool> RequestPasswordResetAsync(string email);
    Task<bool> ResetPasswordAsync(string email, string newPassword, string token);
}