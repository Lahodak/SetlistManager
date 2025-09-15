using SetlistManager.Common.Models;

namespace SetlistManager.API.Services;

public interface IUserService
{
    Task UpdateUserAsync(UserModel model);   
    Task<UserModel> GetCurrentUserAsync(int userId);
}