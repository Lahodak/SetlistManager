using Newtonsoft.Json;
using SetlistManager.Common.Models;
using System.IO;
using System.Text;

namespace SetlistManager.Services;

public class SetlistService
{
    private const string _pathSetlists = "https://localhost:7143/api/Setlists";
    private IHttpClientFactory _httpClientFactory;
    public SetlistService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    public async Task PushSetlistToApi(SetlistModel setlistModel)
    {
        using var httpClient = _httpClientFactory.CreateClient();
        var s  = JsonConvert.SerializeObject(setlistModel);
        var content = new StringContent(s, Encoding.UTF8, "application/json");
        HttpResponseMessage message = await httpClient.PostAsync(_pathSetlists, content);
        if (!message.IsSuccessStatusCode)
        {
            Console.WriteLine(message.ToString());
        }
        string json = await message.Content.ReadAsStringAsync();
    }
}