using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using SetlistManager.App.Options;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Services.Implementations;

public class SongService : ISongService
{
    private readonly IApiService _apiService;
    private readonly IOptions<SetlistManagerApiOptions> _apiOptions;

    public SongService(IApiService apiService, IOptions<SetlistManagerApiOptions> apiOptions)
    {
        _apiOptions = apiOptions;
        _apiService = apiService;
    }

    public async Task<PagedResponse<SongModel>?> GetAllSongsAsync(PagedRequest request)
    {
        UriBuilder uri = new(_apiOptions.Value.SongsEndpoint)
        {
            Query = new QueryBuilder
            {
                { "PageSize", request.PageSize.ToString() },
                { "PageIndex", request.PageIndex.ToString() },
                { "Query", request.Query ?? string.Empty },
                { "ContentType", request.ContentType.ToString() }
            }.ToString()
        };

        return await _apiService.GetAsync<PagedResponse<SongModel>>(uri.ToString());
    }

    public async Task<SongModel?> GetSongByIdAsync(int id)
        => await _apiService.GetAsync<SongModel>($"{_apiOptions.Value.SongsEndpoint}/{id}");

    public async Task UploadSongAsync(SongCreateModel songCreateModel) 
        => await _apiService.PostAsync(_apiOptions.Value.SongsEndpoint, songCreateModel);

    public async Task<bool> TryUpdateSongAsync(int id, SongUpdateModel songModel)
        => await _apiService.TryPutAsync($"{_apiOptions.Value.SongsEndpoint}/{id}", songModel);
    public async Task<bool> TryDeleteSongAsync(int id)
        => await _apiService.TryDeleteAsync($"{_apiOptions.Value.SongsEndpoint}/{id}");

    public async Task<bool> TryMakeSongPublicAsync(int id)
        => await _apiService.PostAsync($"{_apiOptions.Value.SongsEndpoint}/{id}/public", true);

    public async Task<bool> TryGiveAccessToUserAsync(int songId, int targetId)
        => await _apiService.PostAsync($"{_apiOptions.Value.SongsEndpoint}/{songId}/songsusers/{targetId}", true);

    public async Task RemoveAccessFromUserAsync(int songId, int targetId)
        => await _apiService.TryDeleteAsync($"{_apiOptions.Value.SongsEndpoint}/{songId}/songsusers/{targetId}");
}