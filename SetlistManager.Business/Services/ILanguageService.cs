using SetlistManager.Common.Models;
using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Services;

public interface ILanguageService
{
    Task<List<LanguageModel>> GetAvailableLanguagesAsync();
}