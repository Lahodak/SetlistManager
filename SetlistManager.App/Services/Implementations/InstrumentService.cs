using Microsoft.Extensions.Options;
using SetlistManager.App.Options;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Services.Implementations;

public class InstrumentService : IInstrumentService
{
    private readonly IApiService _apiService;
    private readonly string _apiPath;
    public InstrumentService(IApiService apiService, IOptions<SetlistManagerApiOptions> apiOptions)
    {
        _apiService = apiService;
        _apiPath = apiOptions.Value.BaseUrl + apiOptions.Value.InstrumentsEndpoint;
    }

    public async Task<List<InstrumentModel>?> GetAvailableInstrumentsAsync()
        => await _apiService.GetAsync<List<InstrumentModel>>(_apiPath);
}