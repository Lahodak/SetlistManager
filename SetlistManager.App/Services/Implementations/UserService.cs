using Blazored.LocalStorage;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SetlistManager.App.Options;
using SetlistManager.Common.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace SetlistManager.App.Services.Implementations;
public class UserService : IUserService
{
    private const string _loginUserSuffix = "/login";
    private const string _tokenKey = "authToken";
    private const string _getUserSetlistsSuffix = "/Setlists";
    private const string _verifyEmailSuffix = "/verify";
    private const string _resetPasswordSuffix = "/reset-password";
    private const string _resetPasswordRequestSuffix = "/request-password-reset";

    private readonly IHttpClientFactory _httpClientFactory; 
    private readonly ILocalStorageService _localStorage;
    private readonly IApiService _apiService;
    private readonly ILogger<UserService> _logger;
    private readonly IOptions<SetlistManagerApiOptions> _apiOptions;

    public UserService(IHttpClientFactory httpClientFactory, ILocalStorageService localStorageService, IApiService apiService, 
        ILogger<UserService> logger, IOptions<SetlistManagerApiOptions> apiOptions)
    {
        _apiOptions = apiOptions;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _localStorage = localStorageService;
        _apiService = apiService;
    }

    public async Task AddNewProviderToken(TokenCreateModel tokenModel) 
        => await _apiService.PutAsync(_apiOptions.Value.UsersEndpoint + "/tokens", tokenModel);

    public async Task<UserModel?> GetUserAsync() 
        => await _apiService.GetAsync<UserModel>(_apiOptions.Value.UsersEndpoint);

    public async Task<List<SetlistModel>?> GetAllUserSetlists()
    {
        UserModel? user = await GetUserAsync();
        
        if(user is null)
            return null;

        return await _apiService.GetAsync<List<SetlistModel>?>(_apiOptions.Value.UsersEndpoint + "/" + user.Id.ToString() + _getUserSetlistsSuffix);
    }

    public async Task RegisterAsync(RegisterRequestModel model) 
        => await _apiService.PostAsync(_apiOptions.Value.AuthEndpoint, model);

    public async Task LogOutAsync()
    {
        await _localStorage.RemoveItemAsync(_tokenKey);
    }   

    public async Task<string?> GetUserToken()
    {
        return await _localStorage.GetItemAsync<string>(_tokenKey);
    }

    public async Task<bool> IsUserLoggedInAsync()
    {
        var token = await _localStorage.GetItemAsync<string>(_tokenKey);

        if (string.IsNullOrWhiteSpace(token))
            return false;

        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(token))
            return false;

        var jwt = handler.ReadJwtToken(token);

        var exp = jwt.ValidTo;

        if (exp < DateTime.UtcNow)
            return false;

        return true;
    }

    public async Task UpdateUser(UserModel user) 
        => await _apiService.PutAsync(_apiOptions.Value.UsersEndpoint, user);

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
            _logger.Log(LogLevel.Error, ex, message: ex.Message);
            return;
        }

        var content = new StringContent(user, Encoding.UTF8, "application/json");
        HttpResponseMessage message = await client.PostAsync(_apiOptions.Value.AuthEndpoint + _loginUserSuffix, content);

        if (!message.IsSuccessStatusCode)
            return;
        
        string json = await message.Content.ReadAsStringAsync();

        if (json is null)
            return;
        
        LoginResultModel? loginResult;

        try
        {
            loginResult = JsonConvert.DeserializeObject<LoginResultModel>(json);
            
            if (loginResult!.Token is null  || loginResult is null)
                return;
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex, message: ex.Message);
            return;
        }

        await _localStorage.SetItemAsync(_tokenKey, loginResult.Token);
    }

    public async Task<bool> VerifyEmailAsync(string token, string email)
    {
        var verifyModel = new VerifyModel
        {
            Email = email,
            Token = token
        };

        await _apiService.PostAsync(_apiOptions.Value.AuthEndpoint + _verifyEmailSuffix, verifyModel);
        
        return true;                
    }

    public async Task<bool> RequestPasswordResetAsync(string email)
    {
        var model = new PasswordResetRequestModel
        {
            Email = email
        };

        await _apiService.PostAsync(_apiOptions.Value.AuthEndpoint + _resetPasswordRequestSuffix, model);

        return true;
    }

    public async Task<bool> ResetPasswordAsync(string email, string newPassword, string token)
    {
        var resetModel = new ResetPasswordModel
        {
            Email = email,
            NewPassword = newPassword,
            Token = token
        };

        await _apiService.PostAsync(_apiOptions.Value.AuthEndpoint + _resetPasswordSuffix, resetModel);
        
        return true;
    }   
}