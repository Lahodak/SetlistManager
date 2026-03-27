using Microsoft.Extensions.Options;
using SetlistManager.App.Extensions;
using SetlistManager.App.Options;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Services.Implementations;

public class SongService : ISongService
{
    private readonly IApiService _apiService;
    private readonly SetlistManagerApiOptions _apiOptions;
    private readonly string _apiPath;

    public SongService(IApiService apiService, IOptions<SetlistManagerApiOptions> apiOptions)
    {
        _apiService = apiService;
        _apiOptions = apiOptions.Value;
        _apiPath = _apiOptions.BaseUrl + _apiOptions.SongsEndpoint;
    }

    public async Task<PagedResponse<SongModel>> GetSongsAsync(ContentPagedRequest request)
    {
        var uri = request.ToContentPagedRequestUri(_apiPath);
        return await _apiService.GetAsync<PagedResponse<SongModel>>(uri);
    }

    public async Task<SongModel?> GetSongByIdAsync(int id)
        => await _apiService.GetAsync<SongModel>($"{_apiPath}/{id}");

    public async Task<bool> TryCreateSongAsync(SongCreateModel songCreateModel)
        => await _apiService.TryPostAsync(_apiPath, songCreateModel);

    public async Task<bool> TryUpdateSongAsync(int id, SongUpdateModel songModel)
        => await _apiService.TryPutAsync($"{_apiPath}/{id}", songModel);

    public async Task<bool> TryDeleteSongAsync(int id)
        => await _apiService.TryDeleteAsync($"{_apiPath}/{id}");

    public async Task<bool> TryMakeSongPublicAsync(int id)
        => await _apiService.TryPostAsync($"{_apiPath}/{id}/public", true);

    public async Task<bool> TryGiveAccessToUserAsync(int songId, int targetId)
        => await _apiService.TryPostAsync($"{_apiPath}/{songId}/users/{targetId}", true);

    public async Task<bool> TryRemoveAccessFromUserAsync(int songId, int targetId)
        => await _apiService.TryDeleteAsync($"{_apiPath}/{songId}/users/{targetId}");

    public async Task<List<SongUsageStatModel>> GetStatisticsAsync(StatsRequest request)
    {
        var uri = request.ToUri(_apiOptions.BaseUrl + _apiOptions.StatisticsEndpoint);
        return await _apiService.GetAsync<List<SongUsageStatModel>>(uri);
    }
}