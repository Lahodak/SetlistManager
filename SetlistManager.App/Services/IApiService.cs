namespace SetlistManager.App.Services;

public interface IApiService
{
    Task<T?> GetAsync<T>(string endpoint);
    Task<T?> PostAsync<T>(string endpoint, T data);
    Task<T?> PutAsync<T>(string endpoint, T data);
    Task<bool> TryDeleteAsync(string endpoint);
    Task<bool> TryPutAsync<T>(string endpoint, T data);
}