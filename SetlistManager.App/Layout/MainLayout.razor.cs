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
        public required ILocalStorageService LocalStorage { get; set; }
        [Inject]
        public required IUserService UserService { get; set; }

        private bool _drawerOpen = true;
        private const string _localStorageKey = "ToggleDarkMode";
        private const string _authTokenKey = "authToken";
        private readonly MudTheme _theme = new();
        private bool _isDarkMode;
        private UserModel? userModel;

        protected override async Task OnInitializedAsync()
        {
            var localData = await LocalStorage.GetItemAsync<bool>(_localStorageKey);
            _isDarkMode = localData;
            StateHasChanged();

            var token = await LocalStorage.GetItemAsStringAsync(_authTokenKey);

            if (string.IsNullOrWhiteSpace(token) && !Navigation.Uri.Contains("/login"))
            {
                Navigation.NavigateTo("/login", true);
                return;
            }

            userModel = await UserService.GetUserAsync();
        }

        void DrawerToggle()
        {
            _drawerOpen = !_drawerOpen;
        }

        void NavigateHome()
        {
            Navigation.NavigateTo("/Home");
        }

        private void OpenUserDetail()
        {
            Navigation.NavigateTo("/UserPortal");
        }

        private async Task ToggleTheme()
        {
            _isDarkMode = !_isDarkMode;
            await LocalStorage.SetItemAsync(_localStorageKey, _isDarkMode);
            StateHasChanged();
        }
    }
}