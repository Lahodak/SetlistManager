using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Services;

public interface IJwtService
{
    Task<string> GenerateTokenAsync(User user);
}