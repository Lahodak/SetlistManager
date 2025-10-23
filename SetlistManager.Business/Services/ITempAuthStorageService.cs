using SetlistManager.Common.Genius.Models;

namespace SetlistManager.Business.Services;

public interface ITempAuthStorageService
{
    Task<CodeExchangeResponseModel?> ExchangeGeniusCode(string code);
    Task<string> GetGrantAccessTokenRequestUri(int userId);
}