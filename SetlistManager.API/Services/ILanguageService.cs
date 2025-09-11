using SetlistManager.Common.Models;
using SetlistManager.API.Data.Entities;

namespace SetlistManager.API.Services;

public interface ILanguageService
{
    Task<List<LanguageModel>> GetAvailableLanguagesAsync();
    Task<Data.Entities.Language> GetLanguageByNameAsync(string name);
    Task<Data.Entities.Language> GetLanguageByIdAsync(int id);
}