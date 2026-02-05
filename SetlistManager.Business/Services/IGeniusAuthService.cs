using SetlistManager.Common.Genius.Models;
using SetlistManager.Common.Models;

namespace SetlistManager.Business.Services;

public interface IGeniusAuthService
{
    Task<CodeExchangeResponseModel?> ExchangeGeniusCode(string code);
    Task<UrlResponseModel> GetGrantAccessTokenRequestUri();
}