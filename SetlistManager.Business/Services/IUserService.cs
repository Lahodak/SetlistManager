using SetlistManager.Common.Models;
using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Services;

public interface IUserService
{
    Task UpdateUserAsync(UserModel model);   
    Task<UserModel?> GetCurrentUserAsync(int userId);
    Task<bool> TryAddUserTokenAsync(int userId, TokenCreateModel tokenModel);
    Task<User?> GetUserByTempSalt(string salt);
    Task<User?> GetUserEntityByIdAsync(int userId);
}