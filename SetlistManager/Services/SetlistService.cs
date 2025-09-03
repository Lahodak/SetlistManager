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
    private const string _tokenKey = "authToken";
    private readonly ILocalStorageService _localStorage;

    private readonly IHttpClientFactory _httpClientFactory;
    public SetlistService(IHttpClientFactory httpClientFactory, ILocalStorageService localStorage)
    {
        _httpClientFactory = httpClientFactory;
        _localStorage = localStorage;
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

    public async Task<int> PushSetlist(SetlistModel setlistModel)
    {
        using var httpClient = _httpClientFactory.CreateClient();

        await ConfigureHttpClientAsync(httpClient);

        string setlist;

        try
        {
            setlist = JsonConvert.SerializeObject(setlistModel);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return -1;
        }
        
        var content = new StringContent(setlist, Encoding.UTF8, "application/json");
        HttpResponseMessage message = await httpClient.PostAsync(_setlistsEndpointPath, content);
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

        await ConfigureHttpClientAsync(httpClient);

        string str = _setlistsEndpointPath + _setlistByIdSuffix + id.ToString();
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

        await ConfigureHttpClientAsync(httpClient);

        string str = _setlistsEndpointPath + _getAllSetlistsSuffix;
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

    public async Task EditSetlist(SetlistModel setlistModel)
    {
        using var httpClient = _httpClientFactory.CreateClient();

        await ConfigureHttpClientAsync(httpClient);

        string setlist;

        try
        {
            setlist = JsonConvert.SerializeObject(setlistModel);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return;
        }
                
        var content = new StringContent(setlist, Encoding.UTF8, "application/json");
        HttpResponseMessage message = await httpClient.PostAsync(_setlistsEndpointPath + _editSetlistSuffix, content);

        if (!message.IsSuccessStatusCode)
        {
            Console.WriteLine(message.ToString());
        }
    }
}