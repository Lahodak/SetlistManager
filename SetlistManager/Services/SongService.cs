using Newtonsoft.Json;
using SetlistManager.Common.Models;

namespace SetlistManager.Services;

public class SongService
{
    private const string _songsEndpointPath = "https://localhost:7143/api/Songs";
    private const string _getAllSongsSuffix = "/getallsongs";
    private const string _getSongbyIdSuffix = "/songbyid/";
    private const string _getSongbyNameSuffix = "/songbyname/";
    private const string _uploadSongsSuffix = "/addsongs";


    private readonly IHttpClientFactory _httpClientFactory;
    public SongService(IHttpClientFactory factory)
    {
        _httpClientFactory = factory;
    }

    public async Task<List<SongModel>?> GetAllSongsAsync()
    {
        using var httpClient = _httpClientFactory.CreateClient();
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
        HttpResponseMessage message = await httpClient.GetAsync(_songsEndpointPath + _getSongbyNameSuffix + name);

        if (!message.IsSuccessStatusCode)
        {
            Console.WriteLine(message.ToString());
            return null;
        }

        string json = await message.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<SongModel>(json);
    }

    public async Task UploadSongsAsync(List<SongModel> songs)
    {
        using var httpClient = _httpClientFactory.CreateClient();

    }
}