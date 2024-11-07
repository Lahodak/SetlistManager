using Newtonsoft.Json;
using SetlistManager.Common.Models;

namespace SetlistManager.Services;

public class SongService
{
    private const string path = "https://localhost:7143/api/Songs";
    private IHttpClientFactory _httpClientFactory;
    public SongService(IHttpClientFactory factory)
    {
        _httpClientFactory = factory;
    }

    public async Task<List<SongModel>?> FetchSongsFromAPI()
    {
        using var httpClient = _httpClientFactory.CreateClient();
        HttpResponseMessage message = await httpClient.GetAsync(path);
        if (!message.IsSuccessStatusCode)
        {
            Console.WriteLine(message.ToString());
            return null;
        }
        string json = await message.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<SongModel>>(json);
    }    
    public async Task PushSetlistToApi()
    {

    }
}