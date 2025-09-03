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
    private const string _tokenKey = "authToken";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILocalStorageService _localStorage;

    public SongService(IHttpClientFactory factory, ILocalStorageService localStorageService)
    {
        _httpClientFactory = factory;
        _localStorage = localStorageService;
    }

    private async Task ConfigureHttpClientAsync(HttpClient httpClient)
    {
        var token = await _localStorage.GetItemAsync<string>(_tokenKey);

        if (!string.IsNullOrWhiteSpace(token))
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<List<SongModel>?> GetAllSongsAsync()
    {
        using var httpClient = _httpClientFactory.CreateClient();

        await ConfigureHttpClientAsync(httpClient);

        HttpResponseMessage message = await httpClient.GetAsync(_songsEndpointPath + _getAllSongsSuffix);        

        if (!message.IsSuccessStatusCode)
        {
            Console.WriteLine(message.ToString());
            return null;
        }

        string json = await message.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<SongModel>>(json);
    }      

    public async Task<SongModel?> GetSongByIdAsync(int id)
    {
        using var httpClient = _httpClientFactory.CreateClient();

        await ConfigureHttpClientAsync(httpClient);

        HttpResponseMessage message = await httpClient.GetAsync(_songsEndpointPath + _getSongbyIdSuffix + id.ToString());

        if (!message.IsSuccessStatusCode)
        {
            Console.WriteLine(message.ToString());
            return null;
        }

        string json = await message.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<SongModel>(json);
    }

    public async Task<SongModel?> GetSongByNameAsync(string name)
    {
        using var httpClient = _httpClientFactory.CreateClient();

        await ConfigureHttpClientAsync(httpClient);

        HttpResponseMessage message = await httpClient.GetAsync(_songsEndpointPath + _getSongbyNameSuffix + name);

        if (!message.IsSuccessStatusCode)
        {
            Console.WriteLine(message.ToString());
            return null;
        }

        string json = await message.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<SongModel>(json);
    }

    public async Task UploadSongsAsync(List<SongModel> songsToUpload)
    {
        using var httpClient = _httpClientFactory.CreateClient();

        await ConfigureHttpClientAsync(httpClient);

        string songs;
        
        try
        {
            songs = JsonConvert.SerializeObject(songsToUpload);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return;
        }

        var content = new StringContent(songs, Encoding.UTF8, "application/json");
        HttpResponseMessage message = await httpClient.PostAsync(_songsEndpointPath + _uploadSongCollectionSuffix, content);

        if (!message.IsSuccessStatusCode)
        {
            Console.WriteLine(message.ToString());
        }
    }

    public async Task UploadSongAsync(SongModel songToUpload)
    {
        using var httpClient = _httpClientFactory.CreateClient();

        await ConfigureHttpClientAsync(httpClient);

        string song;
        
        try
        {
            song = JsonConvert.SerializeObject(songToUpload);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return;
        }
        
        var content = new StringContent(song, Encoding.UTF8, "application/json");
        HttpResponseMessage message = await httpClient.PostAsync(_songsEndpointPath + _uploadSongSuffix, content);
        
        if (!message.IsSuccessStatusCode)
        {
            Console.WriteLine(message.ToString());
        }
    }
}