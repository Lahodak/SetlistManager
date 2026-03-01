namespace SetlistManager.App.Services;

/// <summary>
/// Provides generic HTTP methods for communicating with the SetlistManager API.
/// </summary>
public interface IApiService
{
    /// <summary>Sends a GET request and deserializes the response.</summary>
    /// <typeparam name="T">The expected response type.</typeparam>
    /// <param name="endpoint">The API endpoint URL.</param>
    Task<T> GetAsync<T>(string endpoint);

    /// <summary>Sends a POST request with a body and deserializes the response.</summary>
    Task<T?> PostAsync<T>(string endpoint, T data);

    /// <summary>Sends a POST request with no body and returns success.</summary>
    Task<bool> PostAsync(string endpoint);

    /// <summary>Sends a POST request with a body and deserializes into a different response type.</summary>
    Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data);

    /// <summary>Sends a PUT request with a body and deserializes the response.</summary>
    Task<T?> PutAsync<T>(string endpoint, T data);

    /// <summary>Sends a POST request and returns whether the request succeeded.</summary>
    Task<bool> TryPostAsync<T>(string endpoint, T data);

    /// <summary>Sends a DELETE request and returns whether the request succeeded.</summary>
    Task<bool> TryDeleteAsync(string endpoint);

    /// <summary>Sends a PUT request and returns whether the request succeeded.</summary>
    Task<bool> TryPutAsync<T>(string endpoint, T data);
}