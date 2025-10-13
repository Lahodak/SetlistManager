using Blazored.LocalStorage;
using Newtonsoft.Json;

namespace SetlistManager.App.Services;

public class ApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILocalStorageService _localStorage;
    private readonly ILogger<ApiService> _logger;

    public ApiService(IHttpClientFactory factory, ILocalStorageService localStorageService, ILogger<ApiService> logger)
    {
        _httpClientFactory = factory;
        _logger = logger;
        _localStorage = localStorageService;
    }

    public async Task ConfigureHttpClientAsync(HttpClient httpClient)
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        if (!string.IsNullOrWhiteSpace(token))
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        using var httpClient = _httpClientFactory.CreateClient();
        await ConfigureHttpClientAsync(httpClient);

        var response = await httpClient.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        if(string.IsNullOrWhiteSpace(json))        
            return default;
        

        T? result;   
        
        try
        {
            result = JsonConvert.DeserializeObject<T?>(json);
        }
        catch(Exception ex)
        {
            _logger.Log(LogLevel.Error, ex, message: ex.Message);
            return default;
        }

        return result;
    }

    public async Task<T?> PostAsync<T>(string endpoint, T data)
    {
        using var httpClient = _httpClientFactory.CreateClient();
        await ConfigureHttpClientAsync(httpClient);
        string jsonData;

        try
        {
            jsonData = JsonConvert.SerializeObject(data);
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex, message: ex.Message);
            return default;
        }

        var content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(endpoint, content);
        response.EnsureSuccessStatusCode();
        var jsonResponse = await response.Content.ReadAsStringAsync();
        
        if (string.IsNullOrWhiteSpace(jsonResponse))        
            return default;        

        T? result;

        try
        {
            result = JsonConvert.DeserializeObject<T>(jsonResponse);
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex, message: ex.Message);
            return default;
        }

        return result;
    }

    public async Task<T?> PutAsync<T>(string endpoint, T data)
    {
        using var httpClient = _httpClientFactory.CreateClient();
        await ConfigureHttpClientAsync(httpClient);
        string jsonData;

        try
        {
            jsonData = JsonConvert.SerializeObject(data);
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex, message: ex.Message);
            return default;
        }

        var content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");
        var response = await httpClient.PutAsync(endpoint, content);

        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(jsonResponse))        
            return default;        

        T? result;
        
        try
        {
            result = JsonConvert.DeserializeObject<T>(jsonResponse);
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex, message: ex.Message);
            return default;
        }

        return result;
    }
}
