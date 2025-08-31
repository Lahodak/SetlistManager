using SetlistManager.API.Data.Entities;

namespace SetlistManager.API.Services;

public interface IJwtService
{
    Task<string> GenerateTokenAsync(User user);
}