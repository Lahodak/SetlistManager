using Microsoft.Extensions.Options;
using SetlistManager.App.Extensions;
using SetlistManager.App.Options;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Services.Implementations;

public class SetlistService : ISetlistService
{
    private readonly IApiService _apiService;
    private readonly string _apiPath;

    public SetlistService(IApiService apiService, IOptions<SetlistManagerApiOptions> apiOptions)
    {
        _apiService = apiService;
        _apiPath = apiOptions.Value.BaseUrl + apiOptions.Value.SetlistsEndpoint;
    }
    
    public async Task<PagedResponse<SetlistModel>> GetSetlistsAsync(PagedRequest request)
    {
        var uri = request.ToPagedRequestUri(_apiPath);
        return await _apiService.GetAsync<PagedResponse<SetlistModel>>(uri);
    }

    public async Task<bool> TryCreateSetlistAsync(SetlistModel setlistModel)
        => await _apiService.TryPostAsync(_apiPath, setlistModel);

    public async Task<SetlistModel?> GetSetlistById(int id)
        => await _apiService.GetAsync<SetlistModel>($"{_apiPath}/{id}");

    public async Task<bool> TryEditSetlist(SetlistModel setlistModel)
        => await _apiService.TryPutAsync($"{_apiPath}/{setlistModel.Id}", setlistModel);

    public async Task<bool> TryDeleteSetlistAsync(int id)
        => await _apiService.TryDeleteAsync($"{_apiPath}/{id}");

    public async Task<bool> TryGiveAccessToUserAsync(int setlistId, int targetId)
        => await _apiService.TryPostAsync($"{_apiPath}/{setlistId}/users/{targetId}", true);

    public async Task<bool> TryRemoveAccessFromUserAsync(int setlistId, int targetId)
        => await _apiService.TryDeleteAsync($"{_apiPath}/{setlistId}/users/{targetId}");
}