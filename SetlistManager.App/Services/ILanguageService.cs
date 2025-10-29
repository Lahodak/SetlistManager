using SetlistManager.Common.Models;

namespace SetlistManager.App.Services;

public interface ILanguageService
{
    Task<List<LanguageModel>?> GetAvailableLanguagesAsync();
}