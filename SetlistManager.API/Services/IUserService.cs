using SetlistManager.Common.Models;

namespace SetlistManager.API.Services;

public interface IUserService
{
    Task UpdateUserAsync(UserModel model);   
    Task GetCurrentUserAsync(int userId);
}