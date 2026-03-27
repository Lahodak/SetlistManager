using Blazored.LocalStorage;
using System.Net.Http.Json;

namespace SetlistManager.App.Services.Implementations;

public class ApiService : IApiService
{
    private readonly ILocalStorageService _localStorage;
    private readonly HttpClient _client;
    private const string _authTokenKey = "authToken";
    private const string _bearerHeaderKey = "Bearer";

    public ApiService(HttpClient client, ILocalStorageService localStorageService)
    {
        _client = client;
        _localStorage = localStorageService;
    }

    private async Task ConfigureHttpClientAsync(HttpClient httpClient)
    {
        var token = await _localStorage.GetItemAsync<string>(_authTokenKey);
        if (!string.IsNullOrWhiteSpace(token))
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(_bearerHeaderKey, token);
        }
    }

    public async Task<bool> TryDeleteAsync(string endpoint)
    {        
        await ConfigureHttpClientAsync(_client);
        
        var response = await _client.DeleteAsync(endpoint);
        
        if(response.IsSuccessStatusCode)
            return true;

        return false;
    }

    public async Task<bool> TryPostAsync<T>(string endpoint, T data)
    {
        await ConfigureHttpClientAsync(_client);    
        
        var response = await _client.PostAsJsonAsync(endpoint, data);
        
        if (response.IsSuccessStatusCode)
            return true;
        
        return false;
    }

    public async Task<bool> TryPutAsync<T>(string endpoint, T data)
    {
        await ConfigureHttpClientAsync(_client);        

        var response = await _client.PutAsJsonAsync(endpoint, data);
        
        if (response.IsSuccessStatusCode)
            return true;
        
        return false;
    }

    public async Task<T> GetAsync<T>(string endpoint)
    {
        await ConfigureHttpClientAsync(_client);

        return await _client.GetFromJsonAsync<T>(endpoint) ?? default!;
    }

    public async Task<T?> PostAsync<T>(string endpoint, T data)
    {
        await ConfigureHttpClientAsync(_client);

        var response = await _client.PostAsJsonAsync(endpoint, data);

        if(response.Content is null || !response.IsSuccessStatusCode)        
            return default;
       
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task<bool> PostAsync(string endpoint)
    {
        await ConfigureHttpClientAsync(_client);
        
        var response = await _client.PostAsync(endpoint, null);
        
        if (response.IsSuccessStatusCode)
            return true;

        return false;
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        await ConfigureHttpClientAsync(_client);

        var response = await _client.PostAsJsonAsync(endpoint, data);

        if (response.Content is null || !response.IsSuccessStatusCode)
            return default;

        return await response.Content.ReadFromJsonAsync<TResponse>();
    }

    public async Task<T?> PutAsync<T>(string endpoint, T data)
    {
        await ConfigureHttpClientAsync(_client);

        var response = await _client.PutAsJsonAsync(endpoint, data);

        if (response.Content is null || !response.IsSuccessStatusCode)        
            return default;

        return await response.Content.ReadFromJsonAsync<T>();
    }
}