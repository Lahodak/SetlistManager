using Blazored.LocalStorage;
using Newtonsoft.Json;
using SetlistManager.Common.Models;
using System.Net.Http.Headers;
using System.Text;

namespace SetlistManager.Services;

public class SetlistService
{
    private const string _setlistsEndpointPath = "https://localhost:7143/api/Setlists";
    private const string _getAllSetlistsSuffix = "/getallsetlists";
    private const string _setlistByIdSuffix = "/";
    private const string _editSetlistSuffix = "/editsetlist";
    private const string _uploadSetlistSuffix = "/uploadsetlist";

    private readonly ApiService _apiService;

    public SetlistService(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task PushSetlist(SetlistModel setlistModel) 
        => await _apiService.PostAsync(_setlistsEndpointPath + _uploadSetlistSuffix, setlistModel);

    public async Task<SetlistModel>? GetSetlistById(int id) 
        => await _apiService.GetAsync<SetlistModel>(_setlistsEndpointPath + _setlistByIdSuffix + id.ToString());

    public async Task<List<SetlistModel>> GetAllSetlists() 
        => await _apiService.GetAsync<List<SetlistModel>>(_setlistsEndpointPath + _getAllSetlistsSuffix);

    public async Task EditSetlist(SetlistModel setlistModel) 
        => await _apiService.PostAsync(_setlistsEndpointPath + _editSetlistSuffix, setlistModel);
}