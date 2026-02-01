using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
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

    public async Task<PagedResponse<SongModel>> GetAllSongsAsync(PagedRequest request)
    {
        UriBuilder uri = new(_apiOptions.SongsEndpoint)
        {
            Query = new QueryBuilder
            {
                { nameof(request.PageSize), request.PageSize.ToString() },
                { nameof(request.PageIndex), request.PageIndex.ToString() },
                { nameof(request.Query), request.Query ?? string.Empty },
                { nameof(request.ContentType), request.ContentType.ToString() }
            }.ToString()
        };

        return await _apiService.GetAsync<PagedResponse<SongModel>>(uri.ToString());
    }

    public async Task<SongModel?> GetSongByIdAsync(int id)
        => await _apiService.GetAsync<SongModel>($"{_apiOptions.SongsEndpoint}/{id}");

    public async Task UploadSongAsync(SongCreateModel songCreateModel) 
        => await _apiService.PostAsync(_apiOptions.SongsEndpoint, songCreateModel);

    public async Task<bool> TryUpdateSongAsync(int id, SongUpdateModel songModel)
        => await _apiService.TryPutAsync($"{_apiOptions.SongsEndpoint}/{id}", songModel);
    public async Task<bool> TryDeleteSongAsync(int id)
        => await _apiService.TryDeleteAsync($"{_apiOptions.SongsEndpoint}/{id}");

    public async Task<bool> TryMakeSongPublicAsync(int id)
        => await _apiService.PostAsync($"{_apiOptions.SongsEndpoint}/{id}/public", true);

    public async Task<bool> TryGiveAccessToUserAsync(int songId, int targetId)
        => await _apiService.PostAsync($"{_apiOptions.SongsEndpoint}/{songId}/songsusers/{targetId}", true);

    public async Task RemoveAccessFromUserAsync(int songId, int targetId)
        => await _apiService.TryDeleteAsync($"{_apiOptions.SongsEndpoint}/{songId}/songsusers/{targetId}");

    public async Task<PagedResponse<SongUsageStatModel>> GetMostUsedSongsAsync(StatsPagedRequest request)
    {
        UriBuilder uri = new($"{_apiOptions.SongsEndpoint}/most-used")
        {
            Query = new QueryBuilder
            {
                { nameof(request.PageSize), request.PageSize.ToString() },
                { nameof(request.PageIndex), request.PageIndex.ToString() },
                { nameof(request.Range), request.Range.ToString()  }
            }.ToString()
        };
        return await _apiService.GetAsync<PagedResponse<SongUsageStatModel>>(uri.ToString());
    }

    public async Task<PagedResponse<SongUsageStatModel>> GetMostAddedToLibraryAsync(PagedRequest request)
    {
        UriBuilder uri = new($"{_apiOptions.SongsEndpoint}/most-added")
        {
            Query = new QueryBuilder
            {
                { nameof(request.PageSize), request.PageSize.ToString() },
                { nameof(request.PageIndex), request.PageIndex.ToString() }
            }.ToString()
        };
        return await _apiService.GetAsync<PagedResponse<SongUsageStatModel>>(uri.ToString());
    }

    public async Task<PagedResponse<LatestSongStatModel>> GetLatestPublicSongsAsync(PagedRequest request)
    {
        UriBuilder uri = new($"{_apiOptions.SongsEndpoint}/latest-public")
        {
            Query = new QueryBuilder
            {
                { nameof(request.PageSize), request.PageSize.ToString() },
                { nameof(request.PageIndex), request.PageIndex.ToString() }
            }.ToString()
        };
        return await _apiService.GetAsync<PagedResponse<LatestSongStatModel>>(uri.ToString());
    }
}