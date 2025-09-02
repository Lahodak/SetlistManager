using Blazored.LocalStorage;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SetlistManager.Common.Models;
using System.Net.Http;
using System.Text;

namespace SetlistManager.Services;
public class UserService
{
    private const string _identityEndpointPath = "https://localhost:7143/api/Identity";
    private const string _registerUserSuffix = "/register";
    private const string _loginUserSuffix = "/login";
    private const string _tokenKey = "authToken";

    private readonly IHttpClientFactory _httpClientFactory; 
    private readonly ILocalStorageService _localStorage;

    public UserService(IHttpClientFactory httpClientFactory, ILocalStorageService localStorageService)
    {
        _httpClientFactory = httpClientFactory;
        _localStorage = localStorageService;
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