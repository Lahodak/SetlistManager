using SetlistManager.Common.Models;

namespace SetlistManager.Business.Services;

public interface ILanguageService
{
    Task<List<LanguageModel>> GetAvailableLanguagesAsync();
}