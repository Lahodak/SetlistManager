using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
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

    public async Task<PagedResponse<ArtistModel>> GetAvailableArtistsAsync(PagedRequest request)
    {
        UriBuilder uri = new(_apiOptions.ArtistsEndpoint)
        {
            Query = new QueryBuilder
            {
                { nameof(request.PageSize), request.PageSize.ToString() },
                { nameof(request.PageIndex), request.PageIndex.ToString() },
                { nameof(request.Query), request.Query ?? string.Empty },
                { nameof(request.ContentType), request.ContentType.ToString() }
            }.ToString()
        };

        return await _apiService.GetAsync<PagedResponse<ArtistModel>>(uri.ToString());
    }

    public async Task<ArtistModel?> GetArtistByIdAsync(int id) 
        => await _apiService.GetAsync<ArtistModel>($"{_apiOptions.ArtistsEndpoint}/{id}" );

    public async Task UploadArtistAsync(ArtistCreateModel createModel)
        => await _apiService.PostAsync(_apiOptions.ArtistsEndpoint, createModel);

    public async Task<bool> TryDeleteArtistAsync(int id)
        => await _apiService.TryDeleteAsync($"{_apiOptions.ArtistsEndpoint}/{id}");

    public async Task<bool> TryUpdateArtistAsync(int id, ArtistUpdateModel updateModel)
        => await _apiService.TryPutAsync($"{_apiOptions.ArtistsEndpoint}/{id}", updateModel);

    public async Task<bool> TryGiveAccessToUserAsync(int artistId, int targetId)
        => await _apiService.PostAsync($"{_apiOptions.ArtistsEndpoint}/{artistId}/users/{targetId}", true);

    public async Task<bool> TryMakeArtistPublicAsync(int id)
        => await _apiService.PostAsync($"{_apiOptions.ArtistsEndpoint}/{id}/public", true);

    public async Task RemoveAccessFromUserAsync(int artistId, int targetId)
        => await _apiService.TryDeleteAsync($"{_apiOptions.ArtistsEndpoint}/{artistId}/users/{targetId}");
}