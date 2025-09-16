using SetlistManager.Data.Entities;

namespace SetlistManager. Api.Services;

public interface IJwtService
{
    Task<string> GenerateTokenAsync(User user);
}