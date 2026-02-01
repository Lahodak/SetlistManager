using Microsoft.Extensions.Options;
using SetlistManager.App.Options;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Services.Implementations;

public class LanguageService : ILanguageService
{
    private readonly IApiService _apiService;
    private readonly SetlistManagerApiOptions _apiOptions;
    public LanguageService(IApiService apiService, IOptions<SetlistManagerApiOptions> apiOptions)
    {
        _apiOptions = apiOptions.Value;
        _apiService = apiService;
    }

    public async Task<List<LanguageModel>?> GetAvailableLanguagesAsync() 
        => await _apiService.GetAsync<List<LanguageModel>>(_apiOptions.LanguagesEndpoint);
}