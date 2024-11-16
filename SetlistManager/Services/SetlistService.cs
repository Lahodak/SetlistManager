using Newtonsoft.Json;
using SetlistManager.Common.Models;
using System.IO;
using System.Net.Http;
using System.Text;

namespace SetlistManager.Services;

public class SetlistService
{
    private const string _pathSetlists = "https://localhost:7143/api/Setlists";
    private const string _pathSetlistById = "https://localhost:7143/api/Setlists/";    
    private IHttpClientFactory _httpClientFactory;
    public SetlistService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task PushSetlist(SetlistModel setlistModel)
    {
        using var httpClient = _httpClientFactory.CreateClient();
        var s  = JsonConvert.SerializeObject(setlistModel);
        var content = new StringContent(s, Encoding.UTF8, "application/json");
        HttpResponseMessage message = await httpClient.PostAsync(_pathSetlists, content);
        if (!message.IsSuccessStatusCode)
        {
            Console.WriteLine(message.ToString());
        }
    }

    public async Task<SetlistModel>? GetSetlistById(int id)
    {
        using var httpClient = _httpClientFactory.CreateClient();
        string str = _pathSetlistById + id.ToString();
        HttpResponseMessage message = await httpClient.GetAsync(str);
        if (!message.IsSuccessStatusCode)
        {
            Console.WriteLine(message.ToString());
            return null;
        }
        string json = await message.Content.ReadAsStringAsync();
        try
        {
            return JsonConvert.DeserializeObject<SetlistModel>(json);

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }
}