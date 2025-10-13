using SetlistManager.Common.Models;

namespace SetlistManager.App.Services;

public class SongService
{
    private readonly string _songsEndpointPath;    
    private const string _getSongbyIdSuffix = "/";
    private const string _getSongbyNameSuffix = "?name=";
    private const string _uploadSongCollectionSuffix = "/bulk";    

    private readonly ApiService _apiService;

    public SongService(ApiService apiService, IConfiguration configuration)
    {
        _songsEndpointPath = configuration["SetlistManager.Api:SongsEndpoint"]!;
        _apiService = apiService;
    }

    public async Task<List<SongModel>?> GetAllSongsAsync() 
        => await _apiService.GetAsync<List<SongModel>>(_songsEndpointPath);

    public async Task<SongModel?> GetSongByIdAsync(int id) 
        => await _apiService.GetAsync<SongModel>(_songsEndpointPath + _getSongbyIdSuffix + id.ToString());

    public async Task<SongModel?> GetSongByNameAsync(string name) 
        => await _apiService.GetAsync<SongModel>(_songsEndpointPath + _getSongbyNameSuffix + name);

    public async Task UploadSongsAsync(List<SongModel> songsToUpload) 
        => await _apiService.PostAsync(_songsEndpointPath + _uploadSongCollectionSuffix, songsToUpload);

    public async Task UploadSongAsync(SongModel songToUpload) 
        => await _apiService.PostAsync(_songsEndpointPath, songToUpload);
}