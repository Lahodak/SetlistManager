using SetlistManager.Common.Genius.Models;

namespace SetlistManager.Business.Services;

public interface IGeniusAuthService
{
    Task<CodeExchangeResponseModel?> ExchangeGeniusCode(string code);
    Task<string> GetGrantAccessTokenRequestUri(int userId);
}