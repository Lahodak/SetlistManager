using Microsoft.AspNetCore.Components;
using MudBlazor;
using SetlistManager.App.Services;

namespace SetlistManager.App.Layout;

public partial class EmptyLayout
{
    [Inject]
    public required NavigationManager NavigationManager { get; set; }
    [Inject]
    public required IUserService UserService { get; set; }

    private readonly MudTheme _theme = new();
    private bool _isDarkMode;
    
    private const string _loginUri = "/login";
    private const string _resetPasswordUri = "/reset-password";
    private const string _requestResetPasswordUri = "/request-password-reset";
    private const string _landingUri = "/";

    protected override async Task OnInitializedAsync()
    {        
        _isDarkMode = await UserService.GetUserDarkModeSettings();
        
        StateHasChanged();

        var token = await UserService.GetUserTokenAsync();

        if (string.IsNullOrWhiteSpace(token) && !NavigationManager.Uri.Contains(_loginUri) && !NavigationManager.Uri.Contains(_resetPasswordUri)
            && !NavigationManager.Uri.Contains(_requestResetPasswordUri) && !NavigationManager.Uri.Contains(_landingUri))
        {
            NavigationManager.NavigateTo(_loginUri);
        }
    }
}