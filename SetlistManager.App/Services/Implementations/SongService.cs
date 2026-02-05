using Microsoft.Extensions.Options;
using SetlistManager.App.Extensions;
using SetlistManager.App.Options;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Services.Implementations;

public class SongService : ISongService
{
    private readonly IApiService _apiService;
    private readonly SetlistManagerApiOptions _apiOptions;

    public SongService(IApiService apiService, IOptions<SetlistManagerApiOptions> apiOptions)
    {
        _apiOptions = apiOptions.Value;
        _apiService = apiService;
    }

    public async Task<PagedResponse<SongModel>> GetSongsAsync(PagedRequest request)
    {
        var uri = request.ToUri(_apiOptions.SongsEndpoint);
        return await _apiService.GetAsync<PagedResponse<SongModel>>(uri);
    }

    public async Task<SongModel?> GetSongByIdAsync(int id)
        => await _apiService.GetAsync<SongModel>($"{_apiOptions.SongsEndpoint}/{id}");

    public async Task<bool> TryCreateSongAsync(SongCreateModel songCreateModel)
        => await _apiService.TryPostAsync(_apiOptions.SongsEndpoint, songCreateModel);

    public async Task<bool> TryUpdateSongAsync(int id, SongUpdateModel songModel)
        => await _apiService.TryPutAsync($"{_apiOptions.SongsEndpoint}/{id}", songModel);

    public async Task<bool> TryDeleteSongAsync(int id)
        => await _apiService.TryDeleteAsync($"{_apiOptions.SongsEndpoint}/{id}");

    public async Task<bool> TryMakeSongPublicAsync(int id)
        => await _apiService.TryPostAsync($"{_apiOptions.SongsEndpoint}/{id}/public", true);

    public async Task<bool> TryGiveAccessToUserAsync(int songId, int targetId)
        => await _apiService.TryPostAsync($"{_apiOptions.SongsEndpoint}/{songId}/users/{targetId}", true);

    public async Task<bool> TryRemoveAccessFromUserAsync(int songId, int targetId)
        => await _apiService.TryDeleteAsync($"{_apiOptions.SongsEndpoint}/{songId}/users/{targetId}");

    public async Task<List<SongUsageStatModel>> GetStatisticsAsync(StatsRequest request)
    {
        var uri = request.ToUri(_apiOptions.StatisticsEndpoint);
        return await _apiService.GetAsync<List<SongUsageStatModel>>(uri);
    }
}