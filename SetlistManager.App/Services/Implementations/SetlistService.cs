using Microsoft.Extensions.Options;
using SetlistManager.App.Options;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Services.Implementations;

public class SetlistService : ISetlistService
{
    private const string _setlistByIdSuffix = "/";
    private const string _getSetlistByNameSuffix = "/";

    private readonly IApiService _apiService;
    private readonly IOptions<SetlistManagerApiOptions> _apiOptions;

    public SetlistService(IApiService apiService, IOptions<SetlistManagerApiOptions> apiOptions)
    {
        _apiOptions = apiOptions;
        _apiService = apiService;
    }

    public async Task PushSetlist(SetlistModel setlistModel) 
        => await _apiService.PostAsync(_apiOptions.Value.SetlistsEndpoint, setlistModel);

    public async Task<SetlistModel?> GetSetlistById(int id) 
        => await _apiService.GetAsync<SetlistModel>(_apiOptions.Value.SetlistsEndpoint + _setlistByIdSuffix + id.ToString());

    public async Task<List<SetlistModel>?> GetAllSetlistsAsync() 
        => await _apiService.GetAsync<List<SetlistModel>>(_apiOptions.Value.SetlistsEndpoint);

    public async Task<SetlistModel?> GetSetlistByNameAsync(string name)
        => await _apiService.GetAsync<SetlistModel?>(_apiOptions.Value.SetlistsEndpoint + _getSetlistByNameSuffix + name);

    public async Task EditSetlist(SetlistModel setlistModel) 
        => await _apiService.PutAsync(_apiOptions.Value.SetlistsEndpoint, setlistModel); 
}