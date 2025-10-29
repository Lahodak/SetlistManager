using Microsoft.Extensions.Options;
using SetlistManager.App.Options;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Services.Implementations;

public class LanguageService : ILanguageService
{
    private readonly IOptions<SetlistManagerApiOptions> _apiOptions;
    private readonly IApiService _apiService;
    public LanguageService(IApiService apiService, IOptions<SetlistManagerApiOptions> apiOptions)
    {
        _apiOptions = apiOptions;
        _apiService = apiService;
    }

    public async Task<List<LanguageModel>?> GetAvailableLanguagesAsync() 
        => await _apiService.GetAsync<List<LanguageModel>>(_apiOptions.Value.LanguagesEndpoint);
}