using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SetlistManager.App.Services;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Layout
{
    public partial class MainLayout
    {
        [Inject]
        public required NavigationManager Navigation { get; set; }
        [Inject]
        public required IUserService UserService { get; set; }

        private const string _loginUri = "/login";
        private const string _homeUri = "/Home";
        private const string _userPortalUri = "/UserPortal";
        private bool _drawerOpen = true;
        private readonly MudTheme _theme = new();
        private bool _isDarkMode;
        private UserModel? userModel;

        protected override async Task OnInitializedAsync()
        {                       
            _isDarkMode = await UserService.GetUserDarkModeSettings();
            StateHasChanged();

            var token = await UserService.GetUserTokenAsync();

            if (string.IsNullOrWhiteSpace(token) && !Navigation.Uri.Contains(_loginUri))
            {
                Navigation.NavigateTo(_loginUri, true);
                return;
            }

            userModel = await UserService.GetUserAsync();
        }

        private void DrawerToggle()
        {
            _drawerOpen = !_drawerOpen;
        }

        private void NavigateHome()
        {
            Navigation.NavigateTo(_homeUri);
        }

        private void OpenUserDetail()
        {
            Navigation.NavigateTo(_userPortalUri);
        }

        private async Task ToggleTheme()
        {
            _isDarkMode = !_isDarkMode;

            await UserService.UpdateUserDarkModeSettingsAsync(_isDarkMode);
            StateHasChanged();
        }
    }
}