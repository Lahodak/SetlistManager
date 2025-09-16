using SetlistManager.Common.Models;
using SetlistManager.Data.Entities;

namespace SetlistManger.Business.Services;

public interface ILanguageService
{
    Task<List<LanguageModel>> GetAvailableLanguagesAsync();
    Task<Language> GetLanguageByNameAsync(string name);
    Task<Language> GetLanguageByIdAsync(int id);
}