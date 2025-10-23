using SetlistManager.Common.Models;
using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Services;

public interface IUserService
{
    Task UpdateUserAsync(UserModel model);   
    Task<UserModel> GetCurrentUserAsync(int userId);
    Task AddUserTokenAsync(int userId, AddTokenModel tokenModel);
    Task<User?> GetUserByTempSalt(string salt);
}