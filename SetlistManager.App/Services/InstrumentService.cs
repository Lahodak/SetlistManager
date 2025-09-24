using SetlistManager.Common.Models;

namespace SetlistManager.App.Services;

public class InstrumentService
{
    private readonly ApiService _apiService;

    private const string _instrumentsEndpointPath = "https://localhost:7143/api/instruments";

    public InstrumentService(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<List<InstrumentModel>> GetAvailableInstrumentsAsync()
        => await _apiService.GetAsync<List<InstrumentModel>>(_instrumentsEndpointPath);
}