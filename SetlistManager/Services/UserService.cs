using Blazored.LocalStorage;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SetlistManager.Common.Models;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;


namespace SetlistManager.Services;
public class UserService
{
    private const string _userEndpointPath = "https://localhost:7143/api/users";  
    private const string _loginUserSuffix = "/auth";
    private const string _tokenKey = "authToken";

    private readonly IHttpClientFactory _httpClientFactory; 
    private readonly ILocalStorageService _localStorage;
    private readonly ApiService _apiService;

    public UserService(IHttpClientFactory httpClientFactory, ILocalStorageService localStorageService, ApiService apiService)
    {
        _httpClientFactory = httpClientFactory;
        _localStorage = localStorageService;
        _apiService = apiService;
    }

    public async Task<UserModel?> GetUserAsync() 
        => await _apiService.GetAsync<UserModel>(_userEndpointPath);

    public async Task RegisterAsync(RegisterRequestModel model) 
        => await _apiService.PostAsync(_userEndpointPath, model);

    public async Task LogOutAsync()
    {
        await _localStorage.RemoveItemAsync(_tokenKey);
    }   

    public async Task<string> GetUserToken()
    {
        return await _localStorage.GetItemAsync<string>(_tokenKey);
    }

    public async Task<bool> IsUserLoggedInAsync()
    {
        var token = await _localStorage.GetItemAsync<string>(_tokenKey);
        if (string.IsNullOrWhiteSpace(token))
            return false;
        return true;
    }

    public async Task LogInAsync(LoginRequestModel model)
    {
        var client = _httpClientFactory.CreateClient();
        string user;

        try
        {
            user = JsonConvert.SerializeObject(model);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return;
        }

        var content = new StringContent(user, Encoding.UTF8, "application/json");
        HttpResponseMessage message = await client.PostAsync(_userEndpointPath + _loginUserSuffix, content);

        if (!message.IsSuccessStatusCode)
            return;
        
        string json = await message.Content.ReadAsStringAsync();

        if (json is null)
            return;
        
        LoginResultModel loginResult;

        try
        {
            loginResult = JsonConvert.DeserializeObject<LoginResultModel>(json);
            
            if (loginResult.Token is null  || loginResult is null)
                return;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return;
        }

        await _localStorage.SetItemAsync(_tokenKey, loginResult.Token);
    }
}