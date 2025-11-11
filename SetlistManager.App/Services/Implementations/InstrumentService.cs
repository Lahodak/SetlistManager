using Microsoft.Extensions.Options;
using SetlistManager.App.Options;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Services.Implementations;

public class InstrumentService : IInstrumentService
{
    private readonly IOptions<SetlistManagerApiOptions> _apiOptions;
    private readonly IApiService _apiService;

    public InstrumentService(IApiService apiService, IOptions<SetlistManagerApiOptions> apiOptions)
    {
        _apiOptions = apiOptions;
        _apiService = apiService;
    }

    public async Task<List<InstrumentModel>?> GetAvailableInstrumentsAsync()
        => await _apiService.GetAsync<List<InstrumentModel>>(_apiOptions.Value.InstrumentsEndpoint);
}