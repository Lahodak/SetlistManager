using SetlistManager.Common.Models;

namespace SetlistManager.App.Services.Implementations;

public class SetlistService : ISetlistService
{
    private readonly string _setlistsEndpointPath;
    private const string _setlistByIdSuffix = "/";
    private const string _getSetlistByNameSuffix = "/";

    private readonly IApiService _apiService;
     
    public SetlistService(IApiService apiService, IConfiguration configuration)
    {
        _setlistsEndpointPath = configuration["SetlistManager.Api:SetlistsEndpoint"]!;
        _apiService = apiService;
    }

    public async Task PushSetlist(SetlistModel setlistModel) 
        => await _apiService.PostAsync(_setlistsEndpointPath, setlistModel);

    public async Task<SetlistModel?> GetSetlistById(int id) 
        => await _apiService.GetAsync<SetlistModel>(_setlistsEndpointPath + _setlistByIdSuffix + id.ToString());

    public async Task<List<SetlistModel>?> GetAllSetlistsAsync() 
        => await _apiService.GetAsync<List<SetlistModel>>(_setlistsEndpointPath);

    public async Task<SetlistModel?> GetSetlistByNameAsync(string name)
        => await _apiService.GetAsync<SetlistModel?>(_setlistsEndpointPath + _getSetlistByNameSuffix + name);

    public async Task EditSetlist(SetlistModel setlistModel) 
        => await _apiService.PutAsync(_setlistsEndpointPath, setlistModel); 
}