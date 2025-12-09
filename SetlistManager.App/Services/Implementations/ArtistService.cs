using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using SetlistManager.App.Options;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Services.Implementations;

public class ArtistService : IArtistService
{
    private readonly IApiService _apiService;
    private readonly IOptions<SetlistManagerApiOptions> _apiOptions;

    public ArtistService(IApiService apiService, IOptions<SetlistManagerApiOptions> apiOptions)
    {
        _apiOptions = apiOptions;
        _apiService = apiService;
    }

    public async Task<PagedResponse<ArtistModel>?> GetAvailableArtistsAsync(PagedRequest request)
    {
        UriBuilder uri = new(_apiOptions.Value.ArtistsEndpoint)
        {
            Query = new QueryBuilder
            {
                { "PageSize", request.PageSize.ToString() },
                { "PageIndex", request.PageIndex.ToString() },
                { "Query", request.Query ?? string.Empty }
            }.ToString()
        };

        return await _apiService.GetAsync<PagedResponse<ArtistModel>?>(uri.ToString());
    }

    public async Task<ArtistModel?> GetArtistByIdAsync(int id) 
        => await _apiService.GetAsync<ArtistModel>(_apiOptions.Value.ArtistsEndpoint + "/" + id.ToString());

    public async Task UploadArtistAsync(ArtistCreateModel createModel)
        => await _apiService.PostAsync(_apiOptions.Value.ArtistsEndpoint, createModel);

    public async Task<bool> TryDeleteArtistAsync(int id)
        => await _apiService.TryDeleteAsync(_apiOptions.Value.ArtistsEndpoint + "/" + id.ToString());

    public async Task<bool> TryUpdateArtistAsync(int id, ArtistUpdateModel updateModel)
        => await _apiService.TryPutAsync(_apiOptions.Value.ArtistsEndpoint + "/" + id.ToString(), updateModel);
}