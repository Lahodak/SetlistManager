using SetlistManager.Common.Models;

namespace SetlistManager.Business.Services;

/// <summary>
/// Provides methods for managing languages, including retrieving available languages for song.
/// </summary>
public interface ILanguageService
{
    /// <summary>
    /// Retrieves a list of programming languages supported by the system.
    /// </summary>
    /// <returns>The task result contains a list of LanguageModel objects. </returns>
    Task<List<LanguageModel>> GetAvailableLanguagesAsync();
}