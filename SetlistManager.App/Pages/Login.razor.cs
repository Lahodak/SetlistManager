using Microsoft.AspNetCore.Components;
using MudBlazor;
using SetlistManager.App.Services;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Pages;

public partial class Login
{
    [Inject]
    public required IUserService UserService { get; set; }
    [Inject]
    public required NavigationManager Navigation { get; set; }
    [Inject]
    public required ISnackbar Snackbar { get; set; }

    private readonly LoginRequestModel _loginRequestModel = new();
    private const string _homeUri = "/home";
    private bool _showPassword = false;

    private async Task LoginUser()
    {
        if (string.IsNullOrEmpty(_loginRequestModel.Password) || string.IsNullOrEmpty(_loginRequestModel.Email))
            return;

        var result = await UserService.LogInAsync(_loginRequestModel);

        if (!result)
        {         
            Snackbar.Add("Login failed. Please check your credentials.", Severity.Error);
            return;
        }

        Navigation.NavigateTo(_homeUri);
    }
}