using SetlistManager.Common.Models;

namespace SetlistManager.App.Services.Implementations;

public class ArtistService : IArtistService
{
    private readonly string _artistsEndpointPath;
    private readonly IApiService _apiService;
    
    public ArtistService(IApiService apiService, IConfiguration configuration)
    {
        _artistsEndpointPath = configuration["SetlistManager.Api:ArtistsEndpoint"]!;
        _apiService = apiService;
    }

    public async Task<List<ArtistModel>?> GetAvailableArtistsAsync() 
        => await _apiService.GetAsync<List<ArtistModel>>(_artistsEndpointPath);

    public async Task<ArtistModel?> GetArtistByIdAsync(int id) 
        => await _apiService.GetAsync<ArtistModel>(_artistsEndpointPath + "/" + id.ToString());

    public async Task UploadArtistAsync(ArtistModel artist)
        => await _apiService.PostAsync(_artistsEndpointPath, artist);
}