using SetlistManager.Common.Models;

namespace SetlistManager.App.Services;

public class ArtistService
{
    private const string _artistsEndpointPath = "https://localhost:7143/api/artists";
    private readonly ApiService _apiService;
    
    public ArtistService(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<List<ArtistModel>>? GetAvailableArtistsAsync() 
        => await _apiService.GetAsync<List<ArtistModel>>(_artistsEndpointPath);

    public async Task<ArtistModel>? GetArtistByIdAsync(int id) 
        => await _apiService.GetAsync<ArtistModel>(_artistsEndpointPath + "/" + id.ToString());

    public async Task UploadArtistAsync(ArtistModel artist)
        => await _apiService.PostAsync(_artistsEndpointPath, artist);
}