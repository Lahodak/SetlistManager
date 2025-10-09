using SetlistManager.Common.Models;

namespace SetlistManager.App.Services;

public class InstrumentService
{
    private readonly ApiService _apiService;

    private readonly string _instrumentsEndpointPath;

    public InstrumentService(ApiService apiService, IConfiguration configuration)
    {
        _instrumentsEndpointPath = configuration["SetlistManager.Api:InstrumentsEndpoint"]!;
        _apiService = apiService;
    }

    public async Task<List<InstrumentModel>> GetAvailableInstrumentsAsync()
        => await _apiService.GetAsync<List<InstrumentModel>>(_instrumentsEndpointPath);
}