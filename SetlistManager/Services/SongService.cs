using Newtonsoft.Json;
using SetlistManager.Common.Models;
using System.Net.Http.Headers;
using System.Text;
using Blazored.LocalStorage;

namespace SetlistManager.Services;

public class SongService
{
    private const string _songsEndpointPath = "https://localhost:7143/api/Songs";
    private const string _getAllSongsSuffix = "/getallsongs";
    private const string _getSongbyIdSuffix = "/songbyid/";
    private const string _getSongbyNameSuffix = "/songbyname/";
    private const string _uploadSongCollectionSuffix = "/addsongcollection";
    private const string _uploadSongSuffix = "/addsong";

    private readonly ApiService _apiService;

    public SongService(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<List<SongModel>?> GetAllSongsAsync() 
        => await _apiService.GetAsync<List<SongModel>>(_songsEndpointPath + _getAllSongsSuffix);

    public async Task<SongModel?> GetSongByIdAsync(int id) 
        => await _apiService.GetAsync<SongModel>(_songsEndpointPath + _getSongbyIdSuffix + id.ToString());

    public async Task<SongModel?> GetSongByNameAsync(string name) 
        => await _apiService.GetAsync<SongModel>(_songsEndpointPath + _getSongbyNameSuffix + name);

    public async Task UploadSongsAsync(List<SongModel> songsToUpload) 
        => await _apiService.PostAsync(_songsEndpointPath + _uploadSongCollectionSuffix, songsToUpload);

    public async Task UploadSongAsync(SongModel songToUpload) 
        => await _apiService.PostAsync(_songsEndpointPath + _uploadSongSuffix, songToUpload);
}