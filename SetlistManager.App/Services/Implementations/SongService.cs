using Microsoft.Extensions.Options;
using SetlistManager.App.Options;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Services.Implementations;

public class SongService : ISongService
{
    private const string _getSongbyIdSuffix = "/";

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

    public async Task UploadSongAsync(SongCreateModel songCreateModel) 
        => await _apiService.PostAsync(_apiOptions.Value.SongsEndpoint, songCreateModel);

    public async Task<bool> TryUpdateSongAsync(int id, SongUpdateModel songModel)
        => await _apiService.TryPutAsync(_apiOptions.Value.SongsEndpoint + _getSongbyIdSuffix + id.ToString(), songModel);
    public async Task<bool> TryDeleteSongAsync(int id)
        => await _apiService.TryDeleteAsync(_apiOptions.Value.SongsEndpoint + _getSongbyIdSuffix + id.ToString());
}