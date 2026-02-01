using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using SetlistManager.App.Options;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Services.Implementations;

public class SetlistService : ISetlistService
{
    private readonly IApiService _apiService;
    private readonly SetlistManagerApiOptions _apiOptions;
    public SetlistService(IApiService apiService, IOptions<SetlistManagerApiOptions> apiOptions)
    {
        _apiOptions = apiOptions.Value;
        _apiService = apiService;
    }

    public async Task SaveSetlistAsync(SetlistModel setlistModel) 
        => await _apiService.PostAsync(_apiOptions.SetlistsEndpoint, setlistModel);

    public async Task<SetlistModel?> GetSetlistById(int id) 
        => await _apiService.GetAsync<SetlistModel>($"{_apiOptions.SetlistsEndpoint}/{id}");

    public async Task<PagedResponse<SetlistModel>?> GetAllSetlistsAsync(PagedRequest request)
    {
        UriBuilder uri = new(_apiOptions.SetlistsEndpoint)
        {
            Query = new QueryBuilder
            {
                { nameof(request.PageSize), request.PageSize.ToString() },
                { nameof(request.PageIndex), request.PageIndex.ToString() },
                { nameof(request.Query), request.Query ?? string.Empty },
                { nameof(request.ContentType), request.ContentType.ToString()}
            }.ToString()
        };

        return await _apiService.GetAsync<PagedResponse<SetlistModel>?>(uri.ToString());
    }

    public async Task EditSetlist(SetlistModel setlistModel)
        => await _apiService.PutAsync($"{_apiOptions.SetlistsEndpoint}/{setlistModel.Id}", setlistModel);

    public async Task<bool> TryDeleteSetlistAsync(int id)
        => await _apiService.TryDeleteAsync($"{_apiOptions.SetlistsEndpoint}/{id}");

    public async Task<bool> TryGiveAccessToUserAsync(int setlistId, int targetId)
        => await _apiService.PostAsync($"{_apiOptions.SetlistsEndpoint}/{setlistId}/setlistsusers/{targetId}", true);

    public async Task RemoveAccessFromUserAsync(int setlistId, int targetId)
        => await _apiService.TryDeleteAsync($"{_apiOptions.SetlistsEndpoint}/{setlistId}/setlistsusers/{targetId}");
}