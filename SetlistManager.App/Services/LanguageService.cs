using SetlistManager.Common.Models;

namespace SetlistManager.App.Services;

public class LanguageService
{
    private const string _languagesEndpointPath = "https://localhost:7143/api/languages";

    private readonly ApiService _apiService;
    public LanguageService(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<List<LanguageModel>> GetAvailableSetlistAsync()
    {
        return await _apiService.GetAsync<List<LanguageModel>>(_languagesEndpointPath);
    }
}
