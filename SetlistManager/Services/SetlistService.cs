using SetlistManager.Common.Models;

namespace SetlistManager.Services;

public class SetlistService
{
    private const string _setlistsEndpointPath = "https://localhost:7143/api/setlists";
    private const string _setlistByIdSuffix = "/";
    private const string _getSetlistByNameSuffix = "/";

    private readonly ApiService _apiService;

    public SetlistService(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task PushSetlist(SetlistModel setlistModel) 
        => await _apiService.PostAsync(_setlistsEndpointPath, setlistModel);

    public async Task<SetlistModel>? GetSetlistById(int id) 
        => await _apiService.GetAsync<SetlistModel>(_setlistsEndpointPath + _setlistByIdSuffix + id.ToString());

    public async Task<List<SetlistModel>> GetAllSetlists() 
        => await _apiService.GetAsync<List<SetlistModel>>(_setlistsEndpointPath);

    public async Task<SetlistModel?> GetSetlistByNameAsync(string name)
        => await _apiService.GetAsync<SetlistModel?>(_setlistsEndpointPath + _getSetlistByNameSuffix + name);

    public async Task EditSetlist(SetlistModel setlistModel) 
        => await _apiService.PutAsync(_setlistsEndpointPath, setlistModel);
}