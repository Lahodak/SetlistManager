using Microsoft.Extensions.Options;
using SetlistManager.App.Options;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Services.Implementations;

public class LanguageService : ILanguageService
{
    private readonly IApiService _apiService;
    private readonly string _apiPath;
    public LanguageService(IApiService apiService, IOptions<SetlistManagerApiOptions> apiOptions)
    {
        _apiService = apiService;
        _apiPath = apiOptions.Value.BaseUrl + apiOptions.Value.LanguagesEndpoint;
    }

    public async Task<List<LanguageModel>?> GetAvailableLanguagesAsync() 
        => await _apiService.GetAsync<List<LanguageModel>>(_apiPath);
}