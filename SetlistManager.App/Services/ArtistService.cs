using SetlistManager.Common.Models;

namespace SetlistManager.App.Services;

public class ArtistService
{
    private readonly string _artistsEndpointPath;
    private readonly ApiService _apiService;
    
    public ArtistService(ApiService apiService, IConfiguration configuration)
    {
        _artistsEndpointPath = configuration["SetlistManager.Api:ArtistsEndpoint"]!;
        _apiService = apiService;
    }

    public async Task<List<ArtistModel>>? GetAvailableArtistsAsync() 
        => await _apiService.GetAsync<List<ArtistModel>>(_artistsEndpointPath);

    public async Task<ArtistModel>? GetArtistByIdAsync(int id) 
        => await _apiService.GetAsync<ArtistModel>(_artistsEndpointPath + "/" + id.ToString());

    public async Task UploadArtistAsync(ArtistModel artist)
        => await _apiService.PostAsync(_artistsEndpointPath, artist);
}