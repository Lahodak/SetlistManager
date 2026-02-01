using Blazored.LocalStorage;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using SetlistManager.App.Options;
using SetlistManager.Common.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;

namespace SetlistManager.App.Services.Implementations;
public class UserService : IUserService
{
    private const string _loginUserSuffix = "/login";
    private const string _tokenKey = "authToken";
    private const string _getUserSetlistsSuffix = "/setlists";
    private const string _verifyEmailSuffix = "/verify";
    private const string _resetPasswordSuffix = "/reset-password";
    private const string _resetPasswordRequestSuffix = "/request-password-reset";
    private const string _friendshipsSuffix = "/friendships";
    private const string _tokensEndpointSuffix = "/tokens";
    private const string _darkModeSettingsKey = "ToggleDarkMode";

    private readonly IHttpClientFactory _httpClientFactory; 
    private readonly ILocalStorageService _localStorage;
    private readonly IApiService _apiService;
    private readonly IOptions<SetlistManagerApiOptions> _apiOptions;

    public UserService(IHttpClientFactory httpClientFactory, ILocalStorageService localStorageService, IApiService apiService, 
        IOptions<SetlistManagerApiOptions> apiOptions)
    {
        _apiOptions = apiOptions;
        _httpClientFactory = httpClientFactory;
        _localStorage = localStorageService;
        _apiService = apiService;
    }

    public async Task AddNewProviderToken(TokenCreateModel tokenModel) 
        => await _apiService.PutAsync(_apiOptions.Value.UsersEndpoint + _tokensEndpointSuffix, tokenModel);

    public async Task<bool> GetUserDarkModeSettings()
    {
        return await _localStorage.GetItemAsync<bool>(_darkModeSettingsKey);
    }

    public async Task UpdateUserDarkModeSettingsAsync(bool newValue)
    {
        await _localStorage.SetItemAsync(_darkModeSettingsKey, newValue);
    }

    public async Task<UserModel?> GetUserAsync()
    {
        var userId = await GetCurrentUserIdAsync();
        
        if (userId is null)
            return null;

        return await _apiService.GetAsync<UserModel?>($"{_apiOptions.Value.UsersEndpoint}/{userId}");
    }

    public async Task<List<SetlistModel>?> GetAllUserSetlists()
    {
        UserModel? user = await GetUserAsync();
        
        if (user is null)
            return null;

        return await _apiService.GetAsync<List<SetlistModel>?>($"{_apiOptions.Value.UsersEndpoint}/{user.Id}{_getUserSetlistsSuffix}");
    }

    public async Task RegisterAsync(RegisterRequestModel model) 
        => await _apiService.PostAsync(_apiOptions.Value.AuthEndpoint, model);

    public async Task LogOutAsync() 
        => await _localStorage.RemoveItemAsync(_tokenKey);

    public async Task<string?> GetUserTokenAsync() 
        => await _localStorage.GetItemAsync<string>(_tokenKey);

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

    public async Task<bool> TryUpdateUser(UserModel user)
    {
        var userId = await GetCurrentUserIdAsync();

        if (userId is null)
            return false;

        await _apiService.PutAsync($"{_apiOptions.Value.UsersEndpoint}/{userId}", user);
        return true;
    }

    public async Task<bool> LogInAsync(LoginRequestModel model)
    {
        var client = _httpClientFactory.CreateClient();

        var message = await client.PostAsJsonAsync($"{_apiOptions.Value.AuthEndpoint}{_loginUserSuffix}", model);

        if (!message.IsSuccessStatusCode)
            return false;
        
        var loginResult = await message.Content.ReadFromJsonAsync<LoginResultModel>();

        if (loginResult?.Token is null)
            return false;

        await _localStorage.SetItemAsync(_tokenKey, loginResult.Token);
        return true;
    }

    public async Task<bool> VerifyEmailAsync(string token, string email)
    {
        var verifyModel = new VerifyModel
        {
            Email = email,
            Token = token
        };

        await _apiService.PostAsync($"{_apiOptions.Value.AuthEndpoint}{_verifyEmailSuffix}", verifyModel);
        
        return true;                
    }

    public async Task<bool> RequestPasswordResetAsync(string email)
    {
        var model = new PasswordResetRequestModel
        {
            Email = email
        };

        await _apiService.PostAsync($"{_apiOptions.Value.AuthEndpoint}{_resetPasswordRequestSuffix}", model);

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

        await _apiService.PostAsync($"{_apiOptions.Value.AuthEndpoint}{_resetPasswordSuffix}", resetModel);
        
        return true;
    }

    public async Task<int?> GetCurrentUserIdAsync()
    {
        var token = await _localStorage.GetItemAsync<string>(_tokenKey);
        if (string.IsNullOrWhiteSpace(token))
            return null;

        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(token))
            return null;

        var jwt = handler.ReadJwtToken(token);

        var userIdClaim = jwt.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier
                              || c.Type == "nameid");

        return int.TryParse(userIdClaim?.Value, out var userId)
            ? userId
            : null;
    }

    public async Task<PagedResponse<FriendModel>?> GetUserFriendshipsAsync(PagedRequest request)
    {
        var userId = await GetCurrentUserIdAsync();

        if (userId is null)
            return null;

        UriBuilder uri = new($"{_apiOptions.Value.UsersEndpoint}/{userId}{_friendshipsSuffix}")
        {
            Query = new QueryBuilder
            {
                { "PageSize", request.PageSize.ToString() },
                { "PageIndex", request.PageIndex.ToString() },
                { "Query", request.Query ?? string.Empty }
            }.ToString()
        };

        return await _apiService.GetAsync<PagedResponse<FriendModel>>(uri.ToString());
    }

    public async Task HandleFriendshipRequestAsync(FriendshipRequestModel friendshipRequest)
    {
        var initiatorId = await GetCurrentUserIdAsync();

        if (initiatorId is null)
            return;

        await _apiService.PostAsync($"{_apiOptions.Value.UsersEndpoint}/{initiatorId}{_friendshipsSuffix}", friendshipRequest);
    }

    public async Task<bool> TryRemoveFriendshipAsync(int friendshipId)
    {
        var initiatorId = await GetCurrentUserIdAsync();
        
        if (initiatorId is null)
            return false;
        
        return await _apiService.TryDeleteAsync($"{_apiOptions.Value.UsersEndpoint}/{initiatorId}{_friendshipsSuffix}/{friendshipId}");
    }

    public async Task<bool> TryAcceptFriendshipAsync(int friendshipId)
    {
        var initiatorId = await GetCurrentUserIdAsync();
        
        if (initiatorId is null)
            return false;

        return await _apiService.TryPutAsync($"{_apiOptions.Value.UsersEndpoint}/{initiatorId}{_friendshipsSuffix}/{friendshipId}", "");
    }

    public async Task<PagedResponse<UserViewModel>?> GetPagedUsersAsync(PagedRequest request)
    {
        UriBuilder uri = new($"{_apiOptions.Value.UsersEndpoint}")
        {
            Query = new QueryBuilder
            {
                { "PageSize", request.PageSize.ToString() },
                { "PageIndex", request.PageIndex.ToString() },
                { "Query", request.Query ?? string.Empty }
            }.ToString()
        };
        
        return await _apiService.GetAsync<PagedResponse<UserViewModel>>(uri.ToString());
    }
}