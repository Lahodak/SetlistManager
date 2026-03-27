using SetlistManager.Common.Genius.Models;
using SetlistManager.Common.Models;

namespace SetlistManager.Business.Services;

/// <summary>
/// Provides methods for handling Genius authentication, including exchanging authorization codes for access tokens and generating URIs for granting access permissions.
/// </summary>
public interface IGeniusAuthService
{
    /// <summary>
    /// Exchanges the provided authorization code for an access token from the Genius API.
    /// </summary>
    Task<CodeExchangeResponseModel?> ExchangeGeniusCode(string code);
    
    /// <summary>
    /// Generates a URI that can be used to request access permissions from the Genius API.
    /// </summary>
    Task<UrlResponseModel> GetGrantAccessTokenRequestUri();
}