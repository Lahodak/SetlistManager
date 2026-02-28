using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Services;

public interface IJwtService
{
    string GenerateToken(User user);
}