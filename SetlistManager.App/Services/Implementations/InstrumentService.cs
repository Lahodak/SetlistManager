using Microsoft.Extensions.Options;
using SetlistManager.App.Options;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Services.Implementations;

public class InstrumentService : IInstrumentService
{
    private readonly IApiService _apiService;
    private readonly SetlistManagerApiOptions _apiOptions;

    public InstrumentService(IApiService apiService, IOptions<SetlistManagerApiOptions> apiOptions)
    {
        _apiOptions = apiOptions.Value;
        _apiService = apiService;
    }

    public async Task<List<InstrumentModel>?> GetAvailableInstrumentsAsync()
        => await _apiService.GetAsync<List<InstrumentModel>>(_apiOptions.InstrumentsEndpoint);
}