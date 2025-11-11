using Microsoft.AspNetCore.Components;
using SetlistManager.App.Services;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Pages;

public partial class Login
{
    [Inject]
    public required IUserService UserService { get; set; }
    [Inject]
    public required NavigationManager Navigation { get; set; }

    private readonly LoginRequestModel _loginRequestModel = new();

    private async Task LoginUser()
    {
        if (_loginRequestModel.Password == string.Empty || _loginRequestModel.Email == string.Empty)
            return;

        await UserService.LogInAsync(_loginRequestModel);

        if (await UserService.GetUserToken() is not null)
        {
            Navigation.NavigateTo("/");
        }
    }
}