using Blazored.LocalStorage;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using SetlistManager.App.Extensions;
using SetlistManager.App.Models;
using SetlistManager.App.Options;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Services.Implementations;

public class UserService : IUserService
{
    private readonly ILocalStorageService _localStorage;
    private readonly IApiService _apiService;
    private readonly string _apiUsersEndpointPath;
    private readonly string _apiAuthEndpointPath;

    private const string _loginUserSuffix = "/login";
    private const string _tokenKey = "authToken";
    private const string _verifyEmailSuffix = "/verify-email";
    private const string _resetPasswordSuffix = "/reset-password";
    private const string _verifyTokenSuffix = "/verify-token";
    private const string _resetPasswordRequestSuffix = "/request-password-reset";
    private const string _friendshipsSuffix = "/friendships";
    private const string _meSuffix = "/me";
    private const string _darkModeSettingsKey = "ToggleDarkMode";
    private const string _panelConfigStorageKey = "roomPanelConfig";

    public UserService(ILocalStorageService localStorageService, IApiService apiService, IOptions<SetlistManagerApiOptions> apiOptions)
    {
        _localStorage = localStorageService;
        _apiService = apiService;
        _apiUsersEndpointPath = apiOptions.Value.BaseUrl + apiOptions.Value.UsersEndpoint;
        _apiAuthEndpointPath = apiOptions.Value.BaseUrl + apiOptions.Value.AuthEndpoint;
    }

    public async Task<bool> GetUserDarkModeSettings()
        => await _localStorage.GetItemAsync<bool>(_darkModeSettingsKey);

    public async Task UpdateUserDarkModeSettingsAsync(bool newValue)
        => await _localStorage.SetItemAsync(_darkModeSettingsKey, newValue);

    public async Task<UserModel?> GetUserAsync()
        => await _apiService.GetAsync<UserModel?>($"{_apiUsersEndpointPath}{_meSuffix}");

    public async Task RegisterAsync(RegisterRequestModel model)
        => await _apiService.TryPostAsync(_apiAuthEndpointPath, model);

    public async Task LogOutAsync()
        => await _localStorage.RemoveItemAsync(_tokenKey);

    public async Task<string?> GetUserTokenAsync()
        => await _localStorage.GetItemAsync<string>(_tokenKey);

    public async Task<bool> VerifyStoredToken()
        => await _apiService.PostAsync($"{_apiAuthEndpointPath}{_verifyTokenSuffix}");

    public async Task<bool> TryUpdateUser(UserModel user)
    {
        var userId = await GetCurrentUserIdAsync();

        if (userId is null)
            return false;
        
        return await _apiService.TryPutAsync($"{_apiUsersEndpointPath}/{userId}", user);
    }

    public async Task<List<PanelType>?> GetPanelConfigAsync()
    {        
        var savedConfig = await _localStorage.GetItemAsStringAsync(_panelConfigStorageKey);
        
        if (string.IsNullOrEmpty(savedConfig)) 
            return null;

        List<PanelType>? panels = savedConfig.Split(',')
                .Select(p => Enum.TryParse<PanelType>(p, out var panel) ? panel : (PanelType?)null)
                .Where(p => p.HasValue)
                .Select(p => p!.Value)
                .ToList();
        
        return panels;
    }

    public async Task SavePanelConfigAsync(List<PanelType> panels)
    {
        var configString = string.Join(",", panels);
        await _localStorage.SetItemAsStringAsync(_panelConfigStorageKey, configString);
    }

    public async Task<bool> LogInAsync(LoginRequestModel model)
    {
        var result = await _apiService.PostAsync<LoginRequestModel, LoginResultModel>(
            $"{_apiAuthEndpointPath}{_loginUserSuffix}",
            model
        );

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

        return await _apiService.TryPostAsync($"{_apiAuthEndpointPath}{_verifyEmailSuffix}", verifyModel);
    }

    public async Task<bool> RequestPasswordResetAsync(string email)
    {
        var model = new PasswordResetRequestModel
        {
            Email = email
        };

        return await _apiService.TryPostAsync($"{_apiAuthEndpointPath}{_resetPasswordRequestSuffix}", model);
    }

    public async Task<bool> ResetPasswordAsync(string email, string newPassword, string token)
    {
        var resetModel = new ResetPasswordModel
        {
            Email = email,
            NewPassword = newPassword,
            Token = token
        };

        await _apiService.PostAsync($"{_apiAuthEndpointPath}{_resetPasswordSuffix}", resetModel);
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

        var uri = request.ToPagedRequestUri($"{_apiUsersEndpointPath}/{userId}{_friendshipsSuffix}");
        return await _apiService.GetAsync<PagedResponse<FriendModel>>(uri);
    }

    public async Task HandleFriendshipRequestAsync(FriendshipRequestModel friendshipRequest)
    {
        var initiatorId = await GetCurrentUserIdAsync();

        if (initiatorId is null)
            return;

        await _apiService.TryPostAsync($"{_apiUsersEndpointPath}/{initiatorId}{_friendshipsSuffix}", friendshipRequest);
    }

    public async Task<bool> TryRemoveFriendshipAsync(int friendshipId)
    {
        var initiatorId = await GetCurrentUserIdAsync();

        if (initiatorId is null)
            return false;

        return await _apiService.TryDeleteAsync($"{_apiUsersEndpointPath}/{initiatorId}{_friendshipsSuffix}/{friendshipId}");
    }

    public async Task<bool> TryAcceptFriendshipAsync(int friendshipId)
    {
        var initiatorId = await GetCurrentUserIdAsync();

        if (initiatorId is null)
            return false;

        return await _apiService.TryPutAsync($"{_apiUsersEndpointPath}/{initiatorId}{_friendshipsSuffix}/{friendshipId}", "");
    }

    public async Task<PagedResponse<UserViewModel>?> GetPagedUsersAsync(PagedRequest request)
    {
        var uri = request.ToPagedRequestUri(_apiUsersEndpointPath);
        return await _apiService.GetAsync<PagedResponse<UserViewModel>>(uri);
    }

    public async Task<bool> TryRevokeTokenAsync(int tokenId)
    {
        var userId = await GetCurrentUserIdAsync();

        if (userId is null)
            return false;
        
        return await _apiService.TryDeleteAsync($"{_apiUsersEndpointPath}/{userId}/tokens/{tokenId}");
    }
}