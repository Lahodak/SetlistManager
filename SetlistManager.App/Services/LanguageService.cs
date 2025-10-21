using SetlistManager.Common.Models;

namespace SetlistManager.App.Services;

public class LanguageService
{
    private readonly string _languagesEndpointPath;

    private readonly ApiService _apiService;
    public LanguageService(ApiService apiService, IConfiguration configuration)
    {
        _languagesEndpointPath = configuration["SetlistManager.Api:LanguagesEndpoint"]!;
        _apiService = apiService;
    }

    public async Task<List<LanguageModel>> GetAvailableLanguagesAsync() 
        => await _apiService.GetAsync<List<LanguageModel>>(_languagesEndpointPath);
}