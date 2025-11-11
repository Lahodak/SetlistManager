using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace SetlistManager.App.Layout;

public partial class EmptyLayout
{
    [Inject]
    public required NavigationManager NavigationManager { get; set; }
    [Inject]
    public required ILocalStorageService LocalStorage { get; set; }

    private const string _localStorageKey = "ToggleDarkMode";
    private const string _authTokenKey = "authToken";
    private readonly MudTheme _theme = new();
    private bool _isDarkMode;

    protected override async Task OnInitializedAsync()
    {
        var localData = await LocalStorage.GetItemAsync<bool>(_localStorageKey);
        _isDarkMode = localData;
        StateHasChanged();

        var token = await LocalStorage.GetItemAsStringAsync(_authTokenKey);
        if (string.IsNullOrWhiteSpace(token) && !NavigationManager.Uri.Contains("/login") && !NavigationManager.Uri.Contains("/reset-password")
        && !NavigationManager.Uri.Contains("/request-password-reset"))
        {
            NavigationManager.NavigateTo("/login", true);
        }
    }
}