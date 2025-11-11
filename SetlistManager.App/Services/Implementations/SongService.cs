using Microsoft.Extensions.Options;
using SetlistManager.App.Options;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Services.Implementations;

public class SongService : ISongService
{
    private const string _getSongbyIdSuffix = "/";
    private const string _getSongbyNameSuffix = "?name=";
    private const string _uploadSongCollectionSuffix = "/bulk";    

    private readonly IApiService _apiService;
    private readonly IOptions<SetlistManagerApiOptions> _apiOptions;

    public SongService(IApiService apiService, IOptions<SetlistManagerApiOptions> apiOptions)
    {
        _apiOptions = apiOptions;
        _apiService = apiService;
    }

    public async Task<List<SongModel>?> GetAllSongsAsync() 
        => await _apiService.GetAsync<List<SongModel>>(_apiOptions.Value.SongsEndpoint);

    public async Task<SongModel?> GetSongByIdAsync(int id) 
        => await _apiService.GetAsync<SongModel>(_apiOptions.Value.SongsEndpoint + _getSongbyIdSuffix + id.ToString());

    public async Task<SongModel?> GetSongByNameAsync(string name) 
        => await _apiService.GetAsync<SongModel>(_apiOptions.Value.SongsEndpoint + _getSongbyNameSuffix + name);

    public async Task UploadSongsAsync(List<SongModel> songsToUpload) 
        => await _apiService.PostAsync(_apiOptions.Value.SongsEndpoint + _uploadSongCollectionSuffix, songsToUpload);

    public async Task UploadSongAsync(SongModel songToUpload) 
        => await _apiService.PostAsync(_apiOptions.Value.SongsEndpoint, songToUpload);
}