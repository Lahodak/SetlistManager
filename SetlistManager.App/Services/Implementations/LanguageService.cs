using SetlistManager.Common.Models;

namespace SetlistManager.App.Services.Implementations;

public class LanguageService : ILanguageService
{
    private readonly string _languagesEndpointPath;

    private readonly IApiService _apiService;
    public LanguageService(IApiService apiService, IConfiguration configuration)
    {
        _languagesEndpointPath = configuration["SetlistManager.Api:LanguagesEndpoint"]!;
        _apiService = apiService;
    }

    public async Task<List<LanguageModel>?> GetAvailableLanguagesAsync() 
        => await _apiService.GetAsync<List<LanguageModel>>(_languagesEndpointPath);
}