using Newtonsoft.Json;
using SetlistManager.Common.Models;
using System.Text;

namespace SetlistManager.Services;

public class SetlistService
{
    private const string _pathSetlists = "https://localhost:7143/api/Setlists";
    private const string _pathGetAllSetlists = "https://localhost:7143/GetAllSetlists";

    private const string _pathSetlistById = "https://localhost:7143/api/Setlists/";    
    private readonly IHttpClientFactory _httpClientFactory;
    public SetlistService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<int> PushSetlist(SetlistModel setlistModel)
    {
        using var httpClient = _httpClientFactory.CreateClient();
        var s  = JsonConvert.SerializeObject(setlistModel);
        var content = new StringContent(s, Encoding.UTF8, "application/json");
        HttpResponseMessage message = await httpClient.PostAsync(_pathSetlists, content);
        if (!message.IsSuccessStatusCode)
        {
            Console.WriteLine(message.ToString());
            return -1;
        }

        string responseContent = await message.Content.ReadAsStringAsync();
        if (int.TryParse(responseContent, out int result))
        {
            return result;
        }

        Console.WriteLine("Failed to parse response content.");
        return -1;
    }

    public async Task<SetlistModel>? GetSetlistById(int id)
    {
        using var httpClient = _httpClientFactory.CreateClient();
        string str = _pathSetlistById + id.ToString();
        HttpResponseMessage message = await httpClient.GetAsync(str);
        if (!message.IsSuccessStatusCode)
        {
            Console.WriteLine(message.ToString());
            return null!;
        }
        string json = await message.Content.ReadAsStringAsync();
        try
        {
            return JsonConvert.DeserializeObject<SetlistModel>(json)!;

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null!;
        }
    }

    public async Task<List<SetlistModel>> GetAllSetlists()
    {
        using var httpClient = _httpClientFactory.CreateClient();
        string str = _pathGetAllSetlists;
        HttpResponseMessage message = await httpClient.GetAsync(str);

        if (!message.IsSuccessStatusCode)
        {
            Console.WriteLine(message.ToString());
            return null!;
        }

        string json = await message.Content.ReadAsStringAsync();

        try
        {
            return JsonConvert.DeserializeObject<List<SetlistModel>>(json)!;

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null!;
        }
    }
}