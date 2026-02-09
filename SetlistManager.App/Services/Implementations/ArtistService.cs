using Microsoft.Extensions.Options;
using SetlistManager.App.Extensions;
using SetlistManager.App.Options;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Services.Implementations;

public class ArtistService : IArtistService
{
    private readonly IApiService _apiService;
    private readonly SetlistManagerApiOptions _apiOptions;

    public ArtistService(IApiService apiService, IOptions<SetlistManagerApiOptions> apiOptions)
    {
        _apiOptions = apiOptions.Value;
        _apiService = apiService;
    }

    public async Task<PagedResponse<ArtistModel>> GetArtistsAsync(ContentPagedRequest request)
    {
        var uri = request.ToContentPagedRequestUri(_apiOptions.ArtistsEndpoint);
        return await _apiService.GetAsync<PagedResponse<ArtistModel>>(uri);
    }

    public async Task<ArtistModel?> GetArtistByIdAsync(int id) 
        => await _apiService.GetAsync<ArtistModel>($"{_apiOptions.ArtistsEndpoint}/{id}" );

    public async Task<bool> TryCreateArtistAsync(ArtistCreateModel createModel)
        => await _apiService.TryPostAsync(_apiOptions.ArtistsEndpoint, createModel);

    public async Task<bool> TryDeleteArtistAsync(int id)
        => await _apiService.TryDeleteAsync($"{_apiOptions.ArtistsEndpoint}/{id}");

    public async Task<bool> TryUpdateArtistAsync(int id, ArtistUpdateModel updateModel)
        => await _apiService.TryPutAsync($"{_apiOptions.ArtistsEndpoint}/{id}", updateModel);

    public async Task<bool> TryGiveAccessToUserAsync(int artistId, int targetId)
        => await _apiService.TryPostAsync($"{_apiOptions.ArtistsEndpoint}/{artistId}/users/{targetId}", true);

    public async Task<bool> TryMakeArtistPublicAsync(int id)
        => await _apiService.TryPostAsync($"{_apiOptions.ArtistsEndpoint}/{id}/public", true);

    public async Task<bool> TryRemoveAccessFromUserAsync(int artistId, int targetId)
        => await _apiService.TryDeleteAsync($"{_apiOptions.ArtistsEndpoint}/{artistId}/users/{targetId}");
}