using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Services;

/// <summary>
/// Defines methods for generating JSON Web Tokens (JWT) for user authentication and authorization purposes.
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Generates a JSON Web Token (JWT) for the specified user, containing claims and information necessary for authentication and authorization.
    /// </summary>
    string GenerateToken(User user);
}