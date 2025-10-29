using SetlistManager.Common.Models;

namespace SetlistManager.App.Services.Implementations;

public class InstrumentService : IInstrumentService
{
    private readonly IApiService _apiService;

    private readonly string _instrumentsEndpointPath;

    public InstrumentService(IApiService apiService, IConfiguration configuration)
    {
        _instrumentsEndpointPath = configuration["SetlistManager.Api:InstrumentsEndpoint"]!;
        _apiService = apiService;
    }

    public async Task<List<InstrumentModel>?> GetAvailableInstrumentsAsync()
        => await _apiService.GetAsync<List<InstrumentModel>>(_instrumentsEndpointPath);
}