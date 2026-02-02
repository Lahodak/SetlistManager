using Blazored.LocalStorage;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using SetlistManager.App.Options;
using SetlistManager.Common.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SetlistManager.App.Services.Implementations;
public class UserService : IUserService
{
    private readonly ILocalStorageService _localStorage;
    private readonly IApiService _apiService;
    private readonly SetlistManagerApiOptions _apiOptions;
    
    private const string _loginUserSuffix = "/login";
    private const string _tokenKey = "authToken";
    private const string _verifyEmailSuffix = "/verify-email";
    private const string _resetPasswordSuffix = "/reset-password";
    private const string _verifyTokenSuffix = "/verify-token";
    private const string _resetPasswordRequestSuffix = "/request-password-reset";
    private const string _friendshipsSuffix = "/friendships";
    private const string _meSuffix = "/me";
    private const string _darkModeSettingsKey = "ToggleDarkMode";

    public UserService(ILocalStorageService localStorageService, IApiService apiService, IOptions<SetlistManagerApiOptions> apiOptions)
    {
        _apiOptions = apiOptions.Value;
        _localStorage = localStorageService;
        _apiService = apiService;
    }

    public async Task<bool> GetUserDarkModeSettings() 
        => await _localStorage.GetItemAsync<bool>(_darkModeSettingsKey);

    public async Task UpdateUserDarkModeSettingsAsync(bool newValue) 
        => await _localStorage.SetItemAsync(_darkModeSettingsKey, newValue);

    public async Task<UserModel?> GetUserAsync() 
        => await _apiService.GetAsync<UserModel?>($"{_apiOptions.UsersEndpoint}{_meSuffix}");

    public async Task RegisterAsync(RegisterRequestModel model) 
        => await _apiService.PostAsync(_apiOptions.AuthEndpoint, model);

    public async Task LogOutAsync() 
        => await _localStorage.RemoveItemAsync(_tokenKey);

    public async Task<string?> GetUserTokenAsync() 
        => await _localStorage.GetItemAsync<string>(_tokenKey);

    public async Task<bool> VerifyStoredToken() 
        => await _apiService.PostAsync($"{_apiOptions.AuthEndpoint}{_verifyTokenSuffix}");

    public async Task<bool> TryUpdateUser(UserModel user)
    {
        var userId = await GetCurrentUserIdAsync();

        if (userId is null)
            return false;

        await _apiService.PutAsync($"{_apiOptions.UsersEndpoint}/{userId}", user);
        return true;
    }

    public async Task<bool> LogInAsync(LoginRequestModel model)
    {
        var result = await _apiService.PostAsync<LoginRequestModel, LoginResultModel>($"{_apiOptions.AuthEndpoint}{_loginUserSuffix}", model);

        if (result?.Token is null)
            return false;

        await _localStorage.SetItemAsync(_tokenKey, result.Token);
        return true;
    }

    public async Task<bool> VerifyEmailAsync(string token, string email)
    {
        var verifyModel = new VerifyModel
        {
            Email = email,
            Token = token
        };

        await _apiService.PostAsync($"{_apiOptions.AuthEndpoint}{_verifyEmailSuffix}", verifyModel);
        
        return true;                
    }

    public async Task<bool> RequestPasswordResetAsync(string email)
    {
        var model = new PasswordResetRequestModel
        {
            Email = email
        };

        await _apiService.PostAsync($"{_apiOptions.AuthEndpoint}{_resetPasswordRequestSuffix}", model);

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

        await _apiService.PostAsync($"{_apiOptions.AuthEndpoint}{_resetPasswordSuffix}", resetModel);
        
        return true;
    }

    public async Task<int?> GetCurrentUserIdAsync()
    {
        var token = await _localStorage.GetItemAsync<string>(_tokenKey);
        if (string.IsNullOrWhiteSpace(token))
            return null;

        var user = await GetUserAsync();
        return user?.Id;
    }

    public async Task<PagedResponse<FriendModel>?> GetUserFriendshipsAsync(PagedRequest request)
    {
        var userId = await GetCurrentUserIdAsync();

        if (userId is null)
            return null;

        UriBuilder uri = new($"{_apiOptions.UsersEndpoint}/{userId}{_friendshipsSuffix}")
        {
            Query = new QueryBuilder
            {
                { nameof(request.PageSize), request.PageSize.ToString() },
                { nameof(request.PageIndex), request.PageIndex.ToString() },
                { nameof(request.Query), request.Query ?? string.Empty }
            }.ToString()
        };

        return await _apiService.GetAsync<PagedResponse<FriendModel>>(uri.ToString());
    }

    public async Task HandleFriendshipRequestAsync(FriendshipRequestModel friendshipRequest)
    {
        var initiatorId = await GetCurrentUserIdAsync();

        if (initiatorId is null)
            return;

        await _apiService.PostAsync($"{_apiOptions.UsersEndpoint}/{initiatorId}{_friendshipsSuffix}", friendshipRequest);
    }

    public async Task<bool> TryRemoveFriendshipAsync(int friendshipId)
    {
        var initiatorId = await GetCurrentUserIdAsync();
        
        if (initiatorId is null)
            return false;
        
        return await _apiService.TryDeleteAsync($"{_apiOptions.UsersEndpoint}/{initiatorId}{_friendshipsSuffix}/{friendshipId}");
    }

    public async Task<bool> TryAcceptFriendshipAsync(int friendshipId)
    {
        var initiatorId = await GetCurrentUserIdAsync();
        
        if (initiatorId is null)
            return false;

        return await _apiService.TryPutAsync($"{_apiOptions.UsersEndpoint}/{initiatorId}{_friendshipsSuffix}/{friendshipId}", "");
    }

    public async Task<PagedResponse<UserViewModel>?> GetPagedUsersAsync(PagedRequest request)
    {
        UriBuilder uri = new($"{_apiOptions.UsersEndpoint}")
        {
            Query = new QueryBuilder
            {
                { nameof(request.PageSize), request.PageSize.ToString() },
                { nameof(request.PageIndex), request.PageIndex.ToString() },
                { nameof(request.Query), request.Query ?? string.Empty }
            }.ToString()
        };
        
        return await _apiService.GetAsync<PagedResponse<UserViewModel>>(uri.ToString());
    }
}