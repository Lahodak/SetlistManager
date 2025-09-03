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
    private const string _identityEndpointPath = "https://localhost:7143/api/Identity";
    private const string _registerUserSuffix = "/register";
    private const string _loginUserSuffix = "/login";
    private const string _userEndpointPath = "https://localhost:7143/api/User";
    private const string _updateUserSuffix = "/updateuser";
    private const string _getUserSuffix = "/getuserdetail";
    private const string _tokenKey = "authToken";

    private readonly IHttpClientFactory _httpClientFactory; 
    private readonly ILocalStorageService _localStorage;

    public UserService(IHttpClientFactory httpClientFactory, ILocalStorageService localStorageService)
    {
        _httpClientFactory = httpClientFactory;
        _localStorage = localStorageService;
    }

    public async Task<UserModel?> GetUserAsync()
    {
        var httpClient = _httpClientFactory.CreateClient();
        string? token = await _localStorage.GetItemAsync<string>(_tokenKey);

        if (string.IsNullOrWhiteSpace(token))
            return null;

        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        HttpResponseMessage message = await httpClient.GetAsync(_userEndpointPath + _getUserSuffix);

        if (!message.IsSuccessStatusCode)
            return null;

        string json = await message.Content.ReadAsStringAsync();

        if (json is null)
            return null;

        UserModel? user;

        try
        {
            user = JsonConvert.DeserializeObject<UserModel>(json);
            return user;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }

    public async Task RegisterAsync(RegisterRequestModel model)
    {
        var client = _httpClientFactory.CreateClient();
        string user;

        try
        {
            user = JsonConvert.SerializeObject(model);
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
            return;
        }

        var content = new StringContent(user, Encoding.UTF8, "application/json");
        HttpResponseMessage message = await client.PostAsync(_identityEndpointPath + _registerUserSuffix, content);
    }
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
        HttpResponseMessage message = await client.PostAsync(_identityEndpointPath + _loginUserSuffix, content);

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